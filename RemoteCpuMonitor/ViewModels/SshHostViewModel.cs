﻿using Microsoft.Practices.ServiceLocation;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
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
        private ISshSudoSession _sudoSession;
        private IServiceLocator _serviceLocator;

        public SshHostViewModel(IEventAggregator eventAggregator, IServiceLocator serviceLocator, ISshSudoSession sshSudoSession)
        {
            this._serviceLocator = serviceLocator;
            this._eventAggregator = eventAggregator;
            this._sudoSession = sshSudoSession;
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

            // Commands
            ConfigureCommand = new DelegateCommand(onConfigureClicked);
            ConnectCommand = new DelegateCommand(OnConnectCommandClicked);
            BenchmarkCommand = new DelegateCommand(OnBenchmarkClicked);

            // InteractionRequests
            GetServerConnectionRequest = new InteractionRequest<ServerConnectionNotification>();
        }

        private void registerEvents()
        {
            this._eventAggregator.GetEvent<SshClientStatusMessageEvent>().Subscribe(onSshStatusMessage);
            this._sudoSession.AddMatching(new Regex(@"(\d{2}:\d{2}:\d{2})\s+(\d+[,.]\d+)[^\d]+(\d+)\sMHz\s(\d+[,.]\d{2})\s(\d+[.,]\d{2})\s(\d+[.,]\d{2})\s(\d+[.,]\d{2})"), onStatusMonitorMatch);
        }

        #region CommandHandlers
        private void OnBenchmarkClicked()
        {
            throw new NotImplementedException();
        }

        private void OnConnectCommandClicked()
        {
            throw new NotImplementedException();
        }

        private void onConfigureClicked()
        {
            Console.WriteLine("Clicked on Configure");
            ServerConnectionNotification notification = new ServerConnectionNotification();
            notification.Title = "Configure Connection";
            notification.ConnectionData = new ConnectionData() { Hostname = this.Hostname, UserName = this.UserName, Password = this.Password, PortNumber = this.Port };

            this.GetServerConnectionRequest.Raise(notification, returned => {
                if (returned.Confirmed)
                {
                    Console.WriteLine("Connection data confirmed!");
                }
            });
        }
        #endregion

        

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
                this._cpuClockFrequency.Add(new HeatingChartData() { Time = message.Time, Value = message.CpuSpeed / 1000000 });

                this.CpuLoad1 = message.CpuLoad1;
                this.CpuLoad2 = message.CpuLoad2;
                this.CpuLoad3 = message.CpuLoad3;
                this.CpuLoad4 = message.CpuLoad4;
                this.Freq1 = message.CpuSpeed;
                this.Freq2 = message.CpuSpeed;
                this.Freq3 = message.CpuSpeed;
                this.Freq4 = message.CpuSpeed;
                this.Temperature = message.Temperature;
                this.TemperatureMinimum = message.Temperature;
                this.TemperatureMaximum = message.Temperature;
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
                        this.OnlineStatus = message.MessageType;
                        break;
                    case SshClientStatusMessageType.Connected:
                        Console.WriteLine("xxx connected!");
                        this.OnlineStatus = message.MessageType;
                        break;
                    case SshClientStatusMessageType.Disconnecting:
                        Console.WriteLine("xxx disconnecting!");
                        this.OnlineStatus = message.MessageType;
                        break;
                    case SshClientStatusMessageType.Disconnected:
                        Console.WriteLine("xxx disconnected!");
                        this.OnlineStatus = message.MessageType;
                        break;
                    case SshClientStatusMessageType.ConnectionError:
                        Console.WriteLine(string.Format("xxx Connectionerror: {0}", message.MessageText));
                        this.OnlineStatus = message.MessageType;
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
            if (message != null)
            {
                // mit await runsession async implementieren:
                //using(ISshSudoSession session = this._serviceLocator.GetInstance<ISshSudoSession>())
                //{
                //    var connectiondata = new ConnectionData() { Hostname = this._hostname, UserName = this._userName, Password = this._password, PortNumber = this._port };
                //    session.RunSession(connectiondata, "sudo shutdown now", () => { });
                //}
            }
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
                if (_sudoSession == null)
                {
                    _sudoSession= this._serviceLocator.TryResolve<ISshSudoSession>();
                }
                if (this._sudoSession != null)
                {
                    var connectiondata = new ConnectionData() { Hostname = this._hostname, UserName = this._userName, Password = this._password, PortNumber = this._port };
                    this._sudoSession.RunSession(connectiondata, "sudo coretempmon -d 2000");
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
            _temperatureMaximum = null;
            _temperatureMinimum = null;
            _onlineStatus = SshClientStatusMessageType.Disconnected;

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

        private double _freq1;
        public double Freq1
        {
            get { return _freq1; }
            set { SetProperty(ref _freq1, value); }
        }
        private double _freq2;
        public double Freq2
        {
            get { return _freq2; }
            set { SetProperty(ref _freq2, value); }
        }
        private double _freq3;
        public double Freq3
        {
            get { return _freq3; }
            set { SetProperty(ref _freq3, value); }
        }
        private double _freq4;
        public double Freq4
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

        private double? _temperatureMinimum;
        public double? TemperatureMinimum
        {
            get { return _temperatureMinimum; }
            set
            {
                if (_temperatureMinimum > value || _temperatureMinimum == null)
                {
                    SetProperty(ref _temperatureMinimum, value);
                }
            }
        }

        private double? _temperatureMaximum;
        public double? TemperatureMaximum
        {
            get { return _temperatureMaximum; }
            set
            {
                if (_temperatureMaximum < value || _temperatureMaximum == null)
                {
                    SetProperty(ref _temperatureMaximum, value);
                }
            }
        }

        private SshClientStatusMessageType _onlineStatus;
        public SshClientStatusMessageType OnlineStatus
        {
            get { return _onlineStatus; }
            set { SetProperty(ref _onlineStatus, value); }
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
        #region Commands
        public DelegateCommand ConfigureCommand { get; private set; }
        public DelegateCommand ConnectCommand { get; private set; }
        public DelegateCommand BenchmarkCommand { get; private set; }
        #endregion
        #region InteractionRequests
        public InteractionRequest<ServerConnectionNotification> GetServerConnectionRequest { get; private set; }
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
