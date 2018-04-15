using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        List<PortInfo> KnownPorts;

        public NetStatViewModel NetStatVM { get; set; }

        private TcpGlobalParametersViewModel tcpGlobalParametersViewModel;

        public TcpGlobalParametersViewModel TcpGlobalParametersViewModel
        {

            get => tcpGlobalParametersViewModel;
            set
            {
                tcpGlobalParametersViewModel = value;
                RaisePropertyChanged(() => TcpGlobalParametersViewModel);
            }
        }

        public MainViewModel()
        {
          

            
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException(), @"assets\know-ports.json");
            
            // deserialize JSON directly from a file
            using (var file = File.OpenText(path))
            {
                var serializer = new JsonSerializer();
                KnownPorts = (List<PortInfo>) serializer.Deserialize(file,typeof(List<PortInfo>));
            }

            NetStatsNetStatResults = GetIpConnections();
            NetStatVM=new NetStatViewModel();
            TcpGlobalParametersViewModel = new TcpGlobalParametersViewModel();
        }


        public ObservableCollection<NetStatResult> GetIpConnections()
        {
            netStatResults = new ObservableCollection<NetStatResult>();

            var ip = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();

            foreach (var tcp in ip.GetActiveTcpConnections())
            {
                var result = new NetStatResult
                {
                    LocalIP = tcp.LocalEndPoint.Address.ToString(),
                    RemoteIP = tcp.RemoteEndPoint.Address.ToString(),
                    ConnectionStatus = tcp.State.ToString(),
                    PortNumber = (short) tcp.RemoteEndPoint.Port
                };

                result.PortNormalyUsedBy = getPortDetails(result.PortNumber).Item1;
                result.PortOfficial= getPortDetails(result.PortNumber).Item2;

                result.Origin = getIpOrigin(result.RemoteIP);
                netStatResults.Add(result);
            }

            return netStatResults;
        }

        private Tuple<string,string> getPortDetails(short p)
        {
            if (KnownPorts.Count <= 0) return new Tuple<string, string>(string.Empty,string.Empty);

            foreach (var port in KnownPorts)
            {
                if (p.ToString().Trim().Equals(port.PortStart.Trim()))
                {
                    return new Tuple<string, string>(
                        port.Desciption,
                        port.Status);
                    
                }
            }

            return new Tuple<string, string>(string.Empty, string.Empty);
        }


        public string getIpOrigin(string ip)
        {
            if (ip.Equals("127.0.0.1"))
                return "your machine";

            string origin="";

            using (var client = new WebClient())
            {
                var json = client.DownloadString("http://ip-api.com/json/"+ip);
                var result = JsonConvert.DeserializeObject<Origin>(json);
                origin = result.city + ", " + result.region + "," + result.country + " - " + result.org;
            }

            return origin;
        }

    }



}






