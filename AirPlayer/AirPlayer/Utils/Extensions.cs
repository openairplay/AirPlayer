using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace AirPlayer.Utils
{
    public static class Ext
    {
        public static byte[] LoadBytes(string path)
        {
            return File.ReadAllBytes(path);
        }

        public static string GetIp4Address()
        {
            var ip4Address = string.Empty;

            foreach (var ipa in Dns.GetHostAddresses(Dns.GetHostName()).Where(ipa => ipa.AddressFamily == AddressFamily.InterNetwork))
            {
                ip4Address = ipa.ToString();
                break;
            }

            return ip4Address;
        }
    }
}
