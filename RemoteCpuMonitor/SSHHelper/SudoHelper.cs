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
    public class SudoHelper
    {
        private IEventAggregator eventAggregator;
        private Thread _SshClientThread;
        private bool _isClientRunning = false;
        private bool _abortClient = false;

        public SudoHelper(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

        }

        public void ExpectSSH(ConnectionData connectionData, string command)
        {
            try
            {
                SshClient sshClient = new SshClient(connectionData.Hostname, 22, connectionData.UserName, connectionData.Password);

                sshClient.Connect();
                IDictionary<Renci.SshNet.Common.TerminalModes, uint> termkvp = new Dictionary<Renci.SshNet.Common.TerminalModes, uint>();
                termkvp.Add(Renci.SshNet.Common.TerminalModes.ECHO, 53);

                // ShellStream shellStream = sshClient.CreateShellStream("xterm", 80, 24, 800, 600, 1024, termkvp);
                ShellStream shellStream = sshClient.CreateShellStream("text", 200, 24, 800, 600, 1024, termkvp);


                //Get logged in
                string response = shellStream.Expect(new Regex(@"[$>]")); //expect user prompt
                //this.writeOutput(results, rep);
                //this.writeOutput(rep);

                //send command
                shellStream.WriteLine(command);
                Thread.Sleep(500);
                response = shellStream.Expect(new Regex(@"([$#>:])")); //expect password or user prompt
                // this.writeOutput(results, rep);
                // this.writeOutput(rep);

                //check to send password
                if (response.Contains(string.Format("password for {0}:", connectionData.UserName)))
                {
                    //send password
                    shellStream.WriteLine(connectionData.Password);
                    response = shellStream.Expect(new Regex(@"[$#>]")); //expect user or root prompt
                    this.writeOutput(1, response);
                } else
                {
                    this.writeOutput(2, response);
                }

                sshClient.Disconnect();
            }//try to open connection
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
                throw ex;
            }

        }

        public bool IsClientRunning { get { return this._isClientRunning; } }

        public void StopSession()
        {
            if (_SshClientThread != null && _isClientRunning)
            {
                Thread.Sleep(1);
                this._abortClient = true;
                _SshClientThread.Join();
                _SshClientThread = null;
            }
        }

        public void StartSession(ConnectionData connectionData, string command)
        {
            _SshClientThread = new Thread(() => RunSshThread(connectionData, command));
            _SshClientThread.Start();
        }

        private void RunSshThread(ConnectionData connectionData, string command)
        {
            if (!_isClientRunning)
            {
                this._isClientRunning = true;
                while (!_abortClient)
                {
                    try
                    {
                        // Client erzeugen
                        SshClient sshClient = new SshClient(connectionData.Hostname, 22, connectionData.UserName, connectionData.Password);

                        // Verbinden
                        sshClient.Connect();
                        IDictionary<Renci.SshNet.Common.TerminalModes, uint> termkvp = new Dictionary<Renci.SshNet.Common.TerminalModes, uint>();
                        termkvp.Add(Renci.SshNet.Common.TerminalModes.ECHO, 53);
                        ShellStream shellStream = sshClient.CreateShellStream("text", 200, 24, 800, 600, 1024, termkvp);
                        string response = shellStream.Expect(new Regex(@"[$>]")); //expect user prompt

                        // Befehl ausführen
                        shellStream.WriteLine(command);
                        Thread.Sleep(500);
                        response = shellStream.Expect(new Regex(@"([$#>:])")); //expect password or user prompt
                        if (response.Contains(string.Format("password for {0}:", connectionData.UserName)))
                        {
                            //send password
                            shellStream.WriteLine(connectionData.Password);
                        } else
                        {
                            parseLine(response);
                        }

                        while (!_abortClient)
                        {
                            
                            string line = shellStream.ReadLine();
                            parseLine(line);
                        }

                        Console.WriteLine("Beende Session!");
                        _isClientRunning = false;
                        



                        sshClient.Disconnect();

                    }
                    catch (Exception e)
                    {

                        _abortClient = true;
                        _isClientRunning = false;
                        
                    }
                }
                _abortClient = false;
            }
        }

        private void parseLine(string line)
        {
            var data1 = CpuTempMonitorMessage.ParseMonitorString(line);
            if (data1 != null)
            {
                eventAggregator.GetEvent<CpuTempMonitorMessageEvent>().Publish(data1);
            }
            else
            {

                var data = ArmbianMonitorResult.ParseMonitorString(line);
                if (data != null)
                {
                    // Publish data
                    this.eventAggregator.GetEvent<ArmbianMontorMessageEvent>().Publish(data);
                }
                else
                {
                    eventAggregator.GetEvent<SshResponseMessageEvent>().Publish(new SshResponse() { MessageText = line, Number = 0 });
                }
            }

        }

        private void writeOutput(int nr, string rep)
        {
            Console.WriteLine(rep);
            eventAggregator.GetEvent<SshResponseMessageEvent>().Publish(new SshResponse() { MessageText = rep, Number = nr });
        }
    }
}
