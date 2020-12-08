using OpiumNetStat.model;
using System.Collections.Generic;

namespace OpiumNetStat.services
{
    public interface IDataBaseService
    {
    
        List<NetStatResult> Get24HourDataAsync();
        NetStatResult GetNetStatRecord(string ip);
        bool RemoteIpExists(string ip);
        NetStatResult Upsert(NetStatResult netStat);
    }
}