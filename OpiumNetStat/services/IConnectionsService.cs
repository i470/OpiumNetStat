using OpiumNetStat.Model;
using System.Collections.Generic;
using System.Net;

namespace OpiumNetStat.services
{
    public interface IConnectionsService
    {
        Dictionary<IPAddress, ProcessIPInfo> NetStatRegistry { get; set; } 
        void StartWork();
        void DoWork();
    }
}