using Prism.Events;
using Prism.Mvvm;
using RemoteCpuMonitor.Events;
using RemoteCpuMonitor.Models;
using RemoteCpuMonitor.Notifications;
using RemoteCpuMonitor.SSHHelper;
using RemoteCpuMonitor.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static RemoteCpuMonitor.Configuration.CpuMonitorConfigSection;

namespace RemoteCpuMonitor.ViewModels
{
    public class SshHostViewModel : BindableBase, IDisposable
    {

        private HostConfigElement _config;
        private IEventAggregator _eventAggregator;
        private List<CpuTempMonitorMessage> _statistics;
        private SshSudoSession _sudoSession;

        public SshHostViewModel(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;
            this._sudoSession = new SshSudoSession(eventAggregator);
            this.registerEvents();

            this._eventAggregator = eventAggregator;
            this._eventAggregator.GetEvent<MasterNotificationMessageEvent>().Subscribe(OnMasterNotificationMessage);
            this._eventAggregator.GetEvent<CpuTempMonitorMessageEvent>().Subscribe(onReceiveCpuTemperatureMonitorMessage);
            this._cpu1LoadEntries = new ObservableCollection<HeatingChartData>();
            this._cpu2LoadEntries = new ObservableCollection<HeatingChartData>();
            this._cpu3LoadEntries = new ObservableCollection<HeatingChartData>();
            this._cpu4LoadEntries = new ObservableCollection<HeatingChartData>();
            this._temperatureData = new ObservableCollection<HeatingChartData>();
            this._cpuClockFrequency = new ObservableCollection<HeatingChartData>();

            // Todo: implement reporting
            this._statistics = new List<CpuTempMonitorMessage>();
        }

        private void registerEvents()
        {
            this._eventAggregator.GetEvent<SshClientStatusMessageEvent>().Subscribe(onSshStatusMessage);
            this._sudoSession.AddMatching(new Regex(@"(\d{2}:\d{2}:\d{2})\s+(\d+[,.]\d+)[^\d]+(\d+)\sMHz\s(\d+[,.]\d{2})\s(\d+[.,]\d{2})\s(\d+[.,]\d{2})\s(\d+[.,]\d{2})"), onStatusMonitorMatch);
        }

        #region SshStatusNotifications
        private void onStatusMonitorMatch(Match match)
        {
            // Todo: read the match
            CpuTempMonitorMessage msg = CpuTempMonitorMessage.ParseMatchObject(match);
            this.onReceiveCpuTemperatureMonitorMessage(msg);
        }


        private void onReceiveCpuTemperatureMonitorMessage(CpuTempMonitorMessage message)
        {

            // Received a CPU-Status Message
            DispatcherHelper.Invoke(() =>
            {
                this._statistics.Add(message);
                HeatingChartData entry = new HeatingChartData() { Time = message.Time, Value = message.Temperature };
                    //Todo: Implement CPU Speed and Load
                    this._temperatureData.Add(entry);
                this._cpu1LoadEntries.Add(new HeatingChartData() { Time = message.Time, Value = message.CpuLoad1 });
                this._cpu2LoadEntries.Add(new HeatingChartData() { Time = message.Time, Value = message.CpuLoad2 });
                this._cpu3LoadEntries.Add(new HeatingChartData() { Time = message.Time, Value = message.CpuLoad3 });
                this._cpu4LoadEntries.Add(new HeatingChartData() { Time = message.Time, Value = message.CpuLoad4 });
                this._cpuClockFrequency.Add(new HeatingChartData() { Time = message.Time, Value = message.CpuSpeed });

                this.CpuLoad1 = message.CpuLoad1;
                this.CpuLoad2 = message.CpuLoad2;
                this.CpuLoad3 = message.CpuLoad3;
                this.CpuLoad4 = message.CpuLoad4;
                this.Freq1 = message.CpuSpeed;
                this.Freq2 = message.CpuSpeed;
                this.Freq3 = message.CpuSpeed;
                this.Freq4 = message.CpuSpeed;
                this.Temperature = message.Temperature;
            });

        }
        #endregion

        private void onSshStatusMessage(SshClientStatusMessage message)
        {
            if (message.Sender == this._sudoSession) {
                switch (message.MessageType)
                {
                    case SshClientStatusMessageType.Connecting:
                        Console.WriteLine("xxx Connecting");
                        break;
                    case SshClientStatusMessageType.Connected:
                        Console.WriteLine("xxx connected!");
                        break;
                    case SshClientStatusMessageType.Disconnecting:
                        Console.WriteLine("xxx disconnecting!");
                        break;
                    case SshClientStatusMessageType.Disconnected:
                        Console.WriteLine("xxx disconnected!");
                        break;
                    case SshClientStatusMessageType.ConnectionError:
                        Console.WriteLine(string.Format("xxx Connectionerror: {0}", message.MessageText));
                        break;
                    default:
                        break;
                }
            }
        }

        

        #region MasterNotificationMessage handling
        private void OnMasterNotificationMessage(MasterNotification message)
        {
            if (message != null)
            {
                switch (message.NotificationType)
                {
                    case NotificationType.Connect:
                        OnConnectNotificationMessage(message);
                        break;
                    case NotificationType.Disconnect:
                        OnDisconnectNotificationMessage(message);
                        break;
                    case NotificationType.Shutdown:
                        OnShutdownNotificationMessage(message);
                        break;
                    case NotificationType.ClearData:
                        OnClearDataNotificationMessage();
                        break;
                    case NotificationType.Testfunc:
                        OnTestFunNotificationMessage(message);
                        break;
                    default:
                        break;

                }
            }
        }

        private void OnTestFunNotificationMessage(MasterNotification message)
        {
            Console.WriteLine("Just a test function.");
            this._temperature = 70.1;
        }

        private void OnClearDataNotificationMessage()
        {
            this._cpu1LoadEntries.Clear();
            this._cpu2LoadEntries.Clear();
            this._cpu3LoadEntries.Clear();
            this._cpu4LoadEntries.Clear();
            this._temperatureData.Clear();
            this._statistics.Clear();
        }

        private void OnShutdownNotificationMessage(MasterNotification message)
        {
            //if (message != null)
            //{
            //    if (this._sudoHelper != null)
            //    {
            //        var connectiondata = new ConnectionData() { Hostname = this._hostname, UserName = this._userName, Password = this._password, PortNumber = this._port };
            //        this._sudoHelper.StopSession();
            //        this._sudoHelper.StartSession(connectiondata, "sudo shutdown now");
            //    }
            //}
        }

        private void OnDisconnectNotificationMessage(MasterNotification message)
        {
            if (message != null)
            {
                if (this._sudoSession != null)
                {
                    this._sudoSession.StopSession();
                    this._sudoSession.Dispose();
                    this._sudoSession = null;
                   
                }
            }
        }

        private void OnConnectNotificationMessage(MasterNotification message)
        {

            if (message != null)
            {
                if (this._sudoSession != null)
                {
                    var connectiondata = new ConnectionData() { Hostname = this._hostname, UserName = this._userName, Password = this._password, PortNumber = this._port };
                    this._sudoSession.RunSession(connectiondata, "sudo coretempmon");
                }
            }
        }
        #endregion

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
            _headerText = this._hostname;
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
            set { SetProperty(ref _userName, value); }
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

        private ObservableCollection<HeatingChartData> _cpu1LoadEntries;
        public ObservableCollection<HeatingChartData> Cpu1LoadEntries
        {
            get { return _cpu1LoadEntries; }
            set { SetProperty(ref _cpu1LoadEntries, value); }
        }
        private ObservableCollection<HeatingChartData> _cpu2LoadEntries;
        public ObservableCollection<HeatingChartData> Cpu2LoadEntries
        {
            get { return _cpu2LoadEntries; }
            set { SetProperty(ref _cpu2LoadEntries, value); }
        }

        private ObservableCollection<HeatingChartData> _cpu3LoadEntries;
        public ObservableCollection<HeatingChartData> Cpu3LoadEntries
        {
            get { return _cpu3LoadEntries; }
            set { SetProperty(ref _cpu3LoadEntries, value); }
        }
        private ObservableCollection<HeatingChartData> _cpu4LoadEntries;
        public ObservableCollection<HeatingChartData> Cpu4LoadEntries
        {
            get { return _cpu4LoadEntries; }
            set { SetProperty(ref _cpu4LoadEntries, value); }
        }
        private ObservableCollection<HeatingChartData> _temperatureData;
        public ObservableCollection<HeatingChartData> TemperatureData
        {
            get { return _temperatureData; }
            set { SetProperty(ref _temperatureData, value); }
        }

        private ObservableCollection<HeatingChartData> _cpuClockFrequency;
        public ObservableCollection<HeatingChartData> CpuClockFrequency
        {
            get { return _cpuClockFrequency; }
            set { SetProperty(ref _cpuClockFrequency, value); }
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls
        

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (this._sudoSession != null)
                    {
                        this._sudoSession.StopSession();
                        this._sudoSession.Dispose();
                        this._sudoSession = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SshHostViewModel() {
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
