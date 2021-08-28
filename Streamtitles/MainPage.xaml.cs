
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
        public List<string> Categories { get; set; } = new List<string>();

        public MainPage()
        {
            this.InitializeComponent();


            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(500, 500));
            Data.LoadSettings();
            Data.GetAllCategories();
            Data.GetUsedCategories(Categories);
            if (CategoryChangeBox.SelectedIndex == -1)
            {
                CategoryChangeBox.SelectedIndex = 0;
            }
        }

        private void GenerateButtonClick(object sender, RoutedEventArgs e)
        {
            Data.GenerateTitle(CategoryChangeBox.SelectedItem.ToString());
            StreamOut.Text = Data.CurrentTitle;
        }

        private void StreamOutTextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void SetButtonClick(object sender, RoutedEventArgs e)
        {
            Data.ChangeTwitchTitleAndCategory(StreamOut.Text, Data.CurrentGame);
        }

    }
}
