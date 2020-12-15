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
        public async Task<IpInfo> GetIPInfo(ProcessIPInfo ip)
        {

            using (var client = new WebClient())
            {
                try
                {
                    var uri = new Uri($"http://ip-api.com/json/{ip.remote_ip}");


                    var json = await client.DownloadStringTaskAsync(uri);
                    var result = JsonConvert.DeserializeObject<IpInfo>(json);

                    if(result!=null)
                    {
                        result.Ip = ip.remote_ip;
                        return result;
                    }
                  
                }
                catch (Exception ex)
                {
                    //hitting service too fast too often
                    Debug.Write(ex.Message);
                }

            }

            return null;
        }
    }
}
