using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using NReco.VideoConverter;

namespace AirPlayer.Controllers
{
    public class ConversionController : ApiController
    {

        [HttpGet]
        [Route("ConvertAllMKVFilesInFolderToMp4")]
        public HttpResponseMessage ConvertAllMkvFilesInFolderToMp4(string folderPath)
        {
            foreach (var file in Directory.GetFiles(folderPath))
            {
                new Thread(() =>
                {
                    Globals.Semaphore.WaitOne();
                    ConvertMkv2Mp4(file);
                    Globals.Semaphore.Release();
                }) {IsBackground = true}.Start();
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpGet]
        [Route("ConvertMKVToMp4")]
        public HttpResponseMessage ConvertMKVToMp4(string filePath)
        {
            new Thread(() => ConvertMkv2Mp4(filePath)) { IsBackground = true }.Start();

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        private Uri ConvertMkv2Mp4(string filePath)
        {
            if (!filePath.EndsWith(".mkv"))
                return null;

            var filename = filePath.Replace(".mkv", ".mp4");

            var ffMpeg = new FFMpegConverter();
            ffMpeg.ConvertProgress += ConvertProgressEvent;
            ffMpeg.ConvertMedia(filePath, filename, Format.mp4);

            return new Uri(filename);
        }

        private void ConvertProgressEvent(object sender, ConvertProgressEventArgs e)
        {
            Console.WriteLine("\n------------\nConverting...\n------------");
            Console.WriteLine("ProcessedDuration: {0}", e.Processed);
            Console.WriteLine("TotalDuration: {0}\n", e.TotalDuration);
        }
    }
}
