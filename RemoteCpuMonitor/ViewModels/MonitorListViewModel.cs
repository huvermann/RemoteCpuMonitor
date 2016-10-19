using Prism.Commands;
using Prism.Mvvm;
using RemoteCpuMonitor.Configuration;
using RemoteCpuMonitor.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RemoteCpuMonitor.ViewModels
{
    public class MonitorListViewModel : BindableBase
    {
        public MonitorListViewModel(ICpuMonitorConfigSection configuration)
        {
            this.HostList = new ObservableHostList(configuration);
        }

        public ObservableHostList HostList { get; set; }
    }
}
