using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using RemoteCpuMonitor.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RemoteCpuMonitor.ViewModels
{
    public class SshAuthenticationViewModel : BindableBase, IInteractionRequestAware
    {
        public SshAuthenticationViewModel()
        {
            SaveCredentialsCommand = new DelegateCommand(() => SaveCredentials());
            CancelCommand = new DelegateCommand(CancelDialog);

        }

        

        private ServerConnectionNotification _notification;


        private string _sshServer;

        public string SSHServer
        {
            get
            {
                return _sshServer;
            }
            set { SetProperty(ref _sshServer, value); }
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

        public DelegateCommand SaveCredentialsCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }

        public INotification Notification
        {
            get
            {
                return this._notification;
            }

            set
            {
                if (value is ServerConnectionNotification)
                {
                    this._notification = value as ServerConnectionNotification;
                    this.OnPropertyChanged(() => Notification);
                }
            }
        }

        public Action FinishInteraction
        {
            get; set;
        }

        private void SaveCredentials()
        {
            Console.WriteLine("Save credentials executed");
            this._notification.ConnectionData = new ConnectionData() { Hostname = this.SSHServer, UserName = this.UserName, Password = this.Password };
            this._notification.Confirmed = true;
            
            this.FinishInteraction();
        }

        private void CancelDialog()
        {
            Console.WriteLine("Cancel Dialog");
            this._notification.ConnectionData = null;
            this._notification.Confirmed = false;
            this.FinishInteraction();
        }

    }
}
