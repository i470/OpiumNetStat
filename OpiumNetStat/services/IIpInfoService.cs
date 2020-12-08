using OpiumNetStat.model;
using OpiumNetStat.Model;
using System;
using System.Threading.Tasks;

namespace OpiumNetStat.services
{
    public interface IIpInfoService
    {
        Task<NetStatResult> GetIPInfo(PortInfo ip);
    }
}