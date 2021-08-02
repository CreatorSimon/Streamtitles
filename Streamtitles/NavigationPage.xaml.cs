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

namespace Streamtitles
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NavigationPage : Page
    {
        private MainPage _mainPage;
        private OptionsPage _optionsPage;
        private SuggestPage _suggestPage;
        private DataGridPage _dataPage;

        public NavigationPage()
        {
            this.InitializeComponent();
            this.contentFrame.Navigate(typeof(MainPage));
            NavBar.IsPaneOpen = false;
            _mainPage = new MainPage();
            _optionsPage = new OptionsPage();
            _suggestPage = new SuggestPage();
            _dataPage = new DataGridPage();
            Data.Load_Settings();
        }

        private void NavBar_Click(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if(args.IsSettingsInvoked)
            {
                this.contentFrame.Navigate(typeof(OptionsPage));
            }
            if(args.InvokedItemContainer == Home_Page)
            {
                this.contentFrame.Navigate(typeof(MainPage));
            }
            if (args.InvokedItemContainer == Suggest_Page)
            {
                this.contentFrame.Navigate(typeof(SuggestPage));
            }
            if (args.InvokedItemContainer == Data_Page)
            {
                this.contentFrame.Navigate(typeof(DataGridPage));
            }
        }
    }
}
