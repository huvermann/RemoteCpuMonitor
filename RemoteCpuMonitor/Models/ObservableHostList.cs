using Microsoft.Practices.ServiceLocation;
using Prism.Mvvm;
using RemoteCpuMonitor.Configuration;
using RemoteCpuMonitor.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RemoteCpuMonitor.Configuration.CpuMonitorConfigSection;

namespace RemoteCpuMonitor.Models
{
    public class ObservableHostList : ObservableCollection<SshHostViewModel>
    {
        public ObservableHostList(ICpuMonitorConfigSection configuration, IServiceLocator serviceLocator)
        {
            if (configuration != null)
            {
                foreach(HostConfigElement hostConfig in configuration.Hosts)
                {
                    //var newHost = new SshHostViewModel() { Hostname = hostConfig.SSHServerHostname, UserName = hostConfig.Username, Password = hostConfig.Password, Port = hostConfig.SSHPort };
                    //var newHost = SshHostViewModel.CreateFromConfig(hostConfig, serviceLocator);
                    var newHost = (SshHostViewModel)serviceLocator.GetInstance(typeof(SshHostViewModel));
                    newHost.ConfigureHost(hostConfig);
                    this.Add(newHost);
                }
            }
        }

    }

    
}
