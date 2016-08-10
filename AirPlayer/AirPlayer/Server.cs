using System;
using System.ServiceModel;
using System.Web.Http;
using System.Web.Http.SelfHost;
using AirPlayer.Utils;

namespace AirPlayer
{
    public class Server : IDisposable
    {
        private readonly HttpSelfHostServer httpSelfHostServer;

        public Server(int port = 8080)
        {
            Address = "http://" + Ext.GetIp4Address() + ":"+ port;
            var config = new HttpSelfHostConfiguration(Address);
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.TransferMode = TransferMode.Streamed;
            httpSelfHostServer = new HttpSelfHostServer(config);
        }

        public string Address { get; }

        /// <summary>
        /// Start the selfhosing server
        /// </summary>
        public void Start()
        {
            try
            {
                httpSelfHostServer.OpenAsync().Wait();
            }
            catch (Exception)
            {
                throw new ArgumentException("Remember to run as administrator");
            }
        }

        /// <summary>
        /// Close and dispose the selfhosting server
        /// </summary>
        public void Dispose()
        {
            httpSelfHostServer.CloseAsync().Wait();
            httpSelfHostServer.Dispose();
        }
    }
}
