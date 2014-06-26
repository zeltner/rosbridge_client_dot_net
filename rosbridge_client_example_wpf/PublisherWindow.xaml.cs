using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Rosbridge.Client;
using Newtonsoft.Json.Linq;

namespace rosbridge_client_example_wpf
{
    /// <summary>
    /// Interaction logic for PublisherWindow.xaml
    /// </summary>
    public partial class PublisherWindow : Window, IChildWindow
    {
        private Publisher _publisher;

        public PublisherWindow(Publisher publisher)
        {
            InitializeComponent();

            _publisher = publisher;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TopicLabel.Content = "Publish to \"" + _publisher.Topic.Replace("_", "__") + "\" (" + _publisher.Type.Replace("_", "__") + ")";

            await _publisher.AdvertiseAsync();
        }

        private async void PublishButton_Click(object sender, RoutedEventArgs e)
        {
            var obj = JObject.Parse(MessageTextBox.Text);

            await _publisher.PublishAsync(obj);
        }

        public async Task CleanUp()
        {
            if (null != _publisher)
            {
                await _publisher.UnadvertiseAsync();
                _publisher = null;
            }
        }

        private async void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            await CleanUp();
        }
    }
}
