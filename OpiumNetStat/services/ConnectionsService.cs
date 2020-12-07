using Newtonsoft.Json;
using OpiumNetStat.events;
using OpiumNetStat.model;
using Prism.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
      

        public ConnectionsService(IEventAggregator ea)
        {
            _ea = ea;

        }


        public void StartWork()
        {
            _netStatCollection = new ConcurrentDictionary<string, NetStatResult>();


            wtoken = new CancellationTokenSource();
            task = (ActionBlock<DateTimeOffset>)CreateNeverEndingTask( now =>  DoWorkAsync(), wtoken.Token);
            task.Post(DateTimeOffset.Now);
        }

        private  void DoWorkAsync()
        {
           var ports =  NetStatService.GetNetStatPorts();

            _ea.GetEvent<NetStatReadEvent>().Publish(ports);


            //var ip = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();

            //var pids =  NetStatService.GetNetStatPorts();

            //foreach (var tcp in ip.GetActiveTcpConnections())
            //{
            //    if (!tcp.RemoteEndPoint.Address.ToString().Equals("127.0.0.1"))
            //    {

            //        var record = _db.GetNetStatRecord(tcp.RemoteEndPoint.Address.ToString());

            //        if (record is null)
            //        {

            //            record = new NetStatResult(tcp);

            //        }
            //        else
            //        {
            //            record.LastSeen = DateTime.Now;
            //            record.ConnectionStatus = tcp.State.ToString();
            //        }

            //        if (string.IsNullOrEmpty(record.Country))
            //        {

            //            record = await _updateIpGeoAsync(record);

            //        }


            //        //find matching pid

            //        var pid = pids.Where(x=>x.remote_ip.Trim().Equals(record.RemoteIP)).First();
            //        if(!(pid is null))
            //        {
            //            record.PortNumber = Int16.Parse(pid.port_number);
            //            record.Software = pid.process_name;

            //        }

            //        _db.Upsert(record);
            //        _ea.GetEvent<ConnectionUpdateEvent>().Publish(record);
            //    }


            //}

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
