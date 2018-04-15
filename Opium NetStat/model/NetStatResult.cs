namespace Opium_NetStat.model
{
    public class NetStatResult : BaseNotify
    {
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

    }
}

