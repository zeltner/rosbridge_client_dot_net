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
    /// Interaction logic for ServiceClientWindow.xaml
    /// </summary>
    public partial class ServiceClientWindow : Window, IChildWindow
    {
        private ServiceClient _serviceClient;

        public ServiceClientWindow(ServiceClient serviceClient)
        {
            InitializeComponent();

            _serviceClient = serviceClient;
        }

        public Task CleanUp()
        {
            if (null != _serviceClient)
            {
                _serviceClient = null;
            }

            // we have to return a task, so what?
            return Task.Delay(0);
        }

        private async void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            await CleanUp();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ServiceLabel.Content = "Service: \"" + _serviceClient.Service.Replace("_", "__") + "\"";
        }

        private async void CallButton_Click(object sender, RoutedEventArgs e)
        {
            JArray argsList = JArray.Parse(ArgsTextBox.Text);

            var result = await _serviceClient.Call(argsList.ToObject<List<dynamic>>());

            Dispatcher.Invoke(() =>
            {
                try
                {
                    ResultListBox.Items.Add(result.ToString());
                }
                catch { }
            });
        }
    }
}
