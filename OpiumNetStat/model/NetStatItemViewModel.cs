using OpiumNetStat.Model;
using OpiumNetStat.services;
using System.Collections.Generic;

namespace OpiumNetStat.model
{
    public class NetStatItemViewModel : ConnectionBase, IConnection
    {

        public NetStatItemViewModel(ProcessIPInfo proc)
        {
            ProcessInfo = proc;
            RemoteIp = proc.RemoteIp;
        }

      
        public  bool Equals(NetStatItemViewModel other)
        {
            if (ReferenceEquals(other, null))
                return false;

            return RemoteIp == other.RemoteIp;

        }

    }


    public class NetStatResultComparer: IEqualityComparer<NetStatItemViewModel>
    {
        public bool Equals(NetStatItemViewModel x, NetStatItemViewModel y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(NetStatItemViewModel net)
        {
            return net.GetHashCode();
        }
    }
}

