using Prism.Events;
using RemoteCpuMonitor.SSHHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteCpuMonitor.Events
{
    public class CpuTempMonitorMessageEvent : PubSubEvent<CpuTempMonitorMessage>
    {
    }
}
