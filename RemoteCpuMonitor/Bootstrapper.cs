using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Unity;
using RemoteCpuMonitor.Configuration;
using RemoteCpuMonitor.SSHHelper;
using System.Configuration;
using System.Windows;

namespace RemoteCpuMonitor
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            ModuleCatalog catalog = (ModuleCatalog)ModuleCatalog;
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            Container.RegisterInstance<ICpuMonitorConfigSection>(CpuMonitorConfigSection.Create());
            Container.RegisterType<ISudoHelper, SudoHelper>(); 

        }
    }
}
