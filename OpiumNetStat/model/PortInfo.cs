using Newtonsoft.Json;

namespace OpiumNetStat.Model
{
    public class PortInfo
    {
        public string name
        {
            get
            {
                return string.Format("{0} {1} {2} {3}", this.remote_ip, this.process_name, this.protocol, this.port_number);
            }
            set { }
        }
        public string port_number { get; set; }
        public string process_name { get; set; }
        public string protocol { get; set; }
        public string remote_ip { get; set; }
        public string PID { get; internal set; }
        public string status { get; internal set; }
    }




}
