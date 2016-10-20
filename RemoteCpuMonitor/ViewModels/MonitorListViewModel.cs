using Microsoft.Practices.ServiceLocation;
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
        public MonitorListViewModel(IServiceLocator serviceLocator)
        {
            this.HostList = (ObservableHostList)serviceLocator.GetInstance(typeof(ObservableHostList));
        }

        public ObservableHostList HostList { get; set; }
    }
}
