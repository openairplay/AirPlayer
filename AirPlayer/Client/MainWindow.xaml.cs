using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using AirPlayer;
using AirPlayer.AirPlay;
using AirPlayer.Utils;
using Microsoft.Win32;
using Zeroconf;

namespace Client
{
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<AirplayDevice> airplayDevices;
        private readonly MediaStreamingServer mediaStreamingServer;

        public MainWindow()
        {
            InitializeComponent();
            airplayDevices = new ObservableCollection<AirplayDevice>();
            airPlayDevices.ItemsSource = airplayDevices;
            var airPlayDiscovery = new AirPlayDiscovery();
            airPlayDiscovery.AirplayServiceFound += ServiceDiscoveryAirplayServiceFound;
            mediaStreamingServer = new MediaStreamingServer().Start();
        }

        public AirplayDevice SelectedDevice { get; set; }

        private void ServiceDiscoveryAirplayServiceFound(IZeroconfHost item)
        {
            //Checks if the device aldready is on the list
            if (airplayDevices.Count(x => x.IpAddress == item.IPAddress.ToString()) == 0)
            {
                airplayDevices.Add(new AirplayDevice(item.IPAddress, item.DisplayName));
            }
        }

        private void airPlayDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedDevice != null)
            {
                if (SelectedDevice.StreamingVideo)
                {
                    SelectedDevice.Stop = true;
                }
            }
            SelectedDevice = ((sender as ListBox).SelectedItem as AirplayDevice);
        }

        private void btnVideo_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedDevice == null) return;

            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                var videoThread =
                    new Thread(() => SelectedDevice.StartVideo(new Uri(openFileDialog.FileName), mediaStreamingServer))
                    {
                        IsBackground = true
                    };
                videoThread.Start();
            }
        }

        private void btnImage_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedDevice == null) return;

            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                SelectedDevice.ShowPhoto(File.ReadAllBytes(openFileDialog.FileName));
            }
        }

        private void btnStopVideo_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedDevice == null) return;

            if (SelectedDevice.StreamingVideo)
            {
                SelectedDevice.Stop = true;
            }

            if (SelectedDevice.StreamingVideo)
            {
                SelectedDevice.Stop = true;
            }
        }

        private void btnMkvToMp4_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                Conversion.ConvertMkvToMp4(openFileDialog.FileName);
            }
        }

        private void btnGetStreamLink_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() != true) return;

            var messageBoxText = mediaStreamingServer.Address + "/play?filePath=" + openFileDialog.FileName;
            if (MessageBox.Show(messageBoxText, "Click OK to add the link to clipboard") == MessageBoxResult.OK)
                Clipboard.SetText(messageBoxText);
        }
    }
}
