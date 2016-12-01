using System;
using System.ServiceModel;
using System.Web.Http;
using System.Web.Http.SelfHost;
using AirPlayer.Utils;

namespace AirPlayer
{
    public class MediaStreamingServer : IDisposable
    {
        private readonly HttpSelfHostServer httpSelfHostServer;

        public MediaStreamingServer(int port = 8080)
        {
            Address = "http://" + Ext.GetIp4Address() + ":" + port;
            var config = new HttpSelfHostConfiguration(Address);
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new {id = RouteParameter.Optional}
                );
            config.TransferMode = TransferMode.Streamed;
            httpSelfHostServer = new HttpSelfHostServer(config);
        }

        /// <summary>
        ///     Get the address the selfhosting server is listening to
        /// </summary>
        public string Address { get; }

        /// <summary>
        ///     Close and dispose the selfhosting server
        /// </summary>
        public void Dispose()
        {
            httpSelfHostServer.CloseAsync().Wait();
            httpSelfHostServer.Dispose();
        }

        /// <summary>
        ///     Start the selfhosting server
        /// </summary>
        public MediaStreamingServer Start()
        {
            try
            {
                httpSelfHostServer.OpenAsync().Wait();
                return this;
            }
            catch (Exception)
            {
                throw new ArgumentException("Remember to run as administrator");
            }
        }

        /// <summary>
        ///     Start the selfhosting server async
        /// </summary>
        public MediaStreamingServer StartAsync()
        {
            try
            {
                httpSelfHostServer.OpenAsync();
                return this;
            }
            catch (Exception)
            {
                throw new ArgumentException("Remember to run as administrator");
            }
        }
    }
}