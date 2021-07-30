using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Streamtitels
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OptionsPage : Page
    {

        public OptionsPage()
        {
            this.InitializeComponent();
            if(Data._ip != null)
            {
                IP_In.PlaceholderText = Data._ip;
            }
            if (Data._port != null)
            {
                Port_In.PlaceholderText = Data._port;
            }
            if (Data._user != null)
            {
                User_In.PlaceholderText = Data._user;
            }
        }

        private void IP_In_TextChanged(object sender, TextChangedEventArgs e)
        {
            Data._ip = IP_In.Text;
        }

        private void User_In_TextChanged(object sender, TextChangedEventArgs e)
        {
            Data._user = User_In.Text;
        }

        private void Password_In_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Data._password = Password_In.Password;
        }

        private void Port_In_TextChanged(object sender, RoutedEventArgs e)
        {
            Data._port = Port_In.Text;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            Data.Generate_ConnectionString();
        }
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if(Data.Connect_sql())
            {
                ConnectButton.IsEnabled = false;

                // Capture the current text ("ABC" in your example)
                FailedText.Visibility = Visibility.Collapsed;

                // Create a background worker to sleep for 2 seconds...
                var backgroundWorker = new BackgroundWorker();
                backgroundWorker.DoWork += (s, ea) => Thread.Sleep(TimeSpan.FromSeconds(2));

                // ...and then set the text back to the original when the sleep is done
                // (also, re-enable the button)
                backgroundWorker.RunWorkerCompleted += (s, ea) =>
                {
                    ConnectedText.Visibility = Visibility.Collapsed;
                    ConnectButton.IsEnabled = true;
                };

                // Color c = (Color)ColorConverter.ConvertFromString("Green");
                ConnectedText.Visibility = Visibility.Visible;

                // Start the background worker
                backgroundWorker.RunWorkerAsync();
            }
            else
            {
                ConnectButton.IsEnabled = false;

                var backgroundWorker = new BackgroundWorker();
                backgroundWorker.DoWork += (s, ea) => Thread.Sleep(TimeSpan.FromSeconds(2));

                backgroundWorker.RunWorkerCompleted += (s, ea) =>
                {
                    FailedText.Visibility = Visibility.Collapsed;
                    ConnectButton.IsEnabled = true;
                };

                FailedText.Visibility = Visibility.Visible;

                backgroundWorker.RunWorkerAsync();
            }
        }
    }
}
