using LiteDB;
using OpiumNetStat.events;
using OpiumNetStat.model;
using OpiumNetStat.services;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace OpiumNetStat.ViewModels
{
    public class ConnectionsViewModel:ViewModelBase
    {
        IEventAggregator _ea;
        IConnectionsService _cs;

        private ObservableCollection<NetStatResult> netStat;
        public ObservableCollection<NetStatResult> NetStat
        {

            get => netStat;
            set { SetProperty(ref netStat, value); }
        }


        public ConnectionsViewModel(IEventAggregator ea, IConnectionsService cs)
        {
            _cs = cs;
            _ea = ea;

            NetStat = new ObservableCollection<NetStatResult>();

            using (var db = new LiteDatabase(DB.Path))
            {
                var coll = db.GetCollection<NetStatResult>(DB.CollConnections);
                var last24Hours = DateTime.Now.AddHours(-24);

                var dayRecords = coll.FindAll().ToList();
                //.Where(x => x.LastSeen > last24Hours).OrderByDescending(x=>x.LastSeen).ToList();
                NetStat = new ObservableCollection<NetStatResult>(dayRecords);
            }

             _ea.GetEvent<ConnectionUpdateEvent>().Subscribe(UpdateConnections, ThreadOption.UIThread);
             _cs.StartWork();
          
        }

     
        private void UpdateConnections(NetStatResult result)
        {
            var tmpList = NetStat;

                     
            var record = tmpList.Where(x => x.RemoteIP.Equals(result.RemoteIP)).FirstOrDefault();

            if (record is null)
            {
                tmpList.Add(result);

            }else
            {
                record = result;
            }

            var orderedList = tmpList.OrderByDescending(x => x.LastSeen).ToList();
            NetStat.Clear();
            NetStat = new ObservableCollection<NetStatResult>(orderedList);  
        }
    }
}
