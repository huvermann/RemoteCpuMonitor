using Microsoft.Practices.ServiceLocation;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using RemoteCpuMonitor.Configuration;
using RemoteCpuMonitor.Events;
using RemoteCpuMonitor.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RemoteCpuMonitor.ViewModels
{
    public class MonitorListViewModel : BindableBase
    {
        public MonitorListViewModel(IServiceLocator serviceLocator, IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;
            RegisterEventHandler(eventAggregator);
            this.HostList = (ObservableHostList)serviceLocator.GetInstance(typeof(ObservableHostList));

            // Commands
            ConnectAllCommand = new DelegateCommand(ConnectAll);
            DisconnectCommand = new DelegateCommand(DisconnectAll);
            ShutdownCommand = new DelegateCommand(Shutdown);
            TestFuncCommand = new DelegateCommand(TestFun);
        }

        private void TestFun()
        {
            this._eventAggregator.GetEvent<MasterNotificationMessageEvent>().Publish(new MasterNotification() { NotificationType = NotificationType.Testfunc });
        }

        private void RegisterEventHandler(IEventAggregator eventAggregator)
        {
            // Todo: Register the eventhandler
        }

        private void Shutdown()
        {
            this._eventAggregator.GetEvent<MasterNotificationMessageEvent>().Publish(new MasterNotification() { NotificationType = NotificationType.Shutdown });
        }

        private IEventAggregator _eventAggregator;

        private void DisconnectAll()
        {
            this._eventAggregator.GetEvent<MasterNotificationMessageEvent>().Publish(new MasterNotification() { NotificationType = NotificationType.Disconnect });
        }

        private void ConnectAll()
        {
            this._eventAggregator.GetEvent<MasterNotificationMessageEvent>().Publish(new MasterNotification() { NotificationType = NotificationType.Connect });
        }

        public ObservableHostList HostList { get; set; }
        public DelegateCommand ConnectAllCommand { get; private set; }
        public DelegateCommand DisconnectCommand { get; private set; }
        public DelegateCommand ShutdownCommand { get; private set; }
        public DelegateCommand TestFuncCommand { get; private set; }
    }
}
