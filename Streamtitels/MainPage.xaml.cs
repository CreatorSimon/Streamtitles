using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;
using System.Data.SQLite;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;
using Windows.ApplicationModel.DataTransfer;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Streamtitels
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 




    public sealed partial class MainPage : Page
    {
        private MySqlCommand addToDatabase;
        private MySqlCommand readTitle;
        private DataPackage dataPackage;

        public MainPage()
        {
            this.InitializeComponent();
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(500, 500));

            dataPackage = new DataPackage();
        }

        private double ConvertPixelsToDips(int v)
        {
            throw new NotImplementedException();
        }

        private async void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            Data.mysqlcon.Open();
            readTitle = new MySqlCommand("SELECT title FROM titles ORDER BY RAND() LIMIT 1", Data.mysqlcon);
            using (DbDataReader res = await readTitle.ExecuteReaderAsync())
            {
                await res.ReadAsync();
                StreamOut.Text = res.GetString(0);
            }
            Data.mysqlcon.Close();
        }

        private void SuggestButton_Click(object sender, RoutedEventArgs e)
        {
            Data.mysqlcon.Open();
            addToDatabase = new MySqlCommand("REPLACE INTO titles (title) VALUES (@title);", Data.mysqlcon);
            addToDatabase.Parameters.AddWithValue("@title", StreamOut.Text);
            addToDatabase.Prepare();
            addToDatabase.ExecuteNonQueryAsync();
            Data.mysqlcon.Close();
        }

        private void StreamOut_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            dataPackage.SetText(StreamOut.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(OptionsPage));
        }
    }
}
