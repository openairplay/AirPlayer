using System;
using System.ServiceModel;
using System.Web.Http;
using System.Web.Http.SelfHost;
using AirPlayer.Utils;

namespace AirPlayer
{
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
