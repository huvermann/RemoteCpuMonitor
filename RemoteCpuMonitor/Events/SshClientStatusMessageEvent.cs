using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteCpuMonitor.Events
{
    public class SshClientStatusMessageEvent : PubSubEvent<SshClientStatusMessage>
    {
    }
    public enum SshClientStatusMessageType {
        Connecting,
        Connected,
        Disconnecting,
        Disconnected,
        ConnectionError
    }
    public class SshClientStatusMessage
    {
        public Object Sender { get; set; }
        public SshClientStatusMessageType MessageType { get; set; }
        public string MessageText { get; set; }
    }
}
