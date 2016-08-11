using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AirPlayer.AirPlay
{
    public class AirplayDevice
    {
        public const int DevicePort = 7000;
        public volatile bool Stop;
        public volatile bool StreamingVideo;

        public string IpAddress { get; }
        public Guid Id { get; }
        public string Name { get; }

        public AirplayDevice(string ipAddress, string name)
        {
            ServicePointManager.Expect100Continue = false;
            IpAddress = ipAddress;
            Name = name;
            Id = Guid.NewGuid();
        }

        private Uri GetAppleTvUrl()
        {
            return new Uri("http://" + IpAddress + ":" + DevicePort + "/");
        }

        /// <summary>
        ///     Shows the photo.
        /// </summary>
        /// <param name="photo">The photo.</param>
        public void ShowPhoto(byte[] photo)
        {
            var webClient = new WebClient();
            webClient.Headers.Add("User-Agent", "MediaControl/1.0");
            webClient.Headers.Add("X-Apple-Session-ID", Id.ToString());
            webClient.UploadData(GetAppleTvUrl() + "photo", "PUT", photo);
            webClient.Dispose();
        }

        /// <summary>
        ///     Starts a video.
        /// </summary>
        /// <param name="filePath">The URL of the video to play.</param>
        /// <param name="mediaStreamingServer"></param>
        /// <param name="startPosition">The start position of the video. This value must be between 0 and 1</param>
        public void StartVideo(Uri filePath, MediaStreamingServer mediaStreamingServer, float startPosition = 0)
        {
            StreamingVideo = true;
            if (startPosition > 1)
            {
                throw new ArgumentException("Start Position must be between 0 and 1");
            }

            var tcpClient = new TcpClient(IpAddress, DevicePort)
            {
                ReceiveTimeout = 5000,
                SendTimeout = 5000
            };

            //get the client stream to read data from.
            var clientStream = tcpClient.GetStream();

            var url = mediaStreamingServer.Address + "/play?filePath=" + filePath.AbsolutePath;
            var body = "Content-Location: " + url + "\n" +
                       "Start-Position: " + startPosition + "\n";

            var request = "POST /play HTTP/1.1\n" +
                          "User-Agent: MediaControl/1.0\n" +
                          "Content-Type: text/parameters\n" +
                          "Content-Length: " + Encoding.ASCII.GetBytes(body).Length + "\n" +
                          "X-Apple-Session-ID:" + Id + "\n\n";

            //Send the headers
            SendMessage(clientStream, request);
            //Send the body
            SendMessage(clientStream, body);

            //Get the response
            var myReadBuffer = new byte[1024];
            var myCompleteMessage = new StringBuilder();
            var numberOfBytesRead = 0;
            numberOfBytesRead = clientStream.Read(myReadBuffer, 0, myReadBuffer.Length);
            myCompleteMessage.Append(Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));

            //Now start doing a "keepalive"
            while (true)
            {
                if (Stop)
                {
                    tcpClient.Close();
                    clientStream.Close();
                    Stop = false;
                    StreamingVideo = false;
                    return;
                }
                //Simply send the characters "ok" every two seconds
                SendMessage(clientStream, "ok");
                Thread.Sleep(2000);
            }
        }

        /// <summary>
        ///     Sends a message across the NetworkStream
        /// </summary>
        /// <param name="clientStream">The stream to send the message down</param>
        /// <param name="message">The message to send</param>
        public void SendMessage(NetworkStream clientStream, string message)
        {
            var buffer = new ASCIIEncoding().GetBytes(message);
            try
            {
                clientStream.Write(buffer, 0, buffer.Length);
                clientStream.Flush();
            }
            catch (IOException e)
            {
                Debug.WriteLine("IOException: " + e.Message);
            }
        }
    }
}
