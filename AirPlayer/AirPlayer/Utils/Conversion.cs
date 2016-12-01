using System;
using System.IO;
using System.Threading;
using NReco.VideoConverter;

namespace AirPlayer.Utils
{
    public static class Conversion
    {
        public static void ReencodeVideoToMp4Async(string filePath)
        {
            new Thread(() => ReencodeVideoToMp4(filePath)) {IsBackground = true}.Start();
        }

        public static string ReencodeVideoToMp4(string filePath)
        {
            if (filePath.EndsWith(".mp4"))
                return null;

            var filename = Path.ChangeExtension(filePath, ".mp4");

            var ffMpeg = new FFMpegConverter();
            ffMpeg.ConvertProgress += ConvertProgressEvent;
            ffMpeg.ConvertMedia(filePath, filename, Format.mp4);

            return filename;
        }

        public static string RemuxVideoToMp4(string filePath, bool convertAudioToAcc = false,
            bool convertAudioFast = false)
        {
            if (filePath.EndsWith(".mp4"))
                return null;

            var filename = Path.ChangeExtension(filePath, ".mp4");
            var arguments = "-i " + '"' + filePath + '"' + " -codec copy ";
            if (convertAudioToAcc)
                arguments = arguments + "-c:a aac ";

            arguments = arguments + '"' + filename + '"';
            var ffMpeg = new FFMpegConverter();
            ffMpeg.Invoke(arguments);
            return filename;
        }

        public static void RemuxVideoToMp4Async(string filePath, bool convertAudioToAcc = false)
        {
            new Thread(() => RemuxVideoToMp4(filePath, convertAudioToAcc)) {IsBackground = true}.Start();
        }

        private static void ConvertProgressEvent(object sender, ConvertProgressEventArgs e)
        {
            Console.WriteLine("\n------------\nConverting...\n------------");
            Console.WriteLine("ProcessedDuration: {0}", e.Processed);
            Console.WriteLine("TotalDuration: {0}\n", e.TotalDuration);
        }
    }
}