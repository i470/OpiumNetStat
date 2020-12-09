using OpiumNetStat.events;
using OpiumNetStat.ViewModels;
using Prism.Events;
using System;
using System.Windows;
using System.Windows.Controls;

namespace OpiumNetStat.Views
{
    /// <summary>
    /// Interaction logic for ConnectionsView.xaml
    /// </summary>
    public partial class ConnectionsView : UserControl
    {
        IEventAggregator _ea;

        public ConnectionsView(IEventAggregator ea)
        {
            InitializeComponent();
            VisualStateManager.GoToState(this, "BusyState", true);
            _ea = ea;
            _ea.GetEvent<IsBusyEvent>().Subscribe(UpdateViewState);


        }

        private void UpdateViewState(bool isBusy)
        {
            if(isBusy)
            {
                VisualStateManager.GoToState(this, "BusyState", true);
            }else
            {
                VisualStateManager.GoToState(this, "RegularState", true);
            }
          
        }
    }
}
