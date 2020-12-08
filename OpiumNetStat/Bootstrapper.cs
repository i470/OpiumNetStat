using OpiumNetStat.services;
using OpiumNetStat.Views;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;
using System.Windows;
using Unity;

namespace OpiumNetStat
{
    class Bootstrapper : PrismBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
            containerRegistry.RegisterSingleton<IConnectionsService, ConnectionsService>();
            containerRegistry.Register<IDataBaseService, DataBaseService>();
            containerRegistry.RegisterSingleton<IDataPipeLineService, DataPipeLineService>();
            containerRegistry.RegisterSingleton<IIpInfoService, IpInfoService>();

            var dp = containerRegistry.GetContainer().Resolve<IDataPipeLineService>();
          
        }


        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
        }
    }
}
