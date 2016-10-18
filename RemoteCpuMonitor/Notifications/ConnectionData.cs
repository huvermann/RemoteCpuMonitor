using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteCpuMonitor.Notifications
{
    public class ConnectionData
    {
        public string Hostname { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int PortNumber { get; set; }
    }
}
