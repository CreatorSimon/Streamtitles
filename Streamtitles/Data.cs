using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.Channels.ModifyChannelInformation;
using Windows.Storage;

namespace Streamtitles
{
    class Data
    {
        private static string _conStr;
        private static MainPage _mainPage = new MainPage();
        public static MySqlConnection mysqlcon;

        private static string _title;
        private static string _channel;
        private static TwitchAPI api = new TwitchAPI();

        public static string _ip;
        public static string _port;
        public static string _user;
        public static string _password;

        public static string _clientid;
        public static string _secret;
        public static string _token;

        private static ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

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

        public static string ClientID
        {
            get { return _clientid; }
            set { _clientid = value; }
        }

        public static string Secret
        {
            get { return _secret; }
            set { _secret = value; }
        }

        public static string Token
        {
            get { return _token; }
            set { _token = value; }
        }
      

        public object TwitchApiAddress { get; private set; }

        public static void Generate_ConnectionString()
        {
            _conStr = $"server = {Ip}; port = {Port}; user = {User}; pwd = {Password}; database = streamtitles;";
        }

        public static void Change_Twitch_Title(string title)
        {
            _title = title;
            api.Helix.Settings.ClientId = ClientID;
            api.Helix.Settings.Secret = Secret;
            api.Helix.Settings.AccessToken = Token;

            api.Helix.Settings.Scopes = new List<AuthScopes>() { AuthScopes.Any, AuthScopes.Helix_Channel_Manage_Broadcast };
            BackgroundWorker d = new BackgroundWorker();
            d.DoWork += async (a, s) =>
            {
                var user = await api.Helix.Users.GetUsersAsync(logins: new List<string>() { Channel });

                var t = new ModifyChannelInformationRequest();
                t.Title = _title;
                await api.Helix.Channels.ModifyChannelInformationAsync(user.Users[0].Id, t);
                var subscription =
                    await api.Helix.Channels.GetChannelInformationAsync(user.Users[0].Id);
            };

            d.RunWorkerAsync();
        }

        public static Boolean CheckTwitchConnection()
        {
            try
            {
                api.Helix.Settings.ClientId = ClientID;
                api.Helix.Settings.Secret = Secret;
                api.Helix.Settings.AccessToken = Token;

                api.Helix.Settings.Scopes = new List<AuthScopes>() { AuthScopes.Any, AuthScopes.Helix_Channel_Manage_Broadcast };
                BackgroundWorker d = new BackgroundWorker();
                d.DoWork += async (a, s) =>
                {
                    var user = await api.Helix.Users.GetUsersAsync(logins: new List<string>() { Channel });
                    var subscription =
                        await api.Helix.Channels.GetChannelInformationAsync(user.Users[0].Id);
                };

                d.RunWorkerAsync();

            }
            catch (Exception e) when (e is TwitchLib.Api.Core.Exceptions.InvalidCredentialException || e is TwitchLib.Api.Core.Exceptions.BadRequestException)
            {
                return false;
            }
            return true;
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

        public static void Save_Settings()
        {
            localSettings.Values["database ip"] = Ip;
            localSettings.Values["database port"] = Port;
            localSettings.Values["database user"] = User;
            localSettings.Values["database pwd"] = Password;

            localSettings.Values["api clientid"] = ClientID;
            localSettings.Values["api secret"] = Secret;
            localSettings.Values["api token"] = Token;

            localSettings.Values["channel"] = Channel;
        }

        public static void Load_Settings()
        {
            Ip = localSettings.Values["database ip"] as string;
            Port = localSettings.Values["database port"] as string;
            User = localSettings.Values["database user"] as string;
            Password = localSettings.Values["database pwd"] as string;

            ClientID = localSettings.Values["api clientid"] as string;
            Secret = localSettings.Values["api secret"] as string;
            Token = localSettings.Values["api token"] as string;

            Channel = localSettings.Values["channel"] as string;
        }

    }
}
