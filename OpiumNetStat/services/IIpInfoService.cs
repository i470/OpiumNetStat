using OpiumNetStat.model;
using System;
using System.Threading.Tasks;

namespace OpiumNetStat.services
{
    public interface IIpInfoService
    {
        Task GetIPInfo(string ip, Action<Exception, IpInfo> callback);
        
    }
}