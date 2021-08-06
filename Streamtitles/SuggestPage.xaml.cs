using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
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

namespace Streamtitles
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 

    class CategoryEntry
    {
        public string Name { get; set; }
        public string GameID { get; set; }
    }

    public sealed partial class SuggestPage : Page
    {
        public SuggestPage()
        {
            this.InitializeComponent();
            GetAllCategories();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (Data.mysqlcon != null)
            {
                Data.SaveToDatabase(Title.Text, Genre.Text, EnabledCategories);
            }
            else
            {
                Title.PlaceholderText = "No connection to database!";
            }
        }



        private async void GetAllCategories()
        {
            MySqlCommand _getAllCategories = new MySqlCommand("SELECT name, gameid FROM categories ORDER BY name ASC;", Data.mysqlcon);
            Data.mysqlcon.Open();
            using (DbDataReader res = await _getAllCategories.ExecuteReaderAsync())
            {
                while (await res.ReadAsync())
                {
                    var entry = new CategoryEntry();
                    entry.Name = res.GetString(0);
                    entry.GameID = res.GetString(1);

                    DisabledCategories.Items.Add(entry);
                }
            }
            Data.mysqlcon.Close();
        }

        private void DisabledCategories_ItemClick(object sender, ItemClickEventArgs e)
        {
            if(e.ClickedItem != null)
            {
                EnabledCategories.Items.Add(e.ClickedItem);
                DisabledCategories.Items.Remove(e.ClickedItem);

                //List<string> items = EnabledCategories.Items.Cast<string>().ToList().OrderBy(i => i).ToList();
                //EnabledCategories.Items.

                //EnabledCategories.Items.Clear();
                //items.ForEach(i => EnabledCategories.Items.Add(i));

            }
        }

        private void EnabledCategories_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem != null)
            {
                DisabledCategories.Items.Add(e.ClickedItem);
                EnabledCategories.Items.Remove(e.ClickedItem);

                //List<string> items = DisabledCategories.Items.Cast<string>().ToList().OrderBy(i => i).ToList();

                //DisabledCategories.Items.Clear();
                //items.ForEach(i => DisabledCategories.Items.Add(i));
            }
        }
    }
}
