using OpiumNetStat.Model;

namespace OpiumNetStat.model
{
    public interface IConnection
    {
        string Host { get; set; }
        IpInfo IpInfo { get; set; }
        ProcessIPInfo ProcessInfo { get; set; }
        string RemoteIp { get; set; }
        bool Equals(object obj);
        int GetHashCode();
    }
}