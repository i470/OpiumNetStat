﻿using LiteDB;
using Newtonsoft.Json;
using OpiumNetStat.events;
using OpiumNetStat.model;
using OpiumNetStat.services;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows.Input;

namespace OpiumNetStat.ViewModels
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string[] Phones { get; set; }
        public bool IsActive { get; set; }
    }


    public class MainWindowViewModel : BindableBase
    {

        IEventAggregator _ea;
        IConnectionsService _cs;

      

        private bool isCommandToHide;
        public bool IsCommandToHide
        {
            get => isCommandToHide;
           
        }

        List<PortInfo> KnownPorts;

        public ICommand HideHTTPCommand { get; set; }
        public ICommand RefreshCommand { get; set; }

        public NetStatViewModel NetStatVM { get; set; }

        private TcpGlobalParametersViewModel tcpGlobalParametersViewModel;

        public TcpGlobalParametersViewModel TcpGlobalParametersViewModel
        {

            get => tcpGlobalParametersViewModel;
            set
            {
                tcpGlobalParametersViewModel = value;
               
            }
        }

       

        public MainWindowViewModel(IEventAggregator ea, IConnectionsService cs)
        {


            NetStatService.GetNetStatPorts();


            //string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException(), @"assets\know-ports.json");

            //// deserialize JSON directly from a file
            //using (var file = File.OpenText(path))
            //{
            //    var serializer = new Newtonsoft.Json.JsonSerializer();
            //    KnownPorts = (List<PortInfo>)serializer.Deserialize(file, typeof(List<PortInfo>));
            //}

            //NetStatsNetStatResults = new ObservableCollection<NetStatResult>(); //GetIpConnections();
            //NetStatVM = new NetStatViewModel();
            //TcpGlobalParametersViewModel = new TcpGlobalParametersViewModel();

            //HideHTTPCommand = new ActionCommand<object>(HideHTTP);
            //RefreshCommand = new ActionCommand<object>(Refresh);

        }

      

        //private void Refresh(object obj)
        //{
        //    HideHTTP(obj);
        //}



        //public ObservableCollection<NetStatResult> GetIpConnections()
        //{
        //    //netStatResults = new ObservableCollection<NetStatResult>();

        //    //var ip = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();

            

            

        //    //foreach (var tcp in ip.GetActiveTcpConnections())
        //    //{
        //    //    var result = new NetStatResult
        //    //    {
        //    //        LocalIP = tcp.LocalEndPoint.Address.ToString(),
        //    //        RemoteIP = tcp.RemoteEndPoint.Address.ToString(),
        //    //        ConnectionStatus = tcp.State.ToString(),
        //    //        PortNumber = (short)tcp.RemoteEndPoint.Port
        //    //    };

        //    //    result.PortNormalyUsedBy = getPortDetails(result.PortNumber).Item1;
        //    //    result.PortOfficial = getPortDetails(result.PortNumber).Item2;

        //    //    result.Origin = getIpOrigin(result.RemoteIP);
        //    //    netStatResults.Add(result);
        //    //}


            
        //    //return netStatResults;
        //}


        private Tuple<string, string> getPortDetails(short p)
        {
            if (KnownPorts.Count <= 0) return new Tuple<string, string>(string.Empty, string.Empty);

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

            string origin = "";

            using (var client = new WebClient())
            {
                try
                {
                    var json = client.DownloadString("http://ip-api.com/json/" + ip);
                    var result = JsonConvert.DeserializeObject<Host>(json);
                    origin = result.City + ", " + result.Region + "," + result.Country + " - " + result.Org;
                }
                catch (Exception)
                {
                    Thread.Sleep(100);
                 
                }
            }

            return origin;
        }

    }



}






