using OpiumNetStat.Model;
using OpiumNetStat.services;
using Prism.Events;
using Prism.Mvvm;
using System.Collections.Generic;
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

        IEventAggregator ea;
        IConnectionsService cs;

      

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



        public MainWindowViewModel(IEventAggregator _ea, IConnectionsService _cs)
        {

            ea = _ea;
            cs = _cs;

        }

    }



}






