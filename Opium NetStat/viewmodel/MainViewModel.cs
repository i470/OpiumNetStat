using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opium_NetStat.model;

namespace Opium_NetStat.viewmodel
{
    public class MainViewModel:BaseNotify
    {
        private ObservableCollection<NetStatResult> netStatResults;
        public ObservableCollection<NetStatResult> NetStatsNetStatResults
        {

            get => netStatResults;
            set
            {
                netStatResults = value;
                RaisePropertyChanged(() => NetStatsNetStatResults);
            }
        }



        public MainViewModel()
        {
            NetStatsNetStatResults = getIPConnections();
        }


        public ObservableCollection<NetStatResult> getIPConnections()
        {
            netStatResults = new ObservableCollection<NetStatResult>();

            var ip = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();

            foreach (var tcp in ip.GetActiveTcpConnections())
            {
                var result = new NetStatResult();
                result.LocalIP = tcp.LocalEndPoint.Address.ToString();
                result.RemoteIP = tcp.RemoteEndPoint.Address.ToString();
                result.ConnectionStatus = tcp.State.ToString();
                result.PortNumber =(short) tcp.RemoteEndPoint.Port;
                result.PortNormalyUsedBy = "http";
                result.Software = "chrome";
                netStatResults.Add(result);
            }

            return netStatResults;
        }

      
    }
}




