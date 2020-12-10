using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace OpiumNetStat.model
{
    public class NetStatResult : BaseNotify, IEquatable<NetStatResult>, IComparable<NetStatResult>
    {
        public NetStatResult()
        {
            LastSeen = DateTime.Now;
        }
        public NetStatResult(TcpConnectionInformation tcp)
        {
            LocalIP = tcp.LocalEndPoint.Address.ToString();
            RemoteIP = tcp.RemoteEndPoint.Address.ToString();
            ConnectionStatus = tcp.State.ToString();
            PortNumber = (short)tcp.RemoteEndPoint.Port;
            LastSeen = DateTime.Now;
        }

        private string localIP;
        public string LocalIP
        {
            get => localIP;
            set
            {
                localIP = value;
                RaisePropertyChanged(() => LocalIP);
            }
        }

        private string _remoteIP;
        public string RemoteIP
        {
            get => _remoteIP;
            set
            {
                _remoteIP = value;
                RaisePropertyChanged(() => RemoteIP);
            }
        }

        private string origin;
        public string Origin
        {
            get => origin;
            set
            {
                origin = value;
                RaisePropertyChanged(() => Origin);
            }
        }

        private short portNumber;
        public short PortNumber
        {
            get => portNumber;
            set
            {
                portNumber = value;
                RaisePropertyChanged(() => PortNumber);
            }
        }
        private string portNormalyUsedBy;
        public string PortNormalyUsedBy
        {
            get => portNormalyUsedBy;
            set
            {
                portNormalyUsedBy = value;
                RaisePropertyChanged(() => PortNormalyUsedBy);
            }
        }

        private string _portOfficial;
        public string PortOfficial
        {
            get => _portOfficial;
            set
            {
                _portOfficial = value;
                RaisePropertyChanged(() => PortOfficial);
            }
        }
        private string software;
        public string Software
        {
            get => software;
            set
            {
                software = value;
                RaisePropertyChanged(() => Software);
            }
        }

        private short pid;
        public short PID
        {
            get => pid;
            set
            {
                pid = value;
                RaisePropertyChanged(() => PID);
            }
        }


        private string connectionStatus;
        public string ConnectionStatus
        {
            get => connectionStatus;
            set
            {
                connectionStatus = value;
                RaisePropertyChanged(() => ConnectionStatus);
            }
        }


        public string Status { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Org { get; set; }
        public string Host { get; set; }
        public DateTime LastSeen { get; set; }



        public override int GetHashCode()
        {
            return RemoteIP.GetHashCode() ^ LastSeen.GetHashCode() ^ ConnectionStatus.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as NetStatResult);
        }


        public bool Equals(NetStatResult other)
        {
            if (ReferenceEquals(other, null))
                return false;

            var equal = this.RemoteIP == other.RemoteIP
                && this.connectionStatus == other.connectionStatus
                && this.LastSeen == other.LastSeen
                && this.PID == other.PID
                && this.CountryCode == other.CountryCode;

            return equal;

        }

        // Result:
        //  < 0 : this instance older than other
        //  = 0 : this instance equivalent to other
        //  > 0 : this instance newer than other
        //  -2 if not comparable

        public int CompareTo(NetStatResult other)
        {
            if (ReferenceEquals(other, null))
                return 1;

            if (this.Equals(other))
                return 0;

           
            var isOlder = this.LastSeen > other.LastSeen;
            var hasNewState = this.ConnectionStatus != other.ConnectionStatus;
              
            if((this.RemoteIP == other.RemoteIP) && isOlder && !hasNewState)
            {
                return -1;
            }

            return -2;
        }

     
    }


    public class NetStatResultComparer: IEqualityComparer<NetStatResult>
    {
        public bool Equals(NetStatResult x, NetStatResult y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(NetStatResult net)
        {
            return net.GetHashCode();
        }
    }
}

