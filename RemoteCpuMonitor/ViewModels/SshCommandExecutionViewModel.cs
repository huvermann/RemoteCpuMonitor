using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using RemoteCpuMonitor.Configuration;
using RemoteCpuMonitor.Events;
using RemoteCpuMonitor.Models;
using RemoteCpuMonitor.Notifications;
using RemoteCpuMonitor.SSHHelper;
using RemoteCpuMonitor.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace RemoteCpuMonitor.ViewModels
{
    public class SshCommandExecutionViewModel : BindableBase, IDisposable
    {
        private SudoHelper _sudoHelper;
        private ConnectionData _connectionData;
        private ICpuMonitorConfigSection _configuration;

        public SshCommandExecutionViewModel(IEventAggregator eventAggregator, SudoHelper sudoHelper, ICpuMonitorConfigSection configuration)
        {
            this._sudoHelper = sudoHelper;
            this._configuration = configuration;

            this._monitorDataEntries = new ObservableCollection<HeatingChartData>();
            //this.MonitorDataEntries.Add(new HeatingChartData() { Time = DateTime.ParseExact("17:06:00", "HH:mm:ss", CultureInfo.InvariantCulture), Value = 34 });
            //this.MonitorDataEntries.Add(new HeatingChartData() { Time = DateTime.ParseExact("17:06:05", "HH:mm:ss", CultureInfo.InvariantCulture), Value = 35 });
            //this.MonitorDataEntries.Add(new HeatingChartData() { Time = DateTime.ParseExact("17:06:10", "HH:mm:ss", CultureInfo.InvariantCulture), Value = 36 });
            //this.MonitorDataEntries.Add(new HeatingChartData() { Time = DateTime.ParseExact("17:06:15", "HH:mm:ss", CultureInfo.InvariantCulture), Value = 100 });

            eventAggregator.GetEvent<SshResponseMessageEvent>().Subscribe(onReceiveSshResponse);
            eventAggregator.GetEvent<ArmbianMontorMessageEvent>().Subscribe(onReceiveArmbianMonitorMessage);
            eventAggregator.GetEvent<CpuTempMonitorMessageEvent>().Subscribe(onReceiveCpuTemperatureMonitorMessage);
            ConfigureConnectionCommand = new DelegateCommand(ConfigureConnection);
            ExecuteSSHCommand = new DelegateCommand(ExecuteSsh);
            StartMonitorCommand = new DelegateCommand(StartMonitor);
            StopMonitorCommand = new DelegateCommand(StopMonitor);
            ClearOutputCommand = new DelegateCommand(() => {
                this.SshResponse = String.Empty;
                this._monitorDataEntries.Clear();
            });
            SetCredentialsCommand = new DelegateCommand(SetCredentials);
            TestButtonCommand = new DelegateCommand(TestButtonMethod);
            this.GetServerConnectionRequest = new InteractionRequest<ServerConnectionNotification>();
        }

        

        private void TestButtonMethod()
        {

        }

        #region MessageEventHandler
        private void onReceiveCpuTemperatureMonitorMessage(CpuTempMonitorMessage data)
        {
            DispatcherHelper.Invoke(() =>
            {
                HeatingChartData entry = new HeatingChartData() { Time = data.Time, Value = data.Temperature };
                _monitorDataEntries.Add(entry);
                this.SshResponse += string.Format("Daten hinzugefügt...{0}; {1}\n", data.Time, data.Temperature);
                scrollToend();


            });
        }

        private void onReceiveArmbianMonitorMessage(ArmbianMonitorResult data)
        {

            DispatcherHelper.Invoke(() =>
            {
                HeatingChartData entry = new HeatingChartData() { Time = data.Time, Value = data.Temperature };
                //this.MonitorDataEntries.Add(entry);
                _monitorDataEntries.Add(entry);
                Console.WriteLine("Daten hinzugefügt...");
                this.SshResponse += string.Format("Daten hinzugefügt...{0}; {1}\n", data.Time, data.Temperature);
                scrollToend();

            });
            
        }

        private void onReceiveSshResponse(SshResponse response)
        {
            DispatcherHelper.Invoke(() => {
                this.SshResponse += string.Format("[{0}]: {1}\n", response.Number, response.MessageText);
                scrollToend();
            });
            
        }
        #endregion

        private ObservableCollection<HeatingChartData> _monitorDataEntries;

        public ObservableCollection<HeatingChartData> MonitorDataEntries
        {
            get { return _monitorDataEntries; }
            set { SetProperty(ref _monitorDataEntries, value); }
        }

        private bool _autoscrollTrigger=true;
        public bool AutoscrollTrigger
        {
            get { return _autoscrollTrigger; }
            set { SetProperty(ref _autoscrollTrigger, value); }
        }

        private string _sshCommandLine;
        public string SshCommandLine
        {
            get { return _sshCommandLine; }
            set { SetProperty(ref _sshCommandLine, value); }
        }


        private string _sshResponse;
        public string SshResponse
        {
            get { return _sshResponse; }
            set { SetProperty(ref _sshResponse, value); }
        }

        private int counter = 0;

        private void scrollToend()
        {
            if (AutoscrollTrigger)
            {
                AutoscrollTrigger = false;
            }
            else
            {
                AutoscrollTrigger = true;
            }
        }

        private void SetCredentials()
        {
            _connectionData = new ConnectionData() { Hostname = "orangepipc", UserName = "pi", Password = "hhk13mi", PortNumber = 22 };
            this.OnPropertyChanged("isCredentialAvailable");
            this.OnPropertyChanged("isNotCredentialAvailable");

        }

        private void StartMonitor()
        {
            // this._sudoHelper.StartSession(this._connectionData, "sudo armbianmonitor -m");
            this._sudoHelper.StartSession(this._connectionData, "sudo coretempmon");
        }

        private void ExecuteSsh()
        {
            Console.WriteLine("Execute SSH command");
            this.SshResponse = String.Empty;
            if (this._connectionData != null)

            {
                this._sudoHelper.ExpectSSH(this._connectionData, this._sshCommandLine);
            }
        }

        private void StopMonitor()
        {
            this._sudoHelper.StopSession();
        }

        private void ConfigureConnection()
        {
            Console.WriteLine("Configure connection.");
            ServerConnectionNotification notification = new ServerConnectionNotification();
            notification.Title = "Configure Connection";

            this.GetServerConnectionRequest.Raise(notification, returned => {
                if (returned.Confirmed)
                {
                    this._connectionData = returned.ConnectionData;
                }
                else
                {
                    this._connectionData = null;
                }
                this.OnPropertyChanged("isCredentialAvailable");
                this.OnPropertyChanged("isNotCredentialAvailable");
            });
        }

        public bool isCredentialAvailable
        {
            get { return (this._connectionData != null); }
        }

        public bool isNotCredentialAvailable
        {
            get { return (this._connectionData == null); }
        }

        public DelegateCommand ConfigureConnectionCommand { get; private set; }
        public DelegateCommand ExecuteSSHCommand { get; private set; }
        public DelegateCommand StartMonitorCommand { get; private set; }
        public InteractionRequest<ServerConnectionNotification> GetServerConnectionRequest { get; private set; }
        public DelegateCommand StopMonitorCommand { get; private set; }
        public DelegateCommand ClearOutputCommand { get; private set; }
        public DelegateCommand SetCredentialsCommand { get; private set; }
        public DelegateCommand TestButtonCommand { get; private set; }



        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls
        

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (this._sudoHelper != null)
                    {
                        this._sudoHelper.StopSession();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SshCommandExecutionViewModel() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
