using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteCpuMonitor.Events
{
    public class MasterNotificationMessageEvent : PubSubEvent<MasterNotification>
    {
    }

    public class MasterNotification
    {
        public NotificationType NotificationType { get; set; }
        public string MetaData { get; set; }
    }

    public enum NotificationType
    {
        Connect,
        Disconnect,
        Shutdown,
        ClearData
    }
}
