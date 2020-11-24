using LiteDB;
using Newtonsoft.Json;
using OpiumNetStat.events;
using OpiumNetStat.model;
using Prism.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace OpiumNetStat.services
{
    public class ConnectionsService : IConnectionsService
    {
        IEventAggregator _ea;
        CancellationTokenSource wtoken;
        ActionBlock<DateTimeOffset> task;
        ConcurrentDictionary<string, NetStatResult> _netStatCollection;
        DataBaseService _db;

        public ConnectionsService(IEventAggregator ea)
        {
            _ea = ea;
            _db = new DataBaseService();
        }


        public void StartWork()
        {
            _netStatCollection = new ConcurrentDictionary<string, NetStatResult>();


            wtoken = new CancellationTokenSource();
            task = (ActionBlock<DateTimeOffset>)CreateNeverEndingTask(async now => await DoWorkAsync(), wtoken.Token);
            task.Post(DateTimeOffset.Now);
        }

        private async Task DoWorkAsync()
        {


            var ip = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();



            foreach (var tcp in ip.GetActiveTcpConnections())
            {
                if (!tcp.RemoteEndPoint.Address.ToString().Equals("127.0.0.1"))
                {

                    var record = _db.GetNetStatRecord(tcp.RemoteEndPoint.Address.ToString());

                    if (record is null)
                    {

                        record = new NetStatResult(tcp);

                    }
                    else
                    {
                        record.LastSeen = DateTime.Now;
                        record.ConnectionStatus = tcp.State.ToString();
                    }

                    if (string.IsNullOrEmpty(record.Country))
                    {
                        record = await _updateIpGeoAsync(record);

                    }

                    _db.Upsert(record);
                    _ea.GetEvent<ConnectionUpdateEvent>().Publish(record);
                }


            }

        }

        private async Task<NetStatResult> _updateIpGeoAsync(NetStatResult record)
        {

            using (var client = new WebClient())
            {
                try
                {
                    var uri = new Uri($"http://ip-api.com/json/{record.RemoteIP}");


                    var json = await client.DownloadStringTaskAsync(uri);
                    var result = JsonConvert.DeserializeObject<Host>(json);

                    record.City = result.City;
                    record.Country = result.Country;
                    record.Org = result.Org;
                    record.CountryCode = result.CountryCode;


                    return record;
                }
                catch (Exception ex)
                {
                    Thread.Sleep(100);
                    return record;
                }

            }
        }


        void StopWork()
        {

            using (wtoken)
            {

                wtoken.Cancel();
            }

            wtoken = null;
            task = null;
        }


        ITargetBlock<DateTimeOffset> CreateNeverEndingTask(Action<DateTimeOffset> action, CancellationToken cancellationToken)
        {
            // Validate parameters.
            if (action == null) throw new ArgumentNullException("action");


            ActionBlock<DateTimeOffset> block = null;

            block = new ActionBlock<DateTimeOffset>(async now =>
            {

                action(now);
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken).ConfigureAwait(false);
                block.Post(DateTimeOffset.Now);
            },
            new ExecutionDataflowBlockOptions
            {
                CancellationToken = cancellationToken
            });

            return block;
        }

        public void Get24HourDataAsync()
        {
            using (var db = new LiteDatabase(DB.Path))
            {
                var col = db.GetCollection<NetStatResult>(DB.CollConnections);

                var data = col.FindAll().ToList();

                foreach (var record in data)
                {


                    _ea.GetEvent<ConnectionUpdateEvent>().Publish(record);
                }

            }

        }
    }
}
