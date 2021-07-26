using Newtonsoft.Json;
using OpiumNetStat.events;
using OpiumNetStat.model;
using OpiumNetStat.Model;
using OpiumNetStat.services;
using OpiumNetStat.utils;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Concurrent;
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
    public class TrafficViewModel : BindableBase
    {
        IEventAggregator _ea;
        IConnectionsService _cs;
        IIpInfoService _iis;
        ExplicitProxyEndPoint explicitEndPoint;

        private readonly ProxyServer proxyServer;

        public DelegateCommand ProxyTrafficCommand { get; private set; }

        private bool _isProxyOn;
        public bool IsProxyOn
        {

            get => _isProxyOn;
            set { SetProperty(ref _isProxyOn, value); }
        }


        public TrafficViewModel(IEventAggregator ea, IIpInfoService iis)
        {
            _ea = ea;
            _iis = iis;

            ProxyTrafficCommand = new DelegateCommand(TurnOnProxy, CanTurnOnProxy);
            IsProxyOn = false;

            proxyServer = new ProxyServer
            {
                ForwardToUpstreamGateway = true
            };

        }



        private Dictionary<string, SessionListItem> sessionDictionary = new
            Dictionary<string, SessionListItem>();

        private Dictionary<string, IpInfo> ipDictionary = new
           Dictionary<string, IpInfo>();

        //private readonly Dictionary<string, SessionListItem> sessionDictionary =
        //   new Dictionary<string, SessionListItem>();

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
                    //selectedSessionChanged();
                }
            }
        }


        private async Task BeforeResponse(object sender, SessionEventArgsBase e)
        {

            if (e.HttpClient.Request.Host != null)
            {
                var host = e.HttpClient.Request.Host.Split(':').First();

                if (sessionDictionary.TryGetValue(host, out var item))
                {

                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {

                        item.Update(e);

                    });




                }
            }

        }


        //private async Task<SessionListItem> Prune(SessionListItem item)
        //{
        //    if(string.IsNullOrEmpty(item.RemoteIp))
        //    {
        //        item.RemoteIp =  Dns.GetHostAddresses(item.Host)[0].ToString();
        //    }

        //    if(item.IpInfo is null || string.IsNullOrEmpty(item.IpInfo.CountryCode))
        //    {
        //        item.IpInfo = await GetIpInfo(item.Host);
        //    }

        //    if (item.ProcessInfo is null || item.ProcessInfo.ProcessName.Equals("-") || item.ProcessInfo.ProcessName.Equals("Idle"))
        //    {
        //        var request = item.HttpClient.Request;

        //        item.ProcessInfo = new ProcessIPInfo(item.HttpClient.ProcessId.Value)
        //        {
        //            Protocol = request.RequestUri.Scheme,
        //            Port = 80
        //        };
        //    }

        //    return item;
        //}

        private async Task BeforeRequest(object sender, SessionEventArgsBase e)
        {

            await AddSession(e);
        }


        public async Task AddSession(SessionEventArgsBase e)
        {

            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                await addSessionAsync(e);
            });
        }

        private bool hasIpInfo(SessionEventArgsBase e)
        {
            var host = e.HttpClient.Request.Host;
            try
            {
                return sessionDictionary[host].IpInfo != null;
            }
            catch
            {
                return false;
            }
        }

        private async Task addSessionAsync(SessionEventArgsBase e)
        {
            var webConnection = createSessionListItem(e);


            if (e.HttpClient.Request.Host != null)
            {
                //get clean host name
                var host = e.HttpClient.Request.Host.Split(':').First();

                //check for sessions that doesn't have IP info
                //then add them to IP catalog and update IP info
                foreach (var em in Sessions.Where(x => x.IpInfo is null))
                {
                    if (!ipDictionary.ContainsKey(em.RemoteIp))
                    {
                        ipDictionary.Add(em.RemoteIp, await getIpInfo(em.RemoteIp));

                    }

                }

                //if IP is already in the IP catalog, update it info with new
                //if not get IP info and add it to the catalog
                if (ipDictionary.TryGetValue(webConnection.RemoteIp, out var ipInfo))
                {
                    webConnection.IpInfo = ipInfo;

                }
                else
                {
                    ipInfo = await getIpInfo(webConnection.RemoteIp);
                    ipDictionary.Add(webConnection.RemoteIp, ipInfo);
                    webConnection.IpInfo = ipInfo;
                }

                
                //Update session catalog with new host
                if (!sessionDictionary.ContainsKey(host))
                {
                    sessionDictionary.Add(webConnection.Host, webConnection);
                    Sessions.Insert(0, sessionDictionary[webConnection.Host]);
                }
                
                //update existing host entry with new info
                else
                {

                    if (!string.IsNullOrEmpty(webConnection.Url))
                    {
                        sessionDictionary[webConnection.Host].Url = webConnection.Url;
                    }

                    var session = Sessions.FirstOrDefault(x => x.Host.Equals(webConnection.Host));
                    if (session != null)
                    {

                        var index = Sessions.IndexOf(session);
                        Sessions.RemoveAt(index);
                        Sessions.Insert(index, sessionDictionary[webConnection.Host]);

                    }

                }
            }
        }

        /// <summary>
        /// Generates session item object
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private SessionListItem createSessionListItem(SessionEventArgsBase e)
        {
            bool isTunnelConnect = e is TunnelConnectSessionEventArgs;

            var sessionInfo = new SessionListItem
            {
                HttpClient = e.HttpClient,
                IsTunnelConnect = isTunnelConnect,
                Host = e.HttpClient.Request.Host
            };

            sessionInfo.Update(e);

            return sessionInfo;
        }


        private async Task<IpInfo> getIpInfo(string ip)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    var json = await client.DownloadStringTaskAsync($"http://ip-api.com/json/{ip}");
                    return JsonConvert.DeserializeObject<IpInfo>(json);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
                return null;
            }

        }



        private string TunnelTypeToPort(TunnelType tunnelType)
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
        private bool CanTurnOnProxy()
        {
            return true;
        }

        private void TurnOnProxy()
        {
            if (!IsProxyOn)
            {
                explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, 8333, true);
                proxyServer.AddEndPoint(explicitEndPoint);

                explicitEndPoint.BeforeTunnelConnectRequest += BeforeResponse; ;
                explicitEndPoint.BeforeTunnelConnectResponse += BeforeResponse;
                proxyServer.BeforeRequest += BeforeRequest;
                proxyServer.BeforeResponse += BeforeResponse;

                proxyServer.Start();
                proxyServer.SetAsSystemProxy(explicitEndPoint, ProxyProtocolType.AllHttp);

                IsProxyOn = true;
            }
            else
            {
                proxyServer.Stop();

                explicitEndPoint.BeforeTunnelConnectRequest -= BeforeRequest;
                explicitEndPoint.BeforeTunnelConnectResponse -= BeforeResponse;
                proxyServer.BeforeRequest -= BeforeRequest;
                proxyServer.BeforeResponse -= BeforeResponse;

                proxyServer.RestoreOriginalProxySettings();
                IsProxyOn = false;
            }
        }


    }
}
