using Newtonsoft.Json;
using System.Windows.Media.Imaging;

namespace OpiumNetStat.Model
{
    public class ProcessIPInfo
    {
        public int port_number { get; set; }
        public string protocol { get; set; }
        public string remote_ip { get; set; }
        public int PID { get; internal set; }
        public string status { get; internal set; }
        public string process_name { get; set; }
        public BitmapImage icon { get; set; }
    }




}
