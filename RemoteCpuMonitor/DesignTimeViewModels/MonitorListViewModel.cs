using RemoteCpuMonitor.ViewModels;
using System.Collections.ObjectModel;

namespace RemoteCpuMonitor.DesignTimeViewModels
{
    public class MonitorListViewModel
    {
        public ObservableCollection<DesignTimeSshViewModel> HostList { get; set; }
        public MonitorListViewModel()
        {
            HostList = new ObservableCollection<DesignTimeSshViewModel>();
            HostList.Add(new DesignTimeSshViewModel() { Hostname = "orangepipc", UserName = "pi" });
            HostList.Add(new DesignTimeSshViewModel() { Hostname = "Orangepione", UserName = "pi" });
        }
    }

    public class DesignTimeSshViewModel
    {
        public string Hostname { get; set; }
        public string UserName { get; set; }
    }
}
