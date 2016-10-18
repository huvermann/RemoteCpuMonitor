using System;
using System.Windows.Controls;

namespace RemoteCpuMonitor.Views
{
    /// <summary>
    /// Interaction logic for SshCommandExecutionView
    /// </summary>
    public partial class SshCommandExecutionView : UserControl
    {
        public SshCommandExecutionView()
        {
            InitializeComponent();
            Dispatcher.ShutdownStarted += OnDispatcherShutDownStarted;
        }

        private void OnDispatcherShutDownStarted(object sender, EventArgs e)
        {
            var disposable = DataContext as IDisposable;
            if (!ReferenceEquals(null, disposable))
            {
                disposable.Dispose();
            }
        }
    }
}
