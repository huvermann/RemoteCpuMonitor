using Prism.Mvvm;
using RemoteCpuMonitor.SSHHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RemoteCpuMonitor.Configuration.CpuMonitorConfigSection;

namespace RemoteCpuMonitor.ViewModels
{
    public class SshHostViewModel : BindableBase
    {

        public SshHostViewModel(ISudoHelper sudoHelper)
        {
            this._sudoHelper = sudoHelper;
        }
        private HostConfigElement _config;
        private ISudoHelper _sudoHelper;

        public void ConfigureHost(HostConfigElement config)
        {
            this.Hostname = config.SSHServerHostname;
            this.UserName = config.Username;
            this.Password = config.Password;
            this.Port = config.SSHPort;
            this._config = config;
            InitDisplay();
        }

        private void InitDisplay()
        {
            _headerText = string.Format("CPU Status: {0}", this._hostname);
            _freq1 = 0;
            _freq2 = 0;
            _freq3 = 0;
            _freq4 = 0;
            _cpuLoad1 = 0;
            _cpuLoad2 = 0;
            _cpuLoad3 = 0;
            _cpuLoad4 = 0;
            _temperature = 0;

        }
        #region ConfigurationData
        private string _hostname;
        public string Hostname
        {
            get { return _hostname; }
            set { SetProperty(ref _hostname, value); }
        }

        private string _userName;

        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _hostname, value); }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }


        private int _port;

        public int Port
        {
            get { return _port; }
            set { SetProperty(ref _port, value); }
        }
        #endregion

        #region DisplayProperties
        private string _headerText = "Default Header";
        public string HeaderText
        {
            get { return _headerText; }
            set { SetProperty(ref _headerText, value); }
        }

        private int _freq1;
        public int Freq1
        {
            get { return _freq1; }
            set { SetProperty(ref _freq1, value); }
        }
        private int _freq2;
        public int Freq2
        {
            get { return _freq2; }
            set { SetProperty(ref _freq2, value); }
        }
        private int _freq3;
        public int Freq3
        {
            get { return _freq3; }
            set { SetProperty(ref _freq3, value); }
        }
        private int _freq4;
        public int Freq4
        {
            get { return _freq4; }
            set { SetProperty(ref _freq4, value); }
        }
        private double _cpuLoad1;
        public double CpuLoad1
        {
            get { return _cpuLoad1; }
            set { SetProperty(ref _cpuLoad1, value); }
        }
        private double _cpuLoad2;
        public double CpuLoad2
        {
            get { return _cpuLoad2; }
            set { SetProperty(ref _cpuLoad2, value); }
        }
        private double _cpuLoad3;
        public double CpuLoad3
        {
            get { return _cpuLoad3; }
            set { SetProperty(ref _cpuLoad3, value); }
        }
        private double _cpuLoad4;
        public double CpuLoad4
        {
            get { return _cpuLoad4; }
            set { SetProperty(ref _cpuLoad4, value); }
        }

        private double _temperature;
        public double Temperature
        {
            get { return _temperature; }
            set { SetProperty(ref _temperature, value); }
        }
        #endregion
    }
}
