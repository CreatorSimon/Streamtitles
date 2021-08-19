using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Streamtitles
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DatabaseOperations : Page
    {
        public DatabaseOperations()
        {
            this.InitializeComponent();
        }

        private void GetButtonClick(object sender, RoutedEventArgs e)
        {
            if (Data.TryTwitchConnection())
            {
                Data.GetAllGames();
                TwitchFailText.Visibility = Visibility.Collapsed;
                TwitchSuccessText.Visibility = Visibility.Visible;
            }
            else
            {
                TwitchSuccessText.Visibility = Visibility.Collapsed;
                TwitchFailText.Visibility = Visibility.Visible;
            }
        }

        private void GetCurrentButtonClick(object sender, RoutedEventArgs e)
        {
            if(Data.TryTwitchConnection())
            {
                Data.GetCurrentTitleAndCategory();
                CurrentTitle.Text = Data.CurrentTitle;
                TwitchFailText.Visibility = Visibility.Collapsed;
                TwitchSuccessText.Visibility = Visibility.Visible;
            }
            else
            {
                TwitchSuccessText.Visibility = Visibility.Collapsed;
                TwitchFailText.Visibility = Visibility.Visible;
            }
        }

        private void SaveCurrentButtonClick(object sender, RoutedEventArgs e)
        {
            ListView temp = new ListView();
            CategoryEntry entry = new CategoryEntry();
            entry.GameID = Data.CurrentGame;
            entry.Name = Data.CurrentTitle;
            temp.Items.Add(entry);
            if (Data.TrySqlConnection())
            {
                Data.SaveToDatabase(Data.CurrentTitle, "", temp);
                SqlFailText.Visibility = Visibility.Collapsed;
                SqlSuccessText.Visibility = Visibility.Visible;
            }
            else
            {
                SqlSuccessText.Visibility = Visibility.Collapsed;
                SqlFailText.Visibility = Visibility.Visible;
            }
        }
    }
}
