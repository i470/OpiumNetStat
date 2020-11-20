using LiteDB;
using Newtonsoft.Json;
using Opium_NetStat.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Opium_NetStat.services
{
    public class ConnectionsService
    {
        CancellationTokenSource wtoken;
        ActionBlock<DateTimeOffset> task;

        public void StartWork()
        {
            // Create the token source.
            wtoken = new CancellationTokenSource();

            // Set the task.
            task = (ActionBlock<DateTimeOffset>)CreateNeverEndingTask(async now => await DoWorkAsync(), wtoken.Token);

            // Start the task.  Post the time.
            task.Post(DateTimeOffset.Now);
        }

        private async Task DoWorkAsync()
        {
            var ip = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();

            string path = @"db/netstat.db";

            using (var db = new LiteDatabase(path))
            {
                var col = db.GetCollection<NetStatResult>("connections");




                foreach (var tcp in ip.GetActiveTcpConnections())
                {
                    if (!tcp.RemoteEndPoint.Address.ToString().Equals("127.0.0.1"))
                    {
                        var exist = col.Exists(x => x.RemoteIP.Equals(tcp.RemoteEndPoint.Address.ToString()));

                        var record = col.Query().Where(x => x.RemoteIP.Equals(tcp.RemoteEndPoint.Address.ToString())).FirstOrDefault();

                        if (record is null)
                        {

                            record = new NetStatResult
                            {
                                LocalIP = tcp.LocalEndPoint.Address.ToString(),
                                RemoteIP = tcp.RemoteEndPoint.Address.ToString(),
                                ConnectionStatus = tcp.State.ToString(),
                                PortNumber = (short)tcp.RemoteEndPoint.Port,
                                LastSeen = DateTime.Now
                            };

                           

                        }
                        else
                        {
                            record.LastSeen = DateTime.Now;
                            record.ConnectionStatus = tcp.State.ToString();
                        }

                        if (string.IsNullOrEmpty(record.Country))
                        {
                            await _updateIpGeo(record);

                        }

                        col.Upsert(record);
                        col.EnsureIndex(x => x.RemoteIP);
                    }
                  
                   
                }
            }
        }


        private async Task _updateIpGeo(NetStatResult record)
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
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(100);

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
                      
            block = new ActionBlock<DateTimeOffset>(async now => {
              
                action(now);
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken).ConfigureAwait(false);
                block.Post(DateTimeOffset.Now);
            },
            new ExecutionDataflowBlockOptions
            {
                CancellationToken = cancellationToken
            });

            return block;
        }

    }
}
