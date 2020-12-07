using OpiumNetStat.events;
using OpiumNetStat.Model;
using OpiumNetStat.Pipeline;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpiumNetStat.services
{
    public class DataPipeLineService
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
            PipeLineController.Begin(Pipeline(portlist), ex => { Debug.Write(ex.Message); });
        }


        private IEnumerable<IPipeLine> Pipeline(IList<PortInfo> portlist)
        {

            //step 1 -- get IP Geo 



            //step 2 -- get PID info


            //step 3 -- update DB 



            yield return null;
        }
    }
}
