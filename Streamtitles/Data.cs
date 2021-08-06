using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.Channels.ModifyChannelInformation;
using TwitchLib.Api.Helix.Models.Games;
using Windows.Storage;

namespace Streamtitles
{

    class Data
    {
        private static string _conStr;
        public static MySqlConnection mysqlcon;

        private static TwitchAPI api;

        private static ApplicationDataContainer localSettings;

        public static string Ip
        { get; set; }
        public static string Port
        { get; set; }
        public static string User
        { get; set; }
        public static string Password
        { get; set; }
        public static string ClientID
        { get; set; }
        public static string Secret
        { get; set; }
        public static string Token
        { get; set; }
        public static string Title
        { get; set; }
        public static string Game
        { get; set; }
        public static string Channel
        { get; set; }
      

        public object TwitchApiAddress { get; private set; }

        static Data()
        {
            localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            api = new TwitchAPI();
        }

        public static void Generate_ConnectionString()
        {
            _conStr = $"server = {Ip}; port = {Port}; user = {User}; pwd = {Password}; database = streamtitles;";
        }

        public static void ChangeTwitchTitleAndCategory(string title, string category)
        {
            Title = title;
            Game = category;
            api.Helix.Settings.ClientId = ClientID;
            api.Helix.Settings.Secret = Secret;
            api.Helix.Settings.AccessToken = Token;

            api.Helix.Settings.Scopes = new List<AuthScopes>() { AuthScopes.Any, AuthScopes.Helix_Channel_Manage_Broadcast };
            BackgroundWorker d = new BackgroundWorker();
            d.DoWork += async (a, s) =>
            {
                var user = await api.Helix.Users.GetUsersAsync(logins: new List<string>() { Channel });

                var t = new ModifyChannelInformationRequest();
                t.Title = Title;
                t.GameId = Game;
                await api.Helix.Channels.ModifyChannelInformationAsync(user.Users[0].Id, t);
            };

            d.RunWorkerAsync();
        }

        public static bool CheckTwitchConnection()
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

        public static void GetAllGames()
        {
            BackgroundWorker d = new BackgroundWorker();
            d.DoWork += async (a, s) =>
            {
                var subscription =
                    await api.Helix.Games.GetTopGamesAsync(first: 100);
                Debug.WriteLine(subscription.Data[0].Id);

                string[] values = new string[100];
                string result = "INSERT IGNORE INTO categories (gameid, name) VALUES ";
                for (int i = 0; i < 100; i++)
                {
                    values[i] = "(" + subscription.Data[i].Id + ", '" + subscription.Data[i].Name.Replace("'", "") + "')";
                }
                MySqlCommand fillCategories = new MySqlCommand(result + string.Join(", ", values) + ";", mysqlcon);
                mysqlcon.Open();
                fillCategories.Prepare();
                fillCategories.ExecuteNonQuery();
                mysqlcon.Close();
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
