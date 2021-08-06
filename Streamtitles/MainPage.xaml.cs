
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Diagnostics;
using System.Collections.Generic;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Streamtitles
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 

    public sealed partial class MainPage : Page
    {
        private MySqlCommand _readTitle;
        private MySqlCommand _getAllCategories;
        private string _currentGame;

        private List<string> Categories { get; set; }

        public MainPage()
        {
            this.InitializeComponent();

            Categories = new List<string>();

            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(500, 500));
            Data.Load_Settings();
            Data.Generate_ConnectionString();
            CheckConnection();
            GetAllCategories();
            if (CategoryChangeBox.SelectedIndex == -1)
            {
                CategoryChangeBox.SelectedIndex = 0;
            }
        }

        private async void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            if (Data.Connect_sql())
            {
                Data.mysqlcon.Open();
                _readTitle = new MySqlCommand("SELECT titles.title, categories.gameid FROM titles, categories, ct_intersect WHERE(titles.idtitle = ct_intersect.idtitle and @Category=categories.name and ct_intersect.gameid = categories.gameid) ORDER BY RAND() LIMIT 1;", Data.mysqlcon);
                _readTitle.Parameters.AddWithValue("@Category", CategoryChangeBox.SelectedItem);
                using (DbDataReader res = await _readTitle.ExecuteReaderAsync())
                {
                    await res.ReadAsync();
                    StreamOut.Text = res.GetString(0);
                    _currentGame = res.GetString(1);
                }
                Data.mysqlcon.Close();
            }
            else
            {
                StreamOut.PlaceholderText = "No connection to database!";
            }
        }

        private async void GetAllCategories()
        {
            Categories.Clear();
            _getAllCategories = new MySqlCommand("SELECT name FROM categories, ct_intersect WHERE(ct_intersect.gameid = categories.gameid) ORDER BY name ASC;", Data.mysqlcon);
            Data.mysqlcon.Open();
            using (DbDataReader res = await _getAllCategories.ExecuteReaderAsync())
            {
                while (await res.ReadAsync())
                {
                    Categories.Add(res.GetString(0));
                }
            }
            Data.mysqlcon.Close();
        }

        private void CheckConnection()
        {
            var twCon = Data.CheckTwitchConnection();
            var DataCon = Data.Connect_sql();
            if(DataCon && twCon)
            {
                StreamOut.PlaceholderText = "Database and Twitch connection are working!" ;
            }
            else if(DataCon && !twCon)
            {
                StreamOut.PlaceholderText = "Twitch connection not working!";
            }else if(!DataCon && twCon)
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
            Data.ChangeTwitchTitleAndCategory(StreamOut.Text, _currentGame);
        }


    }
}
