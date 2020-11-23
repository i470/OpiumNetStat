using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Session;

namespace OpiumNetStat.utils
{
    public static class ProcessInformation
    {
        public static ObservableCollection<Port> GetNetStatPorts()
        {
            var ports = new ObservableCollection<Port>();

            try
            {
                using (var p = new Process())
                {

                    ProcessStartInfo ps = new ProcessStartInfo();
                    ps.Arguments = "-a -n -o";
                    ps.FileName = "netstat.exe";
                    ps.UseShellExecute = false;
                    ps.WindowStyle = ProcessWindowStyle.Hidden;
                    ps.RedirectStandardInput = true;
                    ps.RedirectStandardOutput = true;
                    ps.RedirectStandardError = true;

                    p.StartInfo = ps;
                    p.Start();

                    StreamReader stdOutput = p.StandardOutput;
                    StreamReader stdError = p.StandardError;

                    string content = stdOutput.ReadToEnd() + stdError.ReadToEnd();
                    string exitStatus = p.ExitCode.ToString();

                    if (exitStatus != "0")
                    {
                       
                    }

                    //Get The Row
                    string[] rows = Regex.Split(content, "\r\n");
                    foreach (string row in rows)
                    {
                        //Split it baby
                        string[] tokens = Regex.Split(row, "\\s+");
                        if (tokens.Length > 4 && (tokens[1].Equals("UDP") || tokens[1].Equals("TCP")))
                        {
                            string localAddress = Regex.Replace(tokens[2], @"\[(.*?)\]", "1.1.1.1");
                            ports.Add(new Port
                            {
                                protocol = localAddress.Contains("1.1.1.1") ? String.Format("{0}v6", tokens[1]) : String.Format("{0}v4", tokens[1]),
                                port_number = localAddress.Split(':')[1],
                                process_name = tokens[1] == "UDP" ? LookupProcess(Convert.ToInt16(tokens[4])) : LookupProcess(Convert.ToInt16(tokens[5]))
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return ports;
        }

        public static string LookupProcess(int pid)
        {
            string procName;
            try {

                NetworkInfo ni = new NetworkInfo(pid);

                var proc = Process.GetProcessById(pid);

                Icon ico = Icon.ExtractAssociatedIcon(proc.MainModule.FileName);
                procName = proc.ProcessName;


                var data = ni.m_Counters;

                return procName;
            }
            catch (Exception ex) 
            {
                Debug.WriteLine(ex.Message);
            
            }

            return string.Empty;
        }



        public static ObservableCollection<TcpGlobalParameter> GeTcpGlobalParameters()
        {
            var tcpGlobalParameters=new ObservableCollection<TcpGlobalParameter>();

            using (var p = new Process())
            {
                var command = "netsh int tcp show global";
                var ps = new ProcessStartInfo("cmd", "/c " + command)
                {
                    
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                p.StartInfo = ps;
                p.Start();

                StreamReader stdOutput = p.StandardOutput;
                StreamReader stdError = p.StandardError;

                var content = stdOutput.ReadToEnd() + stdError.ReadToEnd();
                var exitStatus = p.ExitCode.ToString();

                if (exitStatus != "0")
                {

                }

                var rows = Regex.Split(content, "\n");
                var startRecording = false;

                foreach (var row in rows)
                {
                    if (row.StartsWith("Receive"))
                    {
                        startRecording = true;
                    }

                    if (!startRecording) continue;

                    var param = new TcpGlobalParameter();
                    var data = row.Split(':');

                    if (data.Length <= 1) continue;
                    param.Parameter = data[0].Trim();
                    param.Status = data[1].Trim();
                    tcpGlobalParameters.Add(param);
                }
                return tcpGlobalParameters;
            }
        }

        public class Port
        {
            public string name { get; set; }
            public string port_number { get; set; }
            public string process_name { get; set; }
            public string protocol { get; set; }

            public override string ToString()
            {
                return string.Format("{0} ({1} port {2})", this.process_name, this.protocol, this.port_number);
            }
        }

        public class TcpGlobalParameter
        {
            public string Parameter { get; set; }
            public string Status { get; set; }
        }
    }

    public class Counters
    {
        public long Received;
        public long Sent;
    }


    public class NetworkInfo
    {
        private DateTime m_EtwStartTime;
        private TraceEventSession m_EtwSession;
        private int pid;
        public readonly Counters m_Counters = new Counters();




        
        public NetworkInfo(int pid)
        {
            this.pid = pid;
        }

        private void Initialise()
        {
            // Note that the ETW class blocks processing messages, so should be run on a different thread if you want the application to remain responsive.
            Task.Run(() => StartEtwSession(this.pid));
        }

        public void  StartEtwSession(int pid)
        {
            try
            {
                var processId = pid;
               
                using (m_EtwSession = new TraceEventSession("MyKernelAndClrEventsSession"))
                {
                    m_EtwSession.EnableKernelProvider(KernelTraceEventParser.Keywords.NetworkTCPIP);

                    m_EtwSession.Source.Kernel.TcpIpRecv += data =>
                    {
                        if (data.ProcessID == processId)
                        {
                            lock (m_Counters)
                            {
                                m_Counters.Received += data.size;
                            }
                        }
                    };

                    m_EtwSession.Source.Kernel.TcpIpSend += data =>
                    {
                        if (data.ProcessID == processId)
                        {
                            lock (m_Counters)
                            {
                                m_Counters.Sent += data.size;
                            }
                        }
                    };

                    m_EtwSession.Source.Process();
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }
        }
    }
}
