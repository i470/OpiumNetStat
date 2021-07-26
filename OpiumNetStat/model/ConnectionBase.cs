using OpiumNetStat.Model;
using OpiumNetStat.services;
using Prism.Mvvm;
using System;
using System.Windows;

namespace OpiumNetStat.model
{
    public abstract class ConnectionBase : BindableBase, IEquatable<ConnectionBase>
    {
        private string remoteip;
        private ProcessIPInfo processInfo;
        private IpInfo ipInfo;
        private string host;
        

        public string RemoteIp
        {
            get => remoteip;
            set
            {
                if (value != remoteip)
                {
                    SetProperty(ref remoteip, value);
                    
                }

            }
        }

  

        public ProcessIPInfo ProcessInfo
        {
            get => processInfo;
            set => SetProperty(ref processInfo, value);
        }

        public IpInfo IpInfo
        {
            get => ipInfo;
            set => SetProperty(ref ipInfo, value);
        }


        public string Host
        {
            get => host;
            set => SetProperty(ref host, value);
        }


        public override int GetHashCode()
        {
            if (RemoteIp != null)
                return RemoteIp.GetHashCode();
            return -1;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ConnectionBase);
        }

        public virtual bool Equals(ConnectionBase other)
        {
            if (ReferenceEquals(other, null))
                return false;

            return RemoteIp == other.RemoteIp;
        }


    }
}
