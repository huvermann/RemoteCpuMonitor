using RemoteCpuMonitor.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RemoteCpuMonitor.Configuration.CpuMonitorConfigSection;

namespace RemoteCpuMonitor.Models
{
    public class ObservableHostList : ObservableCollection<SshHosts>
    {
        public ObservableHostList(ICpuMonitorConfigSection configuration)
        {
            if (configuration != null)
            {
                foreach(HostConfigElement host in configuration.Hosts)
                {
                    SshHosts newHost = new SshHosts() { Hostname = host.SSHServerHostname, UserName = host.Username, Password = host.Password, Port = host.SSHPort };
                    this.Add(newHost);
                }
            }
        }

    }

    public class SshHosts
    {
        public string Hostname { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }

    }
}
