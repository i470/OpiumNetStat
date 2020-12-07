using OpiumNetStat.events;
using OpiumNetStat.model;
using OpiumNetStat.Model;
using OpiumNetStat.Pipeline;
using Prism.Events;
using System.Collections.Generic;
using System.Diagnostics;

namespace OpiumNetStat.services
{
    public class DataPipeLineService : IDataPipeLineService
    {
        IEventAggregator ea;
        IDataBaseService db;
        IIpInfoService ips;

        public DataPipeLineService(IEventAggregator _ea, IDataBaseService _db, IIpInfoService _ips)
        {
            ea = _ea;

            ea.GetEvent<NetStatReadEvent>().Subscribe(pushPipeLine);

            db = _db;
            ips = _ips;
        }



        private void pushPipeLine(IList<PortInfo> portlist)
        {
            PipeLineController.Begin(PipelineAsync(portlist), ex =>
            {
                Debug.Write(ex.Message);
                ea.GetEvent<ExceptionEvent>().Publish(ex);
            });
        }


        private async IAsyncEnumerable<IPipeLine> PipelineAsync(IList<PortInfo> portlist)
        {

            //step 1 -- get IP Geo 

            var netStatList = new List<NetStatResult>();

            foreach (var ip in portlist)
            {
               await ips.GetIPInfo(ip.remote_ip, (ex, net) =>
                {

                    if (ex == null)
                    {
                        netStatList.Add(net);
                    }

                });

            }



            //step 2 -- get PID info


            //step 3 -- update DB 



            yield return null;
        }
    }
}
