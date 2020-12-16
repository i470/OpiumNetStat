using Newtonsoft.Json;
using OpiumNetStat.model;
using OpiumNetStat.services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Http;
using Titanium.Web.Proxy.Models;

namespace OpiumNetStat.ViewModels
{
    public class TrafficViewModel: BindableBase
    {
        IEventAggregator _ea;
        IConnectionsService _cs;

        ExplicitProxyEndPoint explicitEndPoint;

        private readonly ProxyServer proxyServer;

        public DelegateCommand ProxyTrafficCommand { get; private set; }

        private bool _isProxyOn;
        public bool IsProxyOn
        {

            get => _isProxyOn;
            set { SetProperty(ref _isProxyOn, value); }
        }


        public TrafficViewModel(IEventAggregator ea)
        {
            _ea = ea;
            ProxyTrafficCommand = new DelegateCommand(TurnOnProxy, CanTurnOnProxy);
            IsProxyOn = false;

            proxyServer = new ProxyServer();
            proxyServer.ForwardToUpstreamGateway = true;

           
       

            proxyServer.BeforeRequest += BeforeRequest;
            proxyServer.BeforeResponse += BeforeResponse;
            

        }

      

        private readonly Dictionary<string, SessionListItem> sessionDictionary =
           new Dictionary<string, SessionListItem>();

        public ObservableCollectionEx<SessionListItem> Sessions { get; } = new ObservableCollectionEx<SessionListItem>();

        private SessionListItem selectedSession;
        public SessionListItem SelectedSession
        {
            get => selectedSession;
            set
            {
                if (value != selectedSession)
                {
                    selectedSession = value;
                    selectedSessionChanged();
                }
            }
        }

        private async Task TunnelBeforeResponse(object sender, TunnelConnectSessionEventArgs e)
        {
            if(e!=null && e.HttpClient!=null && e.HttpClient.Request!=null && e.HttpClient.Request.Host!=null)
            {
                var host = e.HttpClient.Request.Host.Split(':').FirstOrDefault();
                
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                        if (sessionDictionary.TryGetValue(host, out var item))
                        {
                            item.Update(e);
                        }

                });

            }
          

           
        }

        private async Task BeforeTunnelConnectRequest(object sender, TunnelConnectSessionEventArgs e)
        {
               await Application.Current.Dispatcher.InvokeAsync(() =>
               { 
                   addSession(e); 
               });
        }



        private SessionListItem addSession(SessionEventArgsBase e)
        {
            var item = createSessionListItem(e);
           
            if(!sessionDictionary.ContainsKey(item.Host))
            {
                sessionDictionary.Add(item.Host, item);

            }else
            {
                
                if(!string.IsNullOrEmpty(item.Url))
                {
                    sessionDictionary[item.Host].Url = item.Url;
                }

            }

            if (string.IsNullOrEmpty(sessionDictionary[item.Host].RemoteIP))
            {
                var ips = Dns.GetHostAddresses(item.Host);

                sessionDictionary[item.Host].RemoteIP = ips[0].ToString();

                //Application.Current.Dispatcher.InvokeAsync(async () =>
                //{

                //    var ipinfo = await GetIpInfo(item.Host);

                //    if (ipinfo != null)
                //    {
                //        sessionDictionary[item.Host].RemoteIP = ipinfo.Query;
                //        sessionDictionary[item.Host].City = ipinfo.City;
                //        sessionDictionary[item.Host].Country = ipinfo.CountryCode;
                //        sessionDictionary[item.Host].State = ipinfo.Region;
                //    }
                //});


            }


            if (Sessions.Any(x => x.Host.Equals(item.Host)))
            {
                var session = Sessions.Where(x => x.Host.Equals(item.Host)).FirstOrDefault();
                var index = Sessions.IndexOf(session);
                Sessions.RemoveAt(index);
                Sessions.Insert(index, sessionDictionary[item.Host]);

            }
            else
            {
                Sessions.Insert(0, sessionDictionary[item.Host]);
            }


            return item;
        }

        private SessionListItem createSessionListItem(SessionEventArgsBase e)
        {
            bool isTunnelConnect = e is TunnelConnectSessionEventArgs;


            var item = new SessionListItem
            {              
                HttpClient = e.HttpClient,
                IsTunnelConnect = isTunnelConnect
            };

            e.DataReceived += (sender, args) =>
            {
                var session = (SessionEventArgsBase)sender;

                if(!string.IsNullOrEmpty(session.HttpClient.Request.Host))
                {
                    if (sessionDictionary.TryGetValue(session.HttpClient.Request.Host, out var li))
                    {
                        var connectRequest = session.HttpClient.ConnectRequest;
                        var tunnelType = connectRequest?.TunnelType ?? TunnelType.Unknown;
                        if (tunnelType != TunnelType.Unknown)
                        {
                            li.Protocol = TunnelTypeToString(tunnelType);
                        }

                        li.ReceivedDataCount += args.Count;

                    }
                }

               
            };

            e.DataSent += (sender, args) =>
            {
                var session = (SessionEventArgsBase)sender;
                var host = session.HttpClient.Request.Host;

                if(!string.IsNullOrEmpty(host))
                {
                    if (sessionDictionary.TryGetValue(host, out var li))
                    {
                        var connectRequest = session.HttpClient.ConnectRequest;


                        var tunnelType = connectRequest?.TunnelType ?? TunnelType.Unknown;
                        if (tunnelType != TunnelType.Unknown)
                        {
                            li.Protocol = TunnelTypeToString(tunnelType);
                        }

                        li.SentDataCount += args.Count;

                    }
                }
               
            };

            if (e is TunnelConnectSessionEventArgs te)
            {
                te.DecryptedDataReceived += (sender, args) =>
                {
                    var session = (SessionEventArgsBase)sender;
                   
                };

                te.DecryptedDataSent += (sender, args) =>
                {
                    var session = (SessionEventArgsBase)sender;
                   
                };
            }

            item.Update(e);
            return item;
        }

        private async Task<IpInfo> GetIpInfo(string host)
        {
            using (var client = new WebClient())
            {
                try
                {
                    var uri = new Uri($"http://ip-api.com/json/{host}");

                    var json = await client.DownloadStringTaskAsync(uri);
                    return JsonConvert.DeserializeObject<IpInfo>(json);

                }
                catch (Exception ex)
                {
                    Debug.Write(ex.Message);
                    return null;
                }
            }
        }

        private string TunnelTypeToString(TunnelType tunnelType)
        {
            switch (tunnelType)
            {
                case TunnelType.Https:
                    return "https";
                case TunnelType.Websocket:
                    return "websocket";
                case TunnelType.Http2:
                    return "http2";
            }

            return null;
        }

        

        private async Task BeforeResponse(object sender, SessionEventArgs e)
        {
            SessionListItem item = null;
            IpInfo ipinfo = null;

            if (e != null && e.HttpClient != null && e.HttpClient.Request != null && e.HttpClient.Request.Host != null)
            {
                var host = e.HttpClient.Request.Host.Split(':').FirstOrDefault();

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    if (sessionDictionary.TryGetValue(host, out var item))
                    {
                        item.Update(e);
                    }

                });

            }


            if (item != null)
            {
                await Application.Current.Dispatcher.InvokeAsync(() => 
                 { 
                        item.Update(e); 
                 });
                
            }
        }

        private void selectedSessionChanged()
        {
            //throw new NotImplementedException();
        }

        private async Task BeforeRequest(object sender, SessionEventArgs e)
        {
            SessionListItem item = null;

            await Application.Current.Dispatcher.InvokeAsync(() => 
            { 
                item = addSession(e); 
            });

        }

        private bool CanTurnOnProxy()
        {
            return true;
        }

        private void TurnOnProxy()
        {
           if(!IsProxyOn)
            {
                explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, 8333, true);
                proxyServer.AddEndPoint(explicitEndPoint);

                explicitEndPoint.BeforeTunnelConnectRequest += BeforeTunnelConnectRequest;
                explicitEndPoint.BeforeTunnelConnectResponse += TunnelBeforeResponse;

                proxyServer.Start();
                proxyServer.SetAsSystemProxy(explicitEndPoint, ProxyProtocolType.AllHttp);
               
                IsProxyOn = true;
            }
            else
            {
                proxyServer.Stop();

                explicitEndPoint.BeforeTunnelConnectRequest -= BeforeTunnelConnectRequest;
                explicitEndPoint.BeforeTunnelConnectResponse -= TunnelBeforeResponse;


                proxyServer.RestoreOriginalProxySettings();
                IsProxyOn = false;
            }
        }
    }
}
