using OpiumNetStat.Model;
using OpiumNetStat.services;
using System.Linq;
using System.Net;
using System.Windows.Media.Imaging;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Http;

namespace OpiumNetStat.model
{
    public class SessionListItem : ConnectionBase, IConnection
    {
        
        private long receivedDataCount;
        private long sentDataCount;
        private string statusCode;
        private string url;



        public HttpWebClient HttpClient { get; set; }
        public bool IsTunnelConnect { get; set; }



        public string Url
        {
            get => url;
            set => SetProperty(ref url, value);

        }


        public string StatusCode
        {
            get => statusCode;
            set => SetProperty(ref statusCode, value);
        }



        public long ReceivedDataCount
        {
            get => receivedDataCount;
            set => SetProperty(ref receivedDataCount, value);
        }

        public long SentDataCount
        {
            get => sentDataCount;
            set => SetProperty(ref sentDataCount, value);
        }


        private BitmapImage _icon;
        public BitmapImage ProcIcon
        {
            get => _icon;
            set => SetProperty(ref _icon, value);
        }




        public void Update(SessionEventArgsBase args)
        {

            var request = HttpClient.Request;
            var response = HttpClient.Response;
            int statusCode = response?.StatusCode ?? 0;
            StatusCode = statusCode == 0 ? "-" : statusCode.ToString();

            Host = request.RequestUri.Host;

            if (string.IsNullOrEmpty(RemoteIp))
            {
                try
                {
                    RemoteIp = Dns.GetHostAddresses(Host)[0].ToString();
                }
                catch
                {
                    //brave creates bs hosts
                }

            }

            if (!IsTunnelConnect)
            {

                Url = request.RequestUri.AbsoluteUri;
            }else
            {
                Url = request.Url;
            }

            if (ProcessInfo is null || ProcessInfo.ProcessName.Equals("-") || ProcessInfo.ProcessName.Equals("Idle"))
            {

                ProcessInfo = new ProcessIPInfo(HttpClient.ProcessId.Value)
                {

                    Protocol = request.RequestUri.Scheme,
                    Port = 80
                };

                int port = 80;

                if (tryGetPort(args, out port))
                {
                    ProcessInfo.Port = port;
                }
            }


        }

        private bool tryGetPort(SessionEventArgsBase args, out int port)
        {
            try
            {
                if (args.HttpClient.ConnectRequest != null)
                {
                    port = int.Parse(args.HttpClient.ConnectRequest.RequestUriString.Split(':').Last());
                    return true;
                }
                else
                {
                    port = -1;
                    return false;
                }

            }
            catch
            {
                port = -1;
                return false;
            }
        }

        internal void Update(IpInfo ipi)
        {
            IpInfo = ipi;
        }
    }
}
