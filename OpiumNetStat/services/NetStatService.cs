using OpiumNetStat.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace OpiumNetStat.services
{
    public static class NetStatService
    {
       
        public static List<PortInfo> GetNetStatPorts()
        {
            var Ports = new List<PortInfo>();

            try
            {
                using (Process p = new Process())
                {

                    ProcessStartInfo ps = new ProcessStartInfo();
                    ps.Arguments = "-n -o";
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
                        // Command Errored. Handle Here If Need Be
                    }

                    //Get The Rows
                    string[] rows = Regex.Split(content, "\r\n");

                    foreach (string row in rows)
                    {
                        //Split it baby
                        string[] tokens = Regex.Split(row, "\\s+");

                        if (tokens.Length > 4 && (tokens[1].Equals("UDP") || tokens[1].Equals("TCP")))
                        {

                            var protocol = tokens[1];
                            var local = tokens[2];
                            var remote = tokens[3].Split(':').First();
                            var port = tokens[3].Split(':').Last();
                            var status = tokens[4];
                            var pid = tokens[5];
                            var program = LookupProcess(Convert.ToInt16(pid));

                            var pinfo = new PortInfo();
                            pinfo.PID = pid;
                            pinfo.name = program;
                            pinfo.port_number = port;
                            pinfo.remote_ip = remote;
                            pinfo.protocol = protocol;
                            pinfo.process_name = program;
                            pinfo.status = status;

                            Ports.Add(pinfo);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            return Ports;
        }

        public static string LookupProcess(int pid)
        {
            string procName;
            try { procName = Process.GetProcessById(pid).ProcessName; }
            catch (Exception) { procName = "-"; }
            return procName;
        }

       
    }
}
