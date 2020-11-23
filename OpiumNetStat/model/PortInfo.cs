using Newtonsoft.Json;

namespace OpiumNetStat.model
{
    public class PortInfo
    {
        [JsonProperty("description")]
        public string Desciption;

        [JsonProperty("port-start")]
        public string PortStart;

        [JsonProperty("port-end")]
        public string PortEnd;

        [JsonProperty("status")]
        public string Status;

        [JsonProperty("tcp")]
        public string Tcp;

        [JsonProperty("udp")]
        public string Udp;
    }

    
}
