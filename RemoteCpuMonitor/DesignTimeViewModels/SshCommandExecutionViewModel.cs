using RemoteCpuMonitor.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteCpuMonitor.DesignTimeViewModels
{
    public class SshCommandExecutionViewModel
    {
        public SshCommandExecutionViewModel()
        {
            this.MonitorDataEntries = new ObservableCollection<HeatingChartData>();
            this.MonitorDataEntries.Add(new HeatingChartData() { Time = DateTime.ParseExact("17:06:00", "HH:mm:ss", CultureInfo.InvariantCulture), Value = 34 });
            this.MonitorDataEntries.Add(new HeatingChartData() { Time = DateTime.ParseExact("17:06:05", "HH:mm:ss", CultureInfo.InvariantCulture), Value = 35 });
            this.MonitorDataEntries.Add(new HeatingChartData() { Time = DateTime.ParseExact("17:06:10", "HH:mm:ss", CultureInfo.InvariantCulture), Value = 36 });
            this.MonitorDataEntries.Add(new HeatingChartData() { Time = DateTime.ParseExact("17:06:15", "HH:mm:ss", CultureInfo.InvariantCulture), Value = 100 });
            SshResponse = "Designtime Status";
        }
        public string SshResponse { get; set; }

        private ObservableCollection<HeatingChartData> _monitorDataEntries;

        public ObservableCollection<HeatingChartData> MonitorDataEntries
        {
            get { return _monitorDataEntries; }
            set { _monitorDataEntries = value; }
        }
    }
}
