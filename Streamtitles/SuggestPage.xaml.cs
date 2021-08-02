using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
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

                addToDatabaseGenre = new MySqlCommand("INSERT INTO genres (idtitle, genre) VALUES(@titleid, @genre)", Data.mysqlcon);
                addToDatabaseGenre.Parameters.AddWithValue("@genre", Genre.Text);
                addToDatabaseGenre.Parameters.AddWithValue("@titleid", titleid);
                addToDatabaseGenre.Prepare();
                await addToDatabaseGenre.ExecuteNonQueryAsync();

                addToDatabaseCategory = new MySqlCommand("INSERT INTO categories (idtitle, category) VALUES(@titleid, @category)", Data.mysqlcon);
                addToDatabaseCategory.Parameters.AddWithValue("@titleid", titleid);
                addToDatabaseCategory.Parameters.AddWithValue("@category", Category.Text);
                addToDatabaseCategory.Prepare();
                await addToDatabaseCategory.ExecuteNonQueryAsync();

                Data.mysqlcon.Close();
            }
            else
            {
                Title.PlaceholderText = "No connection to database!";
            }
        }
    }
}
