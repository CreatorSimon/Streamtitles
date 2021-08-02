
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MySql.Data.MySqlClient;
using System.Data.Common;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Streamtitles
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 




    public sealed partial class MainPage : Page
    {
        private MySqlCommand readTitle;

        public MainPage()
        {
            this.InitializeComponent();

            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(500, 500));

            //CheckConnection();
        }

        private async void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            if (Data.mysqlcon != null)
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
            else
            {
                StreamOut.PlaceholderText = "No connection to database!";
            }
        }

        private void CheckConnection()
        {
            var twCon = Data.CheckTwitchConnection();
            if(Data.mysqlcon != null && twCon)
            {
                StreamOut.PlaceholderText = "Database and Twitch connection are working!" ;
            }
            else if(Data.mysqlcon != null && !twCon)
            {
                StreamOut.PlaceholderText = "Twitch connection not working!";
            }else if(Data.mysqlcon == null && twCon)
            {
                StreamOut.PlaceholderText = "Database connection not working!";
            }
            else
            {
                StreamOut.PlaceholderText = "Database and Twitch connection not working!";
            }
        }

        private void StreamOut_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void SetButton_Click(object sender, RoutedEventArgs e)
        {
            Data.Change_Twitch_Title(StreamOut.Text);
        }


    }
}
