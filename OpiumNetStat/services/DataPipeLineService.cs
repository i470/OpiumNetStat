using OpiumNetStat.events;
using OpiumNetStat.model;
using OpiumNetStat.Model;
using OpiumNetStat.Pipeline;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OpiumNetStat.services
{
    public class DataPipeLineService : IDataPipeLineService
    {
        IEventAggregator ea;
        IDataBaseService db;
        IIpInfoService ips;
        List<NetStatResult> netStatList = new List<NetStatResult>();

        PipelineAction _getGeoInfoPipeline = new PipelineAction();
        PipelineAction _updateDBPipeline = new PipelineAction();
        PipelineAction _publishNewRecordsPipeline = new PipelineAction();

        public  DataPipeLineService(IEventAggregator _ea, IDataBaseService _db, IIpInfoService _ips)
        {
    
            ea = _ea;
            db = _db;
            ips = _ips;

            ea.GetEvent<NetStatReadEvent>().Subscribe(pushPipeLine);
        }


        private void pushPipeLine(IList<PortInfo> portlist)
        {
            PipeLineController.Begin(Pipeline(portlist), ex =>
            {
                Debug.Write(ex.Message);
                ea.GetEvent<ExceptionEvent>().Publish(ex);
            });
        }


        private  IEnumerable<IPipeLine> Pipeline(IList<PortInfo> portlist)
        {

            //step 1 -- get IP Geo 
            var geoList = new List<NetStatResult>();


            _getGeoInfoPipeline.Execute = async () =>
            {
                geoList =  await GetGeoinfoAsync(portlist);

                _getGeoInfoPipeline.Invoked();
            };

            yield return _getGeoInfoPipeline;


            //_updateDBPipeline.Execute = () => {

            //    _updateDB(netStatList);
            //    _updateDBPipeline.Invoked();
            //};

            //yield return _updateDBPipeline;

            ea.GetEvent<ConnectionUpdateEvent>().Publish(geoList);
          
        }

        private void _updateDB(List<NetStatResult> geoList)
        {
            foreach (var geo in geoList)
            {
                var record = db.GetNetStatRecord(geo.RemoteIP);

                if(record is null)
                {
                    db.Upsert(record);

                }else
                {
                    record.ConnectionStatus = geo.ConnectionStatus;
                    record.LastSeen = geo.LastSeen;
                    db.Upsert(record);
                }
            }
        }

        private async Task<List<NetStatResult>> GetGeoinfoAsync(IList<PortInfo> portlist)
        {
            var geoList = new List<NetStatResult>();

            try
            {
                foreach (var ip in portlist.Where(x=>!x.remote_ip.Equals("127.0.0.1")))
                {
                    if(!ip.remote_ip.StartsWith("192") &&  !ip.remote_ip.StartsWith("10."))
                    {
                        var record = netStatList.Where(x => x.RemoteIP.Equals(ip.remote_ip.Trim())).FirstOrDefault();

                        if (record is null)
                        {
                            record = await ips.GetIPInfo(ip);

                            if (record != null)
                            {
                                record.LastSeen = DateTime.Now;
                                geoList.Add(record);
                                netStatList.Add(record);
                            }

                        }
                        else
                        {
                            record.ConnectionStatus = ip.status;
                            record.LastSeen = DateTime.Now;
                        }
                    }
                    
                }

                return geoList;
            }
            catch (Exception ex)
            {
               
                Debug.Write(ex.Message);
                return null;
            }
           
        }

        
    }
}
