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
        public async Task GetIPInfo(string ip, Action<Exception, IpInfo> callback)
        {
          

            using (var client = new WebClient())
            {
                try
                {
                    var uri = new Uri($"http://ip-api.com/json/{ip}");


                    var json = await client.DownloadStringTaskAsync(uri);
                    var result = JsonConvert.DeserializeObject<IpInfo>(json);

                    callback(null, result);
                  
                }
                catch (Exception ex)
                {
                    //hitting service too fast too often
                    Debug.Write(ex.Message);
                    callback(ex, null);
                }

            }

        }
    }
}
