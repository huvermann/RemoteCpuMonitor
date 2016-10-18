using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Interactivity.InteractionRequest;

namespace RemoteCpuMonitor.Notifications
{
    public class ServerConnectionNotification : Confirmation
    {
        public ConnectionData ConnectionData { get; set; }
    }
}
