using Newtonsoft.Json;
using OpiumNetStat.model;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace OpiumNetStat.services
{
    public class IpInfoService : IIpInfoService
    {
        public async Task GetIPInfo(string ip, Action<Exception, NetStatResult> ipCallback)
        {

            using (var client = new WebClient())
            {
                try
                {
                    var uri = new Uri($"http://ip-api.com/json/{ip}");


                    var json = await client.DownloadStringTaskAsync(uri);
                    var result = JsonConvert.DeserializeObject<Host>(json);

                    var record = new NetStatResult();
                    record.City = result.City;
                    record.Country = result.Country;
                    record.Org = result.Org;
                    record.CountryCode = result.CountryCode;

                    ipCallback(null, record);
                }
                catch (Exception ex)
                {
                    //hitting service too fast too often
                    Debug.Write(ex.Message);
                    ipCallback(ex, null);
                }

            }
        }
    }
}
