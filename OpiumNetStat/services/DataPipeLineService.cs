using OpiumNetStat.events;
using OpiumNetStat.model;
using OpiumNetStat.Model;
using OpiumNetStat.Pipeline;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace OpiumNetStat.services
{
    public class DataPipeLineService : IDataPipeLineService
    {
        IEventAggregator ea;
        IDataBaseService db;
        IIpInfoService ips;
        List<NetStatItemViewModel> netStatList = new List<NetStatItemViewModel>();
        Dictionary<string, NetStatItemViewModel> netStatDict = new Dictionary<string, NetStatItemViewModel>();

        PipelineAction _getGeoInfoPipeline = new PipelineAction();
        PipelineAction _updateDBPipeline = new PipelineAction();
        PipelineAction _publishNewRecordsPipeline = new PipelineAction();

        public DataPipeLineService(IEventAggregator _ea, IDataBaseService _db, IIpInfoService _ips)
        {

            ea = _ea;
            db = _db;
            ips = _ips;

            ea.GetEvent<NetStatReadEvent>().Subscribe(pushPipeLine);
        }


        private void pushPipeLine(ProcessIPInfo proc)
        {
            //if(netStatDict.ContainsKey(proc.RemoteIp))
            //{
            //    var er = netStatDict[proc.RemoteIp];
            //    er.ProcessInfo = proc;

            //    if(er.IpInfo is null)
            //    {
            //       // er.IpInfo = ips.GetIPInfo(proc).Result;
            //    }

            //    ea.GetEvent<ConnectionUpdateEvent>().Publish(er);
            //}
            //else
            //{
            //    var ipInfo = ips.GetIPInfo(proc).Result;

            //    if(ipInfo!=null)
            //    {
            //        var netstat = new NetStatItemViewModel(proc);
            //        netstat.IpInfo = ipInfo;
            //        netstat.Host = GetHostByAddress(proc.RemoteIp);
            //        netStatDict.Add(proc.RemoteIp, netstat);

            //        ea.GetEvent<ConnectionUpdateEvent>().Publish(netstat);
            //    }
            //}

        }

        public string LookupProcess(int pid)
        {
            string procName;
            try
            {
                procName =Process.GetProcessById(pid).ProcessName ;
            }
            catch (Exception) { procName = "-"; }
            return procName;
        }


        //private  IEnumerable<IPipeLine> Pipeline(IList<ProcessIPInfo> processlist)
        //{

        //    ////step 1 -- get IP Geo 
        //    //var geoList = new List<NetStatResult>();


        //    //_getGeoInfoPipeline.Execute = async () =>
        //    //{
        //    //    geoList =  await GetGeoinfoAsync(processlist);

        //    //    _getGeoInfoPipeline.Invoked();
        //    //};

        //    //yield return _getGeoInfoPipeline;


        //    //_updateDBPipeline.Execute = () => {

        //    //    _updateDB(netStatList);
        //    //    _updateDBPipeline.Invoked();
        //    //};

        //    //yield return _updateDBPipeline;



        //}

      

        public string GetHostByAddress(string ipAddress)
        {
            try
            {
                return Dns.GetHostEntry(ipAddress).HostName;
             
            }
            catch (Exception)
            {
                return null;
            }
        }



    }
}
