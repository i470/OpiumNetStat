using OpiumNetStat.model;
using System.Collections.Generic;

namespace OpiumNetStat.services
{
    public interface IDataBaseService
    {
    
        NetStatItemViewModel GetNetStatRecord(string ip);
        bool RemoteIpExists(string ip);
        NetStatItemViewModel Upsert(NetStatItemViewModel netStat);
    }
}