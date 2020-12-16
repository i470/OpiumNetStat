using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Http;

namespace OpiumNetStat.model
{
    public class SessionListItem : INotifyPropertyChanged
    {
        private long? bodySize;
        private Exception exception;
        private string host;
        private int processId;
        private string protocol;
        private long receivedDataCount;
        private long sentDataCount;
        private string statusCode;
        private string url;
        private Guid clientConnectionId;
        private Guid serverConnectionId;
        private string org;
        private string city;
        private string country;
        private string state;



        public int Number { get; set; }

        public Guid ClientConnectionId
        {
            get => clientConnectionId;
            set => SetField(ref clientConnectionId, value);
        }

        public Guid ServerConnectionId
        {
            get => serverConnectionId;
            set => SetField(ref serverConnectionId, value);
        }

        public HttpWebClient HttpClient { get; set; }

        public IPEndPoint ClientLocalEndPoint { get; set; }

        public IPEndPoint ClientRemoteEndPoint { get; set; }

        public bool IsTunnelConnect { get; set; }

        public string StatusCode
        {
            get => statusCode;
            set => SetField(ref statusCode, value);
        }

        public string Org
        {
            get => org;
            set => SetField(ref org, value);
        }
        public string Country
        {
            get => country;
            set => SetField(ref country, value);
        }

        public string State
        {
            get => state;
            set => SetField(ref state, value);
        }

        public string City
        {
            get => city;
            set => SetField(ref city, value);
        }

        public string Protocol
        {
            get => protocol;
            set => SetField(ref protocol, value);
        }

        public string Host
        {
            get => host;
            set => SetField(ref host, value);
         
        }

        private string ip;
        public string RemoteIP
        {
            get => ip;
            set => SetField(ref ip, value);
        }

        public string Url
        {
            get => url;
            set => SetField(ref url, value);
            
        }

        public long? BodySize
        {
            get => bodySize;
            set => SetField(ref bodySize, value);
        }

        public int ProcessId
        {
            get => processId;
            set
            {
                if (SetField(ref processId, value))
                {
                    OnPropertyChanged(nameof(Process));
                    if(ProcIcon is null)
                    {
                        SetIcon(processId);
                    }
                }
            }
        }

        private void SetIcon(int processId)
        {
            this.ProcIcon = GetProcessIcon(processId);
        }

        public string Process
        {
            get
            {
                try
                {
                    var process = System.Diagnostics.Process.GetProcessById(processId);
                    return process.ProcessName;
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

        public long ReceivedDataCount
        {
            get => receivedDataCount;
            set => SetField(ref receivedDataCount, value);
        }

        public long SentDataCount
        {
            get => sentDataCount;
            set => SetField(ref sentDataCount, value);
        }

        public Exception Exception
        {
            get => exception;
            set => SetField(ref exception, value);
        }

        private BitmapImage _icon;
        public BitmapImage ProcIcon
        {
            get => _icon;
            set => SetField(ref _icon, value);
        }

        public BitmapImage GetProcessIcon(int pid)
        {
            try
            {

                var proc = System.Diagnostics.Process.GetProcessById(pid);
                if (proc != null && proc.MainModule != null && proc.MainModule.FileName != null)
                {
                    Icon ico = Icon.ExtractAssociatedIcon(proc.MainModule.FileName);

                    Bitmap bitmap = ico.ToBitmap();
                    MemoryStream stream = new MemoryStream();

                    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    BitmapImage bmpImage = new BitmapImage();
                    bmpImage.BeginInit();
                    stream.Seek(0, SeekOrigin.Begin);
                    bmpImage.StreamSource = stream;
                    bmpImage.EndInit();
                    bmpImage.Freeze();

                    return bmpImage;
                }

                return null;
            }
            catch (Exception ex)
            {
                 return null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
                return true;
            }

            return false;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Update(SessionEventArgsBase args)
        {
            var request = HttpClient.Request;
            var response = HttpClient.Response;
            int statusCode = response?.StatusCode ?? 0;
            StatusCode = statusCode == 0 ? "-" : statusCode.ToString();

            Protocol = request.RequestUri.Scheme;
            Host = request.RequestUri.Host;

            if (!IsTunnelConnect)
            {

                Url = request.RequestUri.AbsoluteUri;
            }
           
            ProcessId = HttpClient.ProcessId.Value;
        }

        internal void Update(IpInfo ipinfo)
        {
            if (ipinfo is null) return;

            this.RemoteIP = ipinfo.Query;
            this.City = ipinfo.City;
            this.Country = ipinfo.CountryCode;
            this.State = ipinfo.Region;
        }
    }
}
