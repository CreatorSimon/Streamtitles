using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.ApplicationModel;
using System.Diagnostics;
using System.Text;
using Windows.Data.Json;
using Windows.Storage.Streams;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net;
using Windows.Data.Json;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Streamtitles
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OAuthPage : Page
    {

        /// <summary>
        /// OAuth 2.0 client configuration.
        /// </summary>
        const string redirectURI = "https://link.dunkingsimon.de/";
        const string authorizationEndpoint = "https://id.twitch.tv/oauth2/authorize";
        const string refreshEndpoint = "https://id.twitch.tv/oauth2/token--data-urlencode?grant_type=refresh_token";
        const string tokenEndpoint = "https://id.twitch.tv/oauth2/token";
        const string validateEndpoint = "https://id.twitch.tv/oauth2/validate";
        const string userInfoEndpoint = "https://www.googleapis.com/oauth2/v3/userinfo";

        private WebView view = new WebView();

        public OAuthPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Starts an OAuth 2.0 Authorization Request.
        /// </summary>
        private async void button_Click(object sender, RoutedEventArgs e)
        {
            // Generates state and PKCE values.

            // Stores the state and code_verifier values into local settings.
            // Member variables of this class may not be present when the app is resumed with the
            // authorization response, so LocalSettings can be used to persist any needed values.

            // Creates the OAuth 2.0 authorization request.
            string authorizationRequest = string.Format("{0}?client_id={2}&redirect_uri={1}&response_type=token&scope=channel:manage:broadcast+openid",
                authorizationEndpoint,
                System.Uri.EscapeDataString(redirectURI),
                Data.ClientID
                );

            output("Opening authorization request URI: " + authorizationRequest);

            Web.NavigationStarting += GetResponse;
            Web.ContentLoading += Failed;
            Web.Navigate(new Uri(authorizationRequest));
        }

        private void GetResponse(object sender, WebViewNavigationStartingEventArgs args)
        {
            var url = args.Uri;
            if (url.IsLoopback && url.Fragment.Contains("access_token"))
            {
                var response = url.Fragment;
                var queryStringParams = response.Substring(1).Split('&').ToDictionary(c => c.Split('=')[0], c => Uri.UnescapeDataString(c.Split('=')[1]));

                Data.Token = queryStringParams["access_token"];
                Data.SaveSettings();
            }
        }

        private void Failed(object sender, WebViewContentLoadingEventArgs e)
        {
            Debug.WriteLine("Test");
            
        }

        /// <summary>
        /// Appends the given string to the on-screen log, and the debug console.
        /// </summary>
        /// <param name="output">string to be appended</param>
        public void output(string output)
        {
            //textBoxOutput.Text = textBoxOutput.Text + output + Environment.NewLine;
            Debug.WriteLine(output);
        }

        /// <summary>
        /// Returns URI-safe data with a given input length.
        /// </summary>
        /// <param name="length">Input length (nb. output will be longer)</param>
        /// <returns></returns>
        public static string randomDataBase64url(uint length)
        {
            IBuffer buffer = CryptographicBuffer.GenerateRandom(length);
            return base64urlencodeNoPadding(buffer);
        }

        /// <summary>
        /// Returns the SHA256 hash of the input string.
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static IBuffer sha256(string inputString)
        {
            HashAlgorithmProvider sha = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
            IBuffer buff = CryptographicBuffer.ConvertStringToBinary(inputString, BinaryStringEncoding.Utf8);
            return sha.HashData(buff);
        }

        /// <summary>
        /// Base64url no-padding encodes the given input buffer.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string base64urlencodeNoPadding(IBuffer buffer)
        {
            string base64 = CryptographicBuffer.EncodeToBase64String(buffer);

            // Converts base64 to base64url.
            base64 = base64.Replace("+", "-");
            base64 = base64.Replace("/", "_");
            // Strips padding.
            base64 = base64.Replace("=", "");

            return base64;
        }

        private async void Clear_Click(object sender, RoutedEventArgs e)
        {
            await WebView.ClearTemporaryWebDataAsync();
        }
    }
}
