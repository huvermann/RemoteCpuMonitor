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
        }

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
    }
}
