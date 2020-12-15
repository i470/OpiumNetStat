using OpiumNetStat.model;
using OpiumNetStat.Model;
using System.Threading.Tasks;

namespace OpiumNetStat.services
{
    public interface IIpInfoService
    {
        Task<IpInfo> GetIPInfo(ProcessIPInfo ip);
    }
}