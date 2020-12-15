using OpiumNetStat.events;
using OpiumNetStat.Model;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Media.Imaging;

namespace OpiumNetStat.services
{
    public class ConnectionsService : IConnectionsService
    {
        IEventAggregator _ea;
        CancellationTokenSource wtoken;
        ActionBlock<DateTimeOffset> task;

        Dictionary<IPAddress, ProcessIPInfo> _netStatRegistry;
       public Dictionary<IPAddress, ProcessIPInfo> NetStatRegistry
        {
            get { return _netStatRegistry; }
            set { _netStatRegistry = value; }
        }

        Dictionary<string, BitmapImage> icons = new Dictionary<string, BitmapImage>();

        public ConnectionsService(IEventAggregator ea)
        {
            NetStatRegistry = new Dictionary<IPAddress, ProcessIPInfo>();
            _ea = ea;
        }


        public void StartWork()
        {

            wtoken = new CancellationTokenSource();
            task = (ActionBlock<DateTimeOffset>)CreateNeverEndingTask( now =>  DoWork(), wtoken.Token);
            task.Post(DateTimeOffset.Now);
        }

        public void DoWork()
        {

           var netStatOutput =  NetStatService.GetNetStatOutput();


            foreach (string row in netStatOutput)
            {
                //Split it baby
                string[] tokens = Regex.Split(row, "\\s+");

                if (tokens.Length > 4 && (tokens[1].Equals("UDP") || tokens[1].Equals("TCP")))
                {
                    IPAddress ip;

                    // we only care about remote IP
                    if (tokens[3].Contains(":") && IPAddress.TryParse(tokens[3].Split(':')[0], out ip) && !tokens[3].Split(':')[0].StartsWith("192"))
                    {

                        if (!IPAddress.IsLoopback(ip))
                        {
                            var addressFamily = ip.AddressFamily;

                            if (!NetStatRegistry.ContainsKey(ip))
                            {
                                int port;
                                int pid;

                                var pinfo = new ProcessIPInfo();
                                pinfo.remote_ip = ip.ToString();

                                if (int.TryParse(tokens[3].Split(':')[1], out port))
                                {
                                    pinfo.port_number = port;
                                }

                                if (int.TryParse(tokens[5], out pid))
                                {
                                    pinfo.PID = pid;
                                    pinfo.process_name = LookupProcess(pid);

                                    if(icons.ContainsKey(pinfo.process_name))
                                    {
                                        if(icons[pinfo.process_name]==null)
                                        {
                                            icons[pinfo.process_name] = GetProcessIcon(pid);
                                        }
                                    }
                                    else
                                    {
                                        icons.Add(pinfo.process_name, GetProcessIcon(pid));
                                    }

                                    pinfo.icon = icons[pinfo.process_name];
                                }

                                pinfo.status = tokens[4];
                                pinfo.protocol = tokens[1];
                                NetStatRegistry.Add(ip,pinfo);

                                _ea.GetEvent<NetStatReadEvent>().Publish(pinfo);
                            }
                            else
                            {
                                var knownRecord = NetStatRegistry[ip];
                                knownRecord.status = tokens[4];
                                _ea.GetEvent<NetStatReadEvent>().Publish(knownRecord);
                            }

                        }
                    }
                }
            }

        }

        public string LookupProcess(int pid)
        {
            string procName;
            try {

                var proc = Process.GetProcessById(pid);     
                procName = Process.GetProcessById(pid).ProcessName; 
            }
            catch (Exception) { procName = "-"; }
            return procName;
        }

        public BitmapImage GetProcessIcon(int pid)
        {
            try
            {

                var proc = Process.GetProcessById(pid);
                if (proc != null && proc.MainModule != null && proc.MainModule.FileName != null)
                {
                    Icon ico = Icon.ExtractAssociatedIcon(proc.MainModule.FileName);

                    Bitmap bitmap = ico.ToBitmap();
                    MemoryStream stream = new MemoryStream();

                    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    BitmapImage bmpImage = new BitmapImage();
                    bmpImage.BeginInit();
                    stream.Seek(0, SeekOrigin.Begin);
                    bmpImage.StreamSource = stream;
                    bmpImage.EndInit();
                    bmpImage.Freeze();

                    return bmpImage;
                }

                return null;
            }
            catch (Exception ex) 
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }


        void StopWork()
        {

            using (wtoken)
            {

                wtoken.Cancel();
            }

            wtoken = null;
            task = null;
        }


        ITargetBlock<DateTimeOffset> CreateNeverEndingTask(Action<DateTimeOffset> action, CancellationToken cancellationToken)
        {
            // Validate parameters.
            if (action == null) throw new ArgumentNullException("action");


            ActionBlock<DateTimeOffset> block = null;

            block = new ActionBlock<DateTimeOffset>(async now =>
            {

                action(now);
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken).ConfigureAwait(false);
                block.Post(DateTimeOffset.Now);
            },
            new ExecutionDataflowBlockOptions
            {
                CancellationToken = cancellationToken
            });

            return block;
        }

    }
}
