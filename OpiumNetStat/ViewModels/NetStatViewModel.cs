using System.Collections.ObjectModel;
using OpiumNetStat.model;
using OpiumNetStat.utils;

namespace OpiumNetStat.ViewModels
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
