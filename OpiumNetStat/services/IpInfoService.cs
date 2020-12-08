using Newtonsoft.Json;
using OpiumNetStat.model;
using OpiumNetStat.Model;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace OpiumNetStat.services
{
    public class IpInfoService : IIpInfoService
    {
        public async Task<NetStatResult> GetIPInfo(PortInfo ip)
        {

            using (var client = new WebClient())
            {
                try
                {
                    var uri = new Uri($"http://ip-api.com/json/{ip.remote_ip}");


                    var json = await client.DownloadStringTaskAsync(uri);
                    var result = JsonConvert.DeserializeObject<Host>(json);

                    var record = new NetStatResult();
                    record.ConnectionStatus = "";
                    record.City = result.City;
                    record.Country = result.Country;
                    record.Org = result.Org;
                    record.CountryCode = result.CountryCode;
                    record.PortNumber = short.Parse(ip.port_number);
                    record.PID = short.Parse(ip.PID);
                    record.RemoteIP = ip.remote_ip;
                    record.Software = ip.process_name;
                    record.ConnectionStatus = ip.status;

                    return record;
                }
                catch (Exception ex)
                {
                    //hitting service too fast too often
                    Debug.Write(ex.Message);
                    return null;
                }

            }
        }
    }
}
