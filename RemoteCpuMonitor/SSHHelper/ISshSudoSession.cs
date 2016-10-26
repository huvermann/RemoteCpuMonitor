using System;
using System.Text.RegularExpressions;
using RemoteCpuMonitor.Notifications;

namespace RemoteCpuMonitor.SSHHelper
{
    public interface ISshSudoSession
    {
        void AddMatching(Regex match, Action<Match> action);
        void Dispose();
        void RunSession(ConnectionData connectionData, string command);
        void StopSession();
    }
}