using System.Collections.ObjectModel;
using Opium_NetStat.model;
using Opium_NetStat.utils;

namespace Opium_NetStat.viewmodel
{
    public class NetStatViewModel:BaseNotify
    {

        private ObservableCollection<ProcessInformation.Port> netStatPorts;
        public ObservableCollection<ProcessInformation.Port> NetStatPorts
        {

            get => netStatPorts;
            set
            {
                netStatPorts = value;
                RaisePropertyChanged(() => NetStatPorts);
            }
        }

        public NetStatViewModel()
        {
            NetStatPorts=new ObservableCollection<ProcessInformation.Port>();
            NetStatPorts = ProcessInformation.GetNetStatPorts();
        }
    }
}
