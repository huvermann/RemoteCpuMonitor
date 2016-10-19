using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using RemoteCpuMonitor.Models;

namespace RemoteCpuMonitor.DesignTimeViewModels
{
    public class MonitorListViewModel
    {
        public ObservableCollection<SshHosts> HostList { get; set; }
        public MonitorListViewModel()
        {
            HostList = new ObservableCollection<SshHosts>();
            HostList.Add(new SshHosts() { Hostname = "orangepipc", UserName = "pi" });
            HostList.Add(new SshHosts() { Hostname = "Orangepione", UserName = "pi" });
        }
    }
}
