using System;
using System.IO;
using System.Threading;
using NReco.VideoConverter;

namespace AirPlayer.Utils
{
    public static class Conversion
    {
        public static void ConvertMkvToMp4(string filePath)
        {
            Globals.Semaphore.WaitOne();
            new Thread(() => ConvertMkv2Mp4(filePath)) {IsBackground = true}.Start();
            Globals.Semaphore.Release();
        }

        public static void ConvertAllMkvInFolderToMp4(string folderPath)
        {
            foreach (var file in Directory.GetFiles(folderPath))
            {
                new Thread(() =>
                {
                    Globals.Semaphore.WaitOne();
                    ConvertMkv2Mp4(file);
                    Globals.Semaphore.Release();
                })
                {IsBackground = true}.Start();
            }
        }

        static Uri ConvertMkv2Mp4(string filePath)
        {
            if (!filePath.EndsWith(".mkv"))
                return null;

            var filename = filePath.Replace(".mkv", ".mp4");

            var ffMpeg = new FFMpegConverter();
            ffMpeg.ConvertProgress += ConvertProgressEvent;
            ffMpeg.ConvertMedia(filePath, filename, Format.mp4);

            return new Uri(filename);
        }

        static void ConvertProgressEvent(object sender, ConvertProgressEventArgs e)
        {
            Console.WriteLine("\n------------\nConverting...\n------------");
            Console.WriteLine("ProcessedDuration: {0}", e.Processed);
            Console.WriteLine("TotalDuration: {0}\n", e.TotalDuration);
        }
    }
}
