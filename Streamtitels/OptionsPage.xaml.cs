using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace Streamtitels
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OptionsPage : Page
    {
        private string _ip;
        private string _port;
        private string _user;
        private string _password;
        private string connStr;

        public OptionsPage()
        {
            this.InitializeComponent();
        }

        private void IP_In_TextChanged(object sender, TextChangedEventArgs e)
        {
            _ip = IP_In.Text;
        }

        private void User_In_TextChanged(object sender, TextChangedEventArgs e)
        {
            _user = User_In.Text;
        }

        private void Password_In_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _password = Password_In.Password;
        }

        private void Port_In_TextChanged(object sender, RoutedEventArgs e)
        {
            _port = Port_In.Text;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            Data.Generate_ConnectionString(_ip, _port, _user, _password);
        }
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            Data.Connect_sql();
        }
    }
}
