using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.Channels.ModifyChannelInformation;
using TwitchLib.Api.Helix.Models.Games;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace Streamtitles
{

    class Data
    {
        private static string _conStr;
        public static MySqlConnection mysqlcon;

        private static TwitchAPI api;

        private static ApplicationDataContainer localSettings;

        private static MySqlCommand addToDatabaseTitle;
        private static MySqlCommand getTitleID;
        private static MySqlCommand addToDatabaseGenre;
        private static MySqlCommand addToDatabaseCategory;

        private static int titleid;

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
        public static string CurrentTitle
        { get; set; }
        public static string CurrentGame
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

        public static void GetCurrentTitleAndCategory()
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
                CurrentTitle = subscription.Data[0].Title;
                CurrentGame = subscription.Data[0].GameId;
            };

            d.RunWorkerAsync();
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

        public static async void SaveToDatabase(string title, string genres, ListView EnabledCategories)
        {
            Data.mysqlcon.Open();
            addToDatabaseTitle = new MySqlCommand("INSERT IGNORE INTO titles (title) VALUES (@title);", Data.mysqlcon);
            addToDatabaseTitle.Parameters.AddWithValue("@title", title);
            await addToDatabaseTitle.ExecuteNonQueryAsync();

            getTitleID = new MySqlCommand("SELECT idtitle FROM titles WHERE title = @title", Data.mysqlcon);
            getTitleID.Parameters.AddWithValue("@title", title);

            using (DbDataReader res = await getTitleID.ExecuteReaderAsync())
            {
                await res.ReadAsync();
                titleid = res.GetInt32(0);
            }

            string[] subs1 = genres.Split(',');
            string[] formed1 = new string[subs1.Length];
            string result1 = "INSERT IGNORE INTO genres (idtitle, genre) VALUES ";

            for (int i = 0; i < subs1.Length; i++)
            {
                formed1[i] = "( @titleid, " + "'" + subs1[i] + "'" + ")";
            }

            result1 = result1 + string.Join(", ", formed1);

            addToDatabaseGenre = new MySqlCommand(result1 + ";", Data.mysqlcon);
            //addToDatabaseGenre.Parameters.AddWithValue("@genre", Genre.Text);
            addToDatabaseGenre.Parameters.AddWithValue("@titleid", titleid);
            addToDatabaseGenre.Prepare();
            await addToDatabaseGenre.ExecuteNonQueryAsync();

            string[] subs = new string[EnabledCategories.Items.Count];
            int j = 0;
            foreach (CategoryEntry item in EnabledCategories.Items.ToList())
            {
                subs[j] = item.GameID;
                j++;
            }
            string[] formed = new string[subs.Length];
            string result = "INSERT IGNORE INTO ct_intersect (idtitle, gameid) VALUES ";

            for (int i = 0; i < subs.Length; i++)
            {
                formed[i] = "( @titleid, " + subs[i] + ")";
            }

            result = result + string.Join(", ", formed);

            Debug.WriteLine(result);
            addToDatabaseCategory = new MySqlCommand(result + ";", Data.mysqlcon);
            addToDatabaseCategory.Parameters.AddWithValue("@titleid", titleid);
            addToDatabaseCategory.Prepare();
            await addToDatabaseCategory.ExecuteNonQueryAsync();

            Data.mysqlcon.Close();
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
