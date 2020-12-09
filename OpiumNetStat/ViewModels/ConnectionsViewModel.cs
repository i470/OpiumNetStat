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
using System.Threading.Tasks;

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

            Task.Run(() =>
            {
                _cs.StartWork();

            }).ConfigureAwait(false);

                
          
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

        private void UpdateConnections(List<NetStatResult> result)
        {

            if (result is null) return;

            foreach(var r in result)
            {
                var record = NetStat.Where(x => x.RemoteIP.Equals(r.RemoteIP)).FirstOrDefault();

                if (record is null)
                {
                    NetStat.Add(r);

                }
                else
                {
                    if (record.ConnectionStatus != r.ConnectionStatus)
                    {
                        record.ConnectionStatus = r.ConnectionStatus;
                    }

                    if (record.LastSeen != r.LastSeen)
                    {
                        record.LastSeen = r.LastSeen;
                    }

                }
            }

            foreach (var net in NetStat)
            {
                var existing = result.Where(x => x.RemoteIP.Equals(net.RemoteIP)).FirstOrDefault();

                if (existing is null)
                {
                   // net.ConnectionStatus = "Closed";

                }
                else
                {
                    net.ConnectionStatus = existing.ConnectionStatus;
                    net.LastSeen = existing.LastSeen;
                }
            }

            NetStat = new ObservableCollection<NetStatResult>(NetStat.OrderByDescending(x => x.LastSeen).ToList());

            //var orderedList = tmpList.OrderByDescending(x => x.LastSeen).ToList();
            // NetStat.OrderByDescending(x => x.LastSeen).ToList();
            // NetStat = new ObservableCollection<NetStatResult>(orderedList);
            
            IsBusy = false;
          
          
        }
    }
}
