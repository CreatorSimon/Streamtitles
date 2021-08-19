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
            Data.LoadSettings();
            if (Data.Channel != null)
            {
                ChannelIn.PlaceholderText = Data.Channel;
            }
        }

        private void ClientIDInTextChanged(object sender, TextChangedEventArgs e)
        {
            Data.ClientID = ClientIDIn.Text;
        }

        private void SecretInTextChanged(object sender, RoutedEventArgs e)
        {
            Data.Secret = SecretIn.Text;
        }

        private void TokenInTextChanged(object sender, RoutedEventArgs e)
        {
            Data.Token = TokenIn.Text;
        }

        private void ChannelInTextChanged(object sender, RoutedEventArgs e)
        {
            Data.Channel = ChannelIn.Text;
        }

        private void ApplyButtonClick(object sender, RoutedEventArgs e)
        {
            Data.SaveSettings();
            Data.ChangeApiCredentials();

        }
        private void ConnectButtonClick(object sender, RoutedEventArgs e)
        {
            if (Data.TrySqlConnection())
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
