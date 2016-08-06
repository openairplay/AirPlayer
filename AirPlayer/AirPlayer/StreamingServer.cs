using System;
using System.Linq;
using System.Management;
using System.ServiceModel;
using System.Threading;
using System.Web.Http;
using System.Web.Http.SelfHost;
using AirPlayer.Utils;

namespace AirPlayer
{

    static public class Globals
    {
        static Globals()
        {
            // Create a semaphore with the amount of physical cores of the computer
            var coreCount = new ManagementObjectSearcher("Select * from Win32_Processor").Get().Cast<ManagementBaseObject>().Sum(item => int.Parse(item["NumberOfCores"].ToString()));
            Semaphore = new Semaphore(coreCount,coreCount);
        }

        public static Semaphore Semaphore;
    }

    public class StreamingServer : IDisposable
    {
        private readonly HttpSelfHostServer httpSelfHostServer;

        public StreamingServer()
        {
            var config = new HttpSelfHostConfiguration(ServerInfo.IpAddress);
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.TransferMode = TransferMode.Streamed;
            httpSelfHostServer = new HttpSelfHostServer(config);
            httpSelfHostServer.OpenAsync().Wait();
        }

        public void Dispose()
        {
            httpSelfHostServer.CloseAsync();
        }
    }

    public static class ServerInfo
    {
        public static readonly string IpAddress = "http://" + Ext.GetIp4Address() + ":8080";
    }
}
