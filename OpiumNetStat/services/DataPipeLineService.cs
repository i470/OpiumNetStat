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
        List<NetStatResult> netStatList = new List<NetStatResult>();
        Dictionary<string, NetStatResult> netStatDict = new Dictionary<string, NetStatResult>();

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
            if(netStatDict.ContainsKey(proc.remote_ip))
            {
                var er = netStatDict[proc.remote_ip];

                if(string.IsNullOrEmpty(er.CountryCode))
                {
                    var ipInfo = ips.GetIPInfo(proc).Result;


                    if (ipInfo != null && string.IsNullOrEmpty(ipInfo.CountryCode))
                    {
                        er.CountryCode = ipInfo.CountryCode;
                    }
                    
                }

                if(string.IsNullOrEmpty(er.Software) || er.Software.Contains("-") || er.Software.Contains("Idle"))
                {
                   er.Software = LookupProcess(er.PID);
                }

                ea.GetEvent<ConnectionUpdateEvent>().Publish(er);
            }
            else
            {
                var ipInfo = ips.GetIPInfo(proc).Result;

                if(ipInfo!=null)
                {
                    var netstat = new NetStatResult(proc);
                    netstat.CountryCode = ipInfo.CountryCode;
                    netstat.Org = ipInfo.Org;
                    netstat.Host = GetHostByAddress(proc.remote_ip);
                    netstat.Software = LookupProcess(proc.PID);

                    netStatDict.Add(proc.remote_ip, netstat);

                    ea.GetEvent<ConnectionUpdateEvent>().Publish(netstat);
                }
            }

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

        private void _updateDB(List<NetStatResult> geoList)
        {
            foreach (var geo in geoList)
            {
                var record = db.GetNetStatRecord(geo.RemoteIP);

                if (record is null)
                {
                    db.Upsert(record);

                }
                else
                {
                    record.ConnectionStatus = geo.ConnectionStatus;
                    record.LastSeen = geo.LastSeen;
                    db.Upsert(record);
                }
            }
        }

        public string GetHostByAddress(string ipAddress)
        {
            try
            {
                var host = Dns.GetHostEntry(ipAddress).HostName;
                var uri = host.Split('.');
                var l = uri.Length;
                var domain = $"{uri[l-2]}.{uri[l - 1]}";
                return domain;
            }
            catch (Exception)
            {
                return ipAddress;
            }
        }



    }
}
