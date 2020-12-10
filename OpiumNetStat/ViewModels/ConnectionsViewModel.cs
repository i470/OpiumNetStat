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
        

        private ObservableCollection<NetStatResult> netStat;
        public ObservableCollection<NetStatResult> NetStat
        {

            get => netStat;
            set { SetProperty(ref netStat, value); }
        }


        public ConnectionsViewModel(IEventAggregator ea, IConnectionsService cs, IDataPipeLineService dps)
        {
            isBusy = true;

            _cs = cs;
            _ea = ea;
            _dps = dps;

            NetStat = new ObservableCollection<NetStatResult>();

            _ea.GetEvent<ConnectionUpdateEvent>().Subscribe(UpdateConnections, ThreadOption.UIThread);

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            Task task = PeriodicTaskFactory.Start(() =>
            {
                _cs.DoWork();

            }, intervalInMilliseconds: 5000, synchronous: true, cancelToken: cancellationTokenSource.Token);

           

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
            firewallPolicy.Rules.Add(firewallRule);
        }

        private void UpdateConnections(List<NetStatResult> result)
        {
            if (result is null) return;
            if (result.Count == 0) return;

            var hashset = new HashSet<NetStatResult>(NetStat.ToList(),new NetStatResultComparer());
            hashset.SymmetricExceptWith(result);
            var merged = hashset.OrderByDescending(x => x.LastSeen).ToList();

            NetStat.Clear();
            NetStat.AddRange(merged);


            if (isBusy)
                IsBusy = false;

        }
    }
}
