using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
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

     
        
        public List<DatabaseListEntry> DataList { get; set; }

        public DataGridPage()
        {
            this.InitializeComponent();

            DataList = new List<DatabaseListEntry>();
            Data.GetData(DataList);
        }

        private void SearchBoxTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (SwitchSeachIndex.SelectedValue.Equals("Category"))
            {
                DataGrid.ItemsSource = new ObservableCollection<DatabaseListEntry>(from item in DataList where item.Category.ToLower().Contains(SearchBox.Text.ToLower()) select item);
            }else if(SwitchSeachIndex.SelectedValue.Equals("Genre"))
            {
                DataGrid.ItemsSource = new ObservableCollection<DatabaseListEntry>(from item in DataList where item.Genre.ToLower().Contains(SearchBox.Text.ToLower()) select item);
            }
        }
    }
}
