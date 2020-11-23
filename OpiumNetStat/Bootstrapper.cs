using OpiumNetStat.services;
using OpiumNetStat.Views;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;
using System.Windows;

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
        }


        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            // generic type
           // ViewModelLocationProvider.Register<MainWindow, MainViewModel>();
        }
    }
}
