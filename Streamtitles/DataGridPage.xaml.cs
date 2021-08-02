﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    

    public class DatabaseListEntry
    {
        public int IDTitle { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public string Category { get; set; }

    }



    public sealed partial class DataGridPage : Page
    {

        private MySqlCommand _GetTableTitle;
        
        public List<DatabaseListEntry> DataList { get; set; }

        public DataGridPage()
        {
            this.InitializeComponent();

            DataList = new List<DatabaseListEntry>();
            GetData();
        }

        public async void GetData()
        {
            if (Data.mysqlcon != null)
            {
                Data.mysqlcon.Open();
                _GetTableTitle = new MySqlCommand("SELECT titles.idtitle, titles.title, genres.genre, categories.category FROM titles, genres, categories WHERE(titles.idtitle = genres.idtitle and titles.idtitle = categories.idtitle);", Data.mysqlcon);
                using (DbDataReader res = await _GetTableTitle.ExecuteReaderAsync())
                {
                    while(await res.ReadAsync())
                    {

                        var entry = new DatabaseListEntry();
                        entry.IDTitle = res.GetInt32(0);
                        entry.Title = res.GetString(1);
                        entry.Genre = res.GetString(2);
                        entry.Category = res.GetString(3);

                        DataList.Add(entry);
                    }
                }
                Data.mysqlcon.Close();
            }
            else
            {

            }

    }

        private void sortCategory_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void sortGenre_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
