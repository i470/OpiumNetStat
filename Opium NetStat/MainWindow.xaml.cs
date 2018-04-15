using System.Windows;
using Opium_NetStat.viewmodel;

namespace Opium_NetStat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var vm = new MainViewModel
            {
                NetStatVM = new NetStatViewModel()
            };

            this.DataContext = vm;

          

        }
    }
}
