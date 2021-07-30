using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.Channels.GetChannelInformation;
using TwitchLib.Api.Helix.Models.Channels.ModifyChannelInformation;
using TwitchLib.Api.Helix.Models.Users.GetUsers;

namespace Streamtitles
{
    class Data
    {
        private static string _conStr;
        private static MainPage _mainPage = new MainPage();
        public static MySqlConnection mysqlcon;

        private static string _title = "Test";
        private static string _channel = "dunkingsimon";
        private static TwitchAPI api = new TwitchAPI();

        public static string _ip;
        public static string _port;
        public static string _user;
        public static string _password;

        private static BackgroundWorker background = new BackgroundWorker();

        public static string Ip
        {
            get { return _ip; }
            set { _ip = value; }
        }

        public static string Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public static string User
        {
            get { return _user; }
            set { _user = value; }
        }

        public static string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public static string Channel
        {
            get { return _channel; }
            set { _channel = value; }
        }

        public static void Generate_ConnectionString()
        {
            _conStr = $"server = {_ip}; port = {_port}; user = {_user}; pwd = {_password}; database = streamtitles;";
        }

        public static void Change_Twitch_Title(string title)
        {
            _title = title;
            api.Helix.Settings.ClientId = SecretData.ClientID;
            api.Helix.Settings.Secret = SecretData.Secret;
            api.Helix.Settings.AccessToken = SecretData.Token;

            api.Helix.Settings.Scopes = new List<AuthScopes>() { AuthScopes.Any, AuthScopes.Helix_Channel_Manage_Broadcast };
            BackgroundWorker d = new BackgroundWorker();
            d.DoWork += async (a, s) =>
            {
                var user = await api.Helix.Users.GetUsersAsync(logins: new List<string>() { _channel });

                var t = new ModifyChannelInformationRequest();
                t.Title = _title;
                await api.Helix.Channels.ModifyChannelInformationAsync(user.Users[0].Id, t);
                var subscription =
                    await api.Helix.Channels.GetChannelInformationAsync(user.Users[0].Id);
            };

            d.RunWorkerAsync();
        }

        public static Boolean Connect_sql()
        {
            mysqlcon = new MySqlConnection(_conStr);
            try
            {
                mysqlcon.Open();
                mysqlcon.Close();
            }
            catch (Exception  e) when (e is MySqlException || e is InvalidOperationException)
            {
                mysqlcon = null;
                return false;
            }
            return true;
        }

        public static string Get_Connection_String()
        {
            return _conStr;
        }

        //public async Task<OAuthObject> GetAuthToken(bool isBroadcaster)
        //{
        //    //Source from: https:github.com/googlesamples/oauth-apps-for-windows/blob/master/OAuthDesktopApp/OAuthDesktopApp/MainWindow.xaml.cs
        //    //Creates an HttpListener to listen for requests on that redirect URI.

        //    var httpListener = new HttpListener();
        //    httpListener.Prefixes.Add("http://127.0.0.1:1234/");

        //    httpListener.Start();

        //    //Creates the OAuth 2.0 authorization request.
        //    var state = RandomDataBase64Url(32);
        //    var authorizationRequest = "";
        //    if (isBroadcaster)
        //    {
        //        authorizationRequest = $"{TwitchApiAddress}{AuthorizationEndpoint}?client_id={TwitchClientID}&redirect_uri={Uri.EscapeDataString(RedirectUri)}&response_type=code&{TwitchScopesChannelOwner}&force_verify=true&state={state}";
        //    }
        //    else
        //    {
        //        authorizationRequest = $"{TwitchApiAddress}{AuthorizationEndpoint}?client_id={TwitchClientID}&redirect_uri={Uri.EscapeDataString(RedirectUri)}&response_type=code&{TwitchScopesBot}&force_verify=true&state={state}";
        //    }

        //    //Opens request in the browser.
        //    Process.Start(authorizationRequest);

        //    //Waits for the OAuth authorization response.

        //    var context = await httpListener.GetContextAsync();

        //    //Brings this app back to the foreground.
        //    //Activate();

        //    //Sends an HTTP response to the browser.
        //    const string responseString = "<html><head><meta http-equiv='refresh' content='10;url=https:www.nanotwitchleafs.com/'></head><body>Please return to the app.</body></html>";
        //    var buffer = Encoding.UTF8.GetBytes(responseString);

        //    var response = context.Response;
        //    response.ContentLength64 = buffer.Length;
        //    var responseOutput = response.OutputStream;
        //    await responseOutput.WriteAsync(buffer, 0, buffer.Length).ContinueWith(task =>
        //    {
        //        responseOutput.Close();
        //        httpListener.Stop();
        //    });

        //    //Checks for errors.
        //    if (context.Request.QueryString.Get("error") != null)
        //    {
        //        _logger.Error($"OAuth authorization error: {context.Request.QueryString.Get("error")}.");
        //        return null;
        //    }

        //    if (context.Request.QueryString.Get("code") == null || context.Request.QueryString.Get("state") == null)
        //    {
        //        _logger.Error("Malformed authorization response. " + context.Request.QueryString);
        //        return null;
        //    }

        //    //extracts the code
        //    string code = context.Request.QueryString.Get("code");
        //    string incomingState = context.Request.QueryString.Get("state");

        //    //compares the receieved state to the expected value, to ensure that this app made the request which resulted in authorization.
        //    if (incomingState != state)
        //    {
        //        _logger.Error($"Received request with invalid state ({incomingState})");
        //        return null;
        //    }

        //    _logger.Debug("Authorization code: " + code);

        //    return await PerformCodeExchange(code);
        //}

        //private async Task<OAuthObject> PerformCodeExchange(string code, bool isRefresh = false)
        //{
        //    _logger.Info("Exchanging code for tokens...");
        //    string tokenRequestBody = "";

        //    if (!isRefresh)
        //    {
        //        //builds the  request
        //        tokenRequestBody = $"code={code}&redirect_uri={Uri.EscapeDataString(RedirectUri)}&client_id={TwitchClientID}&client_secret={TwitchClientSecret}&grant_type=authorization_code";
        //    }
        //    else
        //    {
        //        //builds the  request
        //        tokenRequestBody = $"refresh_token={code}&client_id={TwitchClientID}&client_secret={TwitchClientSecret}&grant_type=refresh_token";
        //    }

        //    //sends the request
        //    var tokenRequest = (HttpWebRequest)WebRequest.Create($"{TwitchApiAddress}{TokenEndpoint}");
        //    tokenRequest.Method = "POST";
        //    tokenRequest.ContentType = "application/x-www-form-urlencoded";
        //    tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
        //    tokenRequest.ProtocolVersion = HttpVersion.Version10;

        //    var byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
        //    tokenRequest.ContentLength = byteVersion.Length;
        //    var stream = tokenRequest.GetRequestStream();
        //    await stream.WriteAsync(byteVersion, 0, byteVersion.Length);
        //    stream.Close();

        //    try
        //    {
        //        //gets the response
        //        var tokenResponse = await tokenRequest.GetResponseAsync();
        //        using (var reader = new StreamReader(tokenResponse.GetResponseStream()))
        //        {
        //            //reads response body
        //            string responseText = await reader.ReadToEndAsync();
        //            _logger.Debug("Response: " + responseText);

        //            responseText = responseText.Remove(136) + "}";

        //            //converts to dictionary
        //            dynamic tokenEndpointDecoded = JsonConvert.DeserializeObject(responseText);

        //            return new OAuthObject { Access_Token = tokenEndpointDecoded["access_token"], Refresh_Token = tokenEndpointDecoded["refresh_token"], Expires_In = tokenEndpointDecoded["expires_in"] };
        //        }
        //    }
        //    catch (WebException ex)
        //    {
        //        if (ex.Status == WebExceptionStatus.ProtocolError)
        //        {
        //            if (ex.Response is HttpWebResponse response)
        //            {
        //                _logger.Debug("HTTP: " + response.StatusCode);
        //                using (var reader = new StreamReader(response.GetResponseStream()))
        //                {
        //                    //reads response body
        //                    string responseText = await reader.ReadToEndAsync();
        //                    _logger.Debug("Response: " + responseText);
        //                }
        //            }
        //        }
        //    }

        //    return null;
        //}

    }
}
