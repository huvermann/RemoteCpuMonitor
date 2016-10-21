using RemoteCpuMonitor.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RemoteCpuMonitor.Views
{
    /// <summary>
    /// Interaction logic for MonitorListItemView
    /// </summary>
    public partial class MonitorListItemView : UserControl
    {

       
        public MonitorListItemView()
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
