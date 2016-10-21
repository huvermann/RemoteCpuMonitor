using RemoteCpuMonitor.Notifications;
using System;

namespace RemoteCpuMonitor.SSHHelper
{
    public interface ISudoHelper
    {
        bool IsClientRunning { get; }

        void ExpectSSH(ConnectionData connectionData, string command);
        void StartSession(ConnectionData connectionData, string command);
        void StopSession();
    }
}