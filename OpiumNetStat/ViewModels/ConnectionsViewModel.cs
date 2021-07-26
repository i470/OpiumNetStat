using LiteDB;
using OpiumNetStat.events;
using OpiumNetStat.model;
using OpiumNetStat.services;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpiumNetStat.utils;
using System.Windows;
using NetFwTypeLib;

namespace OpiumNetStat.ViewModels
{
    public class ConnectionsViewModel: BindableBase
    {
        IEventAggregator _ea;
        IConnectionsService _cs;
        IDataPipeLineService _dps;
        

        private ObservableCollection<NetStatItemViewModel> netStat;
        public ObservableCollection<NetStatItemViewModel> NetStat
        {

            get => netStat;
            set { SetProperty(ref netStat, value); }
        }

        private NetStatItemViewModel _selectedNetStat;
        public  NetStatItemViewModel SelectedNetStat
        {

            get => _selectedNetStat;
            set { SetProperty(ref _selectedNetStat, value); }
        }

        public ConnectionsViewModel(IEventAggregator ea, IConnectionsService cs, IDataPipeLineService dps)
        {
            isBusy = false;

            _cs = cs;
            _ea = ea;
            _dps = dps;

            NetStat = new ObservableCollection<NetStatItemViewModel>();

            _ea.GetEvent<ConnectionUpdateEvent>().Subscribe(UpdateConnections, ThreadOption.UIThread);

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

       

            Task task = PeriodicTaskFactory.Start(() =>
            {
                _cs.DoWork();

               

            }, intervalInMilliseconds: 3000, synchronous: true, cancelToken: cancellationTokenSource.Token);

         

        }

        private void UpdateConnections(NetStatItemViewModel result)
        {
          

            if (NetStat.Any(x=>x.RemoteIp.Equals(result.RemoteIp)))
            {
                var net = netStat.Where(x => x.RemoteIp.Equals(result.RemoteIp)).FirstOrDefault();
                var index = netStat.IndexOf(net);
                netStat.RemoveAt(index);
                netStat.Insert(index, result);
                NetStat = netStat;

            }
            else
            {
                NetStat.Insert(0, result);
            }

            if (isBusy)
                IsBusy = false;
        }

        private bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            set
            {
                if (IsBusy!=value)
                {
                    SetProperty(ref isBusy, value);
                    _ea.GetEvent<IsBusyEvent>().Publish(IsBusy);
                }
                    
            }

        }


        private void BlockIP(string ip)
        {
            INetFwRule firewallRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
            firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
            firewallRule.Description = "Your rule description";
            firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN; // inbound
            firewallRule.Enabled = true;
            firewallRule.InterfaceTypes = "All";
            firewallRule.RemoteAddresses = "1.2.3.0/24"; // add more blocks comma separated
            firewallRule.Name = "You rule name";
           
        }

        
    }
}
