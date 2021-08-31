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
            ChannelName.PlaceholderText = Data.Channel;
            Data.LoadSettings();
        }

        private void ChannelName_TextChanged(object sender, TextChangedEventArgs e)
        {
            Data.Channel = ChannelName.Text;
            Data.SaveSettings();
        }
    }
}
