using System.Collections.ObjectModel;
using HBD.Framework.Collections;
using Opium_NetStat.model;
using Opium_NetStat.utils;

namespace Opium_NetStat.viewmodel
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
            TcpGlobalParameters=new ChangingObservableCollection<ProcessInformation.TcpGlobalParameter>();
            TcpGlobalParameters = ProcessInformation.GeTcpGlobalParameters();
        }
    }
}
