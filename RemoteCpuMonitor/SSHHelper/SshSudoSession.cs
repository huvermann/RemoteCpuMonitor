using Prism.Events;
using RemoteCpuMonitor.Events;
using RemoteCpuMonitor.Notifications;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteCpuMonitor.SSHHelper
{
    public class SshSudoSession : IDisposable
    {
        private IEventAggregator _eventaggregator;
        private IList<MatchAction> matchingAggregator;
        private bool _isSessionRunning;
        private ConnectionData _connectionData;
        private string _commandString;
        private Thread _sessionThread;
        private volatile bool _terminateThread;


        public SshSudoSession(IEventAggregator eventAggregator)
        {
            this._eventaggregator = eventAggregator;
            this.matchingAggregator = new List<MatchAction>();
            
        }

        public void AddMatching(Regex match, Action<Match> action)
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
            if (!this._isSessionRunning)
            {
                this._isSessionRunning = true;
                this._terminateThread = false;
                this._connectionData = connectionData;
                this._commandString = command;
                this._sessionThread = new Thread(() => sshThreadRunner(connectionData, command));
                this._sessionThread.Start();


            }
        }
        /// <summary>
        /// Runs in a new thread
        /// </summary>
        /// <param name="connectionData">The connection data.</param>
        /// <param name="command">The command string.</param>
        private void sshThreadRunner(ConnectionData connectionData, string command)
        {
            using (SshClient sshClient = new SshClient(connectionData.Hostname, 22, connectionData.UserName, connectionData.Password))
            {
                try
                {
                    sshClient.ErrorOccurred += SshClient_ErrorOccurred;
                    this._eventaggregator.GetEvent<SshClientStatusMessageEvent>().Publish(new SshClientStatusMessage() { Sender = this, MessageType = SshClientStatusMessageType.Connecting });
                    sshClient.Connect();

                    
                
                    IDictionary<Renci.SshNet.Common.TerminalModes, uint> termkvp = new Dictionary<Renci.SshNet.Common.TerminalModes, uint>();
                    termkvp.Add(Renci.SshNet.Common.TerminalModes.ECHO, 53);
                    ShellStream shellStream = sshClient.CreateShellStream("text", 200, 24, 800, 600, 1024, termkvp);

                    Console.WriteLine("Verbunden mit: {0}", connectionData.Hostname);
                    this._eventaggregator.GetEvent<SshClientStatusMessageEvent>().Publish(new SshClientStatusMessage()
                    { Sender = this, MessageType = SshClientStatusMessageType.Connected, MessageText = string.Format("Connected to: {0}", connectionData.Hostname) });
                    string response = shellStream.Expect(new Regex(@"[$>]")); //expect user prompt

                    shellStream.WriteLine(command);
                    Thread.Sleep(500);
                    while (!this._terminateThread)
                    {
                        Console.WriteLine("Still listening on ssh...");
                        // Todo: read the data
                        //response = shellStream.Expect(new Regex(@"[$>]")); //expect user prompt
                        response = shellStream.Expect(new Regex(@"([$#>:])")); //expect password or user prompt
  

                        //check to send password
                        if (response.Contains(string.Format("password for {0}:", connectionData.UserName)))
                        {
                            //send password
                            shellStream.WriteLine(connectionData.Password);
                        }
                        else
                        {
                            ParseResponseData(response);
                        }
                        Thread.Sleep(10);
                    }

                }
                catch (Exception e)
                {
                    this._terminateThread = true;
                    this._eventaggregator.GetEvent<SshClientStatusMessageEvent>().Publish(new SshClientStatusMessage() { Sender = this, MessageType = SshClientStatusMessageType.ConnectionError, MessageText = e.Message });
                    sshClient.Disconnect();
                }

                sshClient.Disconnect();
                this._eventaggregator.GetEvent<SshClientStatusMessageEvent>().Publish(new SshClientStatusMessage() { Sender = this, MessageType = SshClientStatusMessageType.Disconnected });
            }
        }

        private void SshClient_ErrorOccurred(object sender, Renci.SshNet.Common.ExceptionEventArgs e)
        {
            this._eventaggregator.GetEvent<SshClientStatusMessageEvent>().Publish(new SshClientStatusMessage() { Sender = this, MessageType = SshClientStatusMessageType.ConnectionError, MessageText = e.Exception.Message });
        }

        private void ParseResponseData(string response)
        {

            foreach (var expression in this.matchingAggregator)
            {
                var match = expression.RegExpression.Match(response);
                if (match.Success)
                {
                    expression.Execute.Invoke(match);
                }
            }
        }

        /// <summary>
        /// Stops the running session
        /// </summary>
        public void StopSession()
        {
            if (this._sessionThread != null)
            {
                if (this._sessionThread.IsAlive)
                {
                    Thread.Sleep(1);
                    // Todo: implement
                    this._terminateThread = true;
                    Thread.Sleep(1);
                    this._sessionThread.Join();
                }
            }
            
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
                    if (this._sessionThread != null && this._sessionThread.IsAlive)
                    {
                        this._sessionThread.Abort();
                        this._sessionThread = null;
                    }
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
        public Action<Match> Execute { get; set; }
    }
}
