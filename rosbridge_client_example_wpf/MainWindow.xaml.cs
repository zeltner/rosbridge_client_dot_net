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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Rosbridge.Client;

namespace rosbridge_client_example_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MessageDispatcher _md;
        private List<SubscriberWindow> _subscriberWindows = new List<SubscriberWindow>();
        private bool _isConnected = false;

        public MainWindow()
        {
            InitializeComponent();

            ToggleConnected();
        }

        private void ToggleConnected()
        {
            if (_isConnected)
            {
                ConnectButton.Content = "Disconnect";
                URITextBox.IsEnabled = false;
                PublishGroupBox.IsEnabled = true;
                SubscribeGroupBox.IsEnabled = true;
                ServicesGroupBox.IsEnabled = true;
            }
            else
            {
                ConnectButton.Content = "Connect";
                URITextBox.IsEnabled = true;
                PublishGroupBox.IsEnabled = false;
                SubscribeGroupBox.IsEnabled = false;
                ServicesGroupBox.IsEnabled = false;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void StartSubscriberButton_Click(object sender, RoutedEventArgs e)
        {
            var subscriberWindow = new SubscriberWindow(new Subscriber(TopicTextBox.Text, TypeTextBox.Text, _md));
            subscriberWindow.Show();

            _subscriberWindows.Add(subscriberWindow);
        }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isConnected)
            {
                foreach (var w in _subscriberWindows)
                {
                    await w.CleanUp();
                    w.Close();
                }
                _subscriberWindows.Clear();

                await _md.StopAsync();
                _md = null;

                _isConnected = false;
            }
            else
            {
                try
                {
                    _md = new MessageDispatcher(new Socket(new Uri(URITextBox.Text)), new MessageSerializerV2_0());
                    await _md.StartAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,
                         "Error!! Could not connect to the rosbridge server", MessageBoxButton.OK, MessageBoxImage.Error);
                    _md = null;
                    return;
                }

                _isConnected = true;
            }

            ToggleConnected();
        }
    }
}
