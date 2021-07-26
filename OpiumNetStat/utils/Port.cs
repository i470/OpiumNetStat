namespace OpiumNetStat.utils
{
    public static partial class ProcessInformation
    {
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
    }
}