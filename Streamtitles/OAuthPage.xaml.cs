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
        const string clientID = "24hhvenpc25ymrrjcj6t9eva2heo6n";
        const string clientSecret = "caqxzplx2ncar6433oiljkczxu65i6";
        const string redirectURI = "http://localhost:50450/";
        const string authorizationEndpoint = "https://id.twitch.tv/oauth2/authorize";
        const string refreshEndpoint = "https://id.twitch.tv/oauth2/token--data-urlencode?grant_type=refresh_token";
        const string tokenEndpoint = "https://id.twitch.tv/oauth2/token";
        const string userInfoEndpoint = "https://www.googleapis.com/oauth2/v3/userinfo";

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
            string id_token = randomDataBase64url(32);
            string access_token = randomDataBase64url(32);
            string code_verifier = randomDataBase64url(32);

            // Stores the state and code_verifier values into local settings.
            // Member variables of this class may not be present when the app is resumed with the
            // authorization response, so LocalSettings can be used to persist any needed values.
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["id_token"] = id_token;
            localSettings.Values["code"] = access_token;

            // Creates the OAuth 2.0 authorization request.
            string authorizationRequest = string.Format("{0}?client_id={2}&redirect_uri={1}&response_type=code&scope=channel:manage:broadcast+openid",
                authorizationEndpoint,
                System.Uri.EscapeDataString(redirectURI),
                clientID
                );

            output("Opening authorization request URI: " + authorizationRequest);

            var listener = new System.Net.Http.HttpListener(IPAddress.Loopback, 50450);
            listener.Request += async (s, context) => {
                var request = context.Request;
                var response = context.Response;
                if (request.HttpMethod == HttpMethods.Get)
                {
                    await response.WriteContentAsync($"Hello from Server at: {DateTime.Now}\r\n");
                }
                else
                {
                    response.MethodNotAllowed();
                }
                // Close the HttpResponse to send it back to the client.
                response.Close();
                listener.Close();
            };
            listener.Start();

            var view = new WebView();
            Test.Navigate(new Uri("https://twitch.tv/dunkingsimon"));
        }

        /// <summary>
        /// Processes the OAuth 2.0 Authorization Response
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Uri)
            {
                // Gets URI from navigation parameters.
                Uri authorizationResponse = (Uri)e.Parameter;
                string queryString = authorizationResponse.Query;
                output("MainPage received authorizationResponse: " + authorizationResponse);

                // Parses URI params into a dictionary
                // ref: http://stackoverflow.com/a/11957114/72176
                Dictionary<string, string> queryStringParams =
                        queryString.Substring(1).Split('&')
                             .ToDictionary(c => c.Split('=')[0],
                                           c => Uri.UnescapeDataString(c.Split('=')[1]));


                // Authorization Code is now ready to use!
                output(Environment.NewLine + "Authorization code: " + queryStringParams.GetValueOrDefault("code"));
            }
            else
            {
                Debug.WriteLine(e.Parameter);
            }
        }

        async void performCodeExchangeAsync(string code, string code_verifier)
        {
            // Builds the Token request
            //string tokenRequestBody = string.Format("code={0}&redirect_uri={1}&client_id={2}&code_verifier={3}&scope=&grant_type=authorization_code",
            //    code,
            //    System.Uri.EscapeDataString(redirectURI),
            //    clientID,
            //    code_verifier
            //    );
            //StringContent content = new StringContent(tokenRequestBody, Encoding.UTF8, "application/x-www-form-urlencoded");

            //// Performs the authorization code exchange.
            //HttpClientHandler handler = new HttpClientHandler();
            //handler.AllowAutoRedirect = true;
            //HttpClient client = new HttpClient(handler);

            //output(Environment.NewLine + "Exchanging code for tokens...");
            //HttpResponseMessage response = await client.PostAsync(tokenEndpoint, content);
            //string responseString = await response.Content.ReadAsStringAsync();
            //output(responseString);

            //if (!response.IsSuccessStatusCode)
            //{
            //    output("Authorization code exchange failed.");
            //    return;
            //}

            //// Sets the Authentication header of our HTTP client using the acquired access token.
            //JsonObject tokens = JsonObject.Parse(responseString);
            //string accessToken = tokens.GetNamedString("access_token");
            //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            //// Makes a call to the Userinfo endpoint, and prints the results.
            //output("Making API Call to Userinfo...");
            //HttpResponseMessage userinfoResponse = client.GetAsync(userInfoEndpoint).Result;
            //string userinfoResponseContent = await userinfoResponse.Content.ReadAsStringAsync();
            //output(userinfoResponseContent);
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
    }
}
