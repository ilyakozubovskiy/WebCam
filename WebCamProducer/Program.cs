using System;
using System.Net;
using System.Configuration;
using AForge.Video.DirectShow;
using System.Net.Sockets;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace WebCamProducer
{
   static class Program
    {
        private static IPEndPoint consumerEndPoint;
        private static readonly UdpClient udpClient = new UdpClient();

        static void Main()
        {

            int consumerPort = int.Parse(ConfigurationManager.AppSettings.Get("consumerPort"));
            var consumerIp = ConfigurationManager.AppSettings.Get("consumerIp");
            consumerEndPoint = new IPEndPoint(IPAddress.Parse(consumerIp), consumerPort);
            Console.WriteLine($"Sending packets to: {consumerEndPoint}");
            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            VideoCaptureDevice videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += VideoSource_NewFrame;
            videoSource.Start();
        }

        private static void VideoSource_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            var bmp = new Bitmap(eventArgs.Frame, 1024, 768);
            try
            {
                using (var ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Jpeg);
                    var bytes = ms.ToArray();
                    udpClient.Send(bytes, bytes.Length, consumerEndPoint);
                }

            }
            catch (Exception)
            {
                //Continue sending frames
            }
        }
    }

}