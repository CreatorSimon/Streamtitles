using System;
using System.ComponentModel;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Streamtitles
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OptionsPage : Page
    {

        public OptionsPage()
        {
            this.InitializeComponent();
            Data.Load_Settings();
            if(Data.Ip != null)
            {
                IP_In.PlaceholderText = Data.Ip;
            }
            if (Data.Port != null)
            {
                Port_In.PlaceholderText = Data.Port;
            }
            if (Data.User != null)
            {
                User_In.PlaceholderText = Data.User;
            }
            if (Data.Channel != null)
            {
                Channel_In.PlaceholderText = Data.Channel;
            }
        }

        private void IP_In_TextChanged(object sender, TextChangedEventArgs e)
        {
            Data.Ip = IP_In.Text;
        }

        private void User_In_TextChanged(object sender, TextChangedEventArgs e)
        {
            Data.User = User_In.Text;
        }

        private void Password_In_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Data.Password = Password_In.Password;
        }

        private void Port_In_TextChanged(object sender, RoutedEventArgs e)
        {
            Data.Port = Port_In.Text;
        }

        private void ClientID_In_TextChanged(object sender, TextChangedEventArgs e)
        {
            Data.ClientID = ClientID_In.Text;
        }

        private void Secret_In_TextChanged(object sender, RoutedEventArgs e)
        {
            Data.Secret = Secret_In.Text;
        }

        private void Token_In_TextChanged(object sender, RoutedEventArgs e)
        {
            Data.Token = Token_In.Text;
        }

        private void Channel_In_TextChanged(object sender, RoutedEventArgs e)
        {
            Data.Channel = Channel_In.Text;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            Data.Save_Settings();

        }
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            Data.Generate_ConnectionString();
            if (Data.Connect_sql())
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
