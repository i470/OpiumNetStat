using System.Collections.ObjectModel;

using OpiumNetStat.model;
using OpiumNetStat.utils;

namespace OpiumNetStat.ViewModels
{
    public class TcpGlobalParametersViewModel:BaseNotify
    {
        private ObservableCollection<ProcessInformation.TcpGlobalParameter> tcpGlobalParameters;
        public ObservableCollection<ProcessInformation.TcpGlobalParameter> TcpGlobalParameters
        {

            get => tcpGlobalParameters;
            set
            {
                tcpGlobalParameters = value;
                RaisePropertyChanged(() => TcpGlobalParameters);
            }
        }

        public TcpGlobalParametersViewModel()
        {
            TcpGlobalParameters=new ObservableCollection<ProcessInformation.TcpGlobalParameter>();
            TcpGlobalParameters = ProcessInformation.GeTcpGlobalParameters();
        }
    }
}
