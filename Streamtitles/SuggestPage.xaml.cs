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
        private MySqlCommand addToDatabaseTitle;
        private MySqlCommand getTitleID;
        private MySqlCommand addToDatabaseGenre;
        private MySqlCommand addToDatabaseCategory;

        private int titleid;


        public SuggestPage()
        {
            this.InitializeComponent();
            GetAllCategories();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (Data.mysqlcon != null)
            {
                Data.mysqlcon.Open();
                addToDatabaseTitle = new MySqlCommand("REPLACE INTO titles (title) VALUES (@title);", Data.mysqlcon);
                addToDatabaseTitle.Parameters.AddWithValue("@title", Title.Text);
                await addToDatabaseTitle.ExecuteNonQueryAsync();

                getTitleID = new MySqlCommand("SELECT idtitle FROM titles WHERE title = @title", Data.mysqlcon);
                getTitleID.Parameters.AddWithValue("@title", Title.Text);

                using (DbDataReader res = await getTitleID.ExecuteReaderAsync())
                {
                    await res.ReadAsync();
                    titleid = res.GetInt32(0);
                }

                string[] subs1 = Genre.Text.Split(',');
                string[] formed1 = new string[subs1.Length];
                string result1 = "INSERT INTO genres (idtitle, genre) VALUES ";

                for(int i = 0; i < subs1.Length; i++)
                {
                    formed1[i] = "( @titleid, " + "'" + subs1[i] + "'" + ")";
                }

                result1 = result1 + string.Join(", ", formed1);

                addToDatabaseGenre = new MySqlCommand(result1 + ";", Data.mysqlcon);
                //addToDatabaseGenre.Parameters.AddWithValue("@genre", Genre.Text);
                addToDatabaseGenre.Parameters.AddWithValue("@titleid", titleid);
                addToDatabaseGenre.Prepare();
                await addToDatabaseGenre.ExecuteNonQueryAsync();

                string[] subs = new string[EnabledCategories.Items.Count];
                int j = 0;
                foreach(CategoryEntry item in EnabledCategories.Items.ToList())
                {
                    subs[j] = item.GameID;
                    j++;
                }
                string[] formed = new string[subs.Length];
                string result = "INSERT INTO ct_intersect (idtitle, gameid) VALUES ";

                for (int i = 0; i < subs.Length; i++)
                {
                    formed[i] = "( @titleid, " + subs[i] + ")";
                }

                result = result + string.Join(", ", formed);

                Debug.WriteLine(result);
                addToDatabaseCategory = new MySqlCommand(result + ";", Data.mysqlcon);
                addToDatabaseCategory.Parameters.AddWithValue("@titleid", titleid);
                addToDatabaseCategory.Prepare();
                await addToDatabaseCategory.ExecuteNonQueryAsync();

                Data.mysqlcon.Close();
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
