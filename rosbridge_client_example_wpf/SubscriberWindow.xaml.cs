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

namespace rosbridge_client_example_wpf
{
    /// <summary>
    /// Interaction logic for SubscriberWindow.xaml
    /// </summary>
    public partial class SubscriberWindow : Window, IChildWindow
    {
        private Subscriber _subscriber;

        public SubscriberWindow(Subscriber subscriber)
        {
            InitializeComponent();

            _subscriber = subscriber;
        }

        public async Task CleanUp()
        {
            if (null != _subscriber)
            {
                _subscriber.MessageReceived -= _subscriber_MessageReceived;
                await _subscriber.UnsubscribeAsync();
                _subscriber = null;
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TopicLabel.Content = "Subscribe to \"" + _subscriber.Topic.Replace("_", "__") + "\" (" + _subscriber.Type.Replace("_", "__") + ")";

            _subscriber.MessageReceived += _subscriber_MessageReceived;
            await _subscriber.SubscribeAsync();
        }

        private void _subscriber_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var message = e.Message;
            string tmp = e.Message["msg"].ToString();

            Dispatcher.Invoke(() =>
            {
                try
                {
                    MessagesListBox.Items.Add(tmp);
                }
                catch { }
            });
        }

        private async void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            await CleanUp();
        }
    }
}
