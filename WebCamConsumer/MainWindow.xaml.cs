using System.Configuration;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;


namespace WebCamConsumer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
      
        private async void Image_Loaded(object sender, RoutedEventArgs e)
        {
            int port;
            int.TryParse(ConfigurationManager.AppSettings.Get("port"),out port);

            using (UdpClient client = new UdpClient(port))
            {  
                double size = 0;
                while (size!= double.MaxValue)
                {
                    var data = await client.ReceiveAsync();
                    using (MemoryStream ms = new MemoryStream(data.Buffer))
                    {
                        BitmapImage bitmapimage = new BitmapImage();
                        bitmapimage.BeginInit();
                        bitmapimage.StreamSource = ms;
                        bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapimage.EndInit();
                        Image.Source = bitmapimage;
                    }
                   
                    size += data.Buffer.Length / 1048576d;
                    Title = $"Bytes recieved: {size.ToString("0.0")}";
                }
            }
        } 
    }
}
