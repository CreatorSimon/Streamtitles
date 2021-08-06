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
            if (Data.CheckTwitchConnection())
            {
                Get_Button.IsEnabled = true;
                FailText.Visibility = Visibility.Collapsed;
                SuccessText.Visibility = Visibility.Visible;
            }
            else
            {
                Get_Button.IsEnabled = false;
                SuccessText.Visibility = Visibility.Collapsed;
                FailText.Visibility = Visibility.Visible;
            }
        }

        private void Get_Button_Click(object sender, RoutedEventArgs e)
        {
            Data.GetAllGames();
        }
    }
}
