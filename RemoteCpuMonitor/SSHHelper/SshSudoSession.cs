using Prism.Events;
using RemoteCpuMonitor.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RemoteCpuMonitor.SSHHelper
{
    public class SshSudoSession : IDisposable
    {
        private IEventAggregator _eventaggregator;
        private IList<MatchAction> matchingAggregator;

        public SshSudoSession(IEventAggregator eventAggregator)
        {
            this._eventaggregator = eventAggregator;
        }

        public void AddMatching(Regex match, Action<string> action)
        {
            this.matchingAggregator.Add(new MatchAction() { RegExpression = match, Execute = action });

        }

        /// <summary>
        /// Run the Session until disconnect or breakSession
        /// </summary>
        /// <param name="connectionData"></param>
        /// <param name="command"></param>
        public void RunSession(ConnectionData connectionData, string command)
        {
            //Todo: Implement
        }

        /// <summary>
        /// Stops the running session
        /// </summary>
        public void StopSession()
        {
            // Todo: implement
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SshSudoSession() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

    public class MatchAction
    {
        public Regex RegExpression { get; set; }
        public Action<string> Execute { get; set; }
    }
}
