using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TheCheapsLib;

namespace TheCheapsServer
{
    public class WhatsMyIp
    {
        //https://stackoverflow.com/questions/3253701/get-public-external-ip-address
        //  +
        //https://www.oreilly.com/library/view/async-in-c/9781449337155/ch01.html
        public static async Task<IPAddress> GetMyIpAsync()
        {
            List<string> services = new List<string>()
        {
            "https://ipv4.icanhazip.com",
            "https://api.ipify.org",
            "https://ipinfo.io/ip",
            "https://checkip.amazonaws.com",
            "https://wtfismyip.com/text",
            "http://icanhazip.com"
        };
            services.Shuffle();
            using (var webclient = new WebClient())
                foreach (var service in services)
                {
                    try
                    {
                        var content = await webclient.DownloadStringTaskAsync(service);
                        return IPAddress.Parse(content);
                    }
                    catch { }
                }
            return null;
        }
    }
}
