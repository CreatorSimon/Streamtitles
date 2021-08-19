using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
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
        private static SQLiteCommand _GetTableTitle;
        private static string _conStr;
        public static SQLiteConnection mysqlcon;

        private static SQLiteCommand _readTitle;
        private static SQLiteCommand _getAllCategories;
        private static string _currentGame;

        private static TwitchAPI api;

        private static ApplicationDataContainer localSettings;

        private static SQLiteCommand addToDatabaseTitle;
        private static SQLiteCommand getTitleID;
        private static SQLiteCommand addToDatabaseGenre;
        private static SQLiteCommand addToDatabaseCategory;

        private static int titleid;

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
        public static bool FullConnected { get; set; }

        public object TwitchApiAddress { get; private set; }
        public static List<string> Categories { get; private set; }

        static Data()
        {
            localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            api = new TwitchAPI();
            CurrentTitle = "";
            StartupCheck();
            Categories = new List<string>();
            api.Helix.Settings.ClientId = ClientID;
            api.Helix.Settings.Secret = Secret;
            api.Helix.Settings.AccessToken = Token;
        }

        public static void ChangeTwitchTitleAndCategory(string title, string category)
        {
            if (TryTwitchConnection())
            {
                Title = title;
                Game = category;

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
        }

        public static bool TryTwitchConnection()
        {
            try
            {
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
            if (TryTwitchConnection())
            {
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
        }

        public static void GetAllGames()
        {
            if (TryFullConnection())
            {
                BackgroundWorker d = new BackgroundWorker();
                d.DoWork += async (a, s) =>
                {
                    var subscription =
                        await api.Helix.Games.GetTopGamesAsync(first: 100);
                    Debug.WriteLine(subscription.Data[0].Id);

                    string[] values = new string[100];
                    string result = "INSERT OR IGNORE INTO categories (gameid, name) VALUES ";
                    for (int i = 0; i < 100; i++)
                    {
                        values[i] = "(" + subscription.Data[i].Id + ", '" + subscription.Data[i].Name.Replace("'", "") + "')";
                    }
                    SQLiteCommand fillCategories = new SQLiteCommand(result + string.Join(", ", values) + ";", mysqlcon);
                    mysqlcon.Open();
                    fillCategories.Prepare();
                    fillCategories.ExecuteNonQuery();
                    mysqlcon.Close();

                };
                d.RunWorkerAsync();
            }
        }

        public static async void GetAllCategories(ListView DisabledCategories = null)
        {
            if (TrySqlConnection())
            {
                Categories.Clear();
                SQLiteCommand _getAllCategories = new SQLiteCommand("SELECT name, gameid FROM categories ORDER BY name ASC;", mysqlcon);
                mysqlcon.Open();
                using (DbDataReader res = await _getAllCategories.ExecuteReaderAsync())
                {
                    while (await res.ReadAsync())
                    {
                        var entry = new CategoryEntry();
                        entry.Name = res.GetString(0);
                        entry.GameID = res.GetInt32(1).ToString();

                        if (DisabledCategories != null)
                        {
                            DisabledCategories.Items.Add(entry);
                        }
                        Categories.Add(entry.Name);
                    }
                }
                mysqlcon.Close();
            }
        }

        public static bool TrySqlConnection()
        {
            try
            {
                mysqlcon.Open();
                mysqlcon.Close();
            }
            catch (Exception  e) when (e is SqlException || e is InvalidOperationException)
            {
                mysqlcon = null;
                return false;
            }
            return true;
        }

        public static bool TryFullConnection()
        {
            if(TrySqlConnection() && TryTwitchConnection())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static async void SaveToDatabase(string title, string genres, ListView EnabledCategories)
        {
            if (TrySqlConnection())
            {
                mysqlcon.Open();
                addToDatabaseTitle = new SQLiteCommand("INSERT OR IGNORE INTO titles (title) VALUES (@title);", mysqlcon);
                addToDatabaseTitle.Parameters.AddWithValue("@title", title);
                await addToDatabaseTitle.ExecuteNonQueryAsync();

                getTitleID = new SQLiteCommand("SELECT idtitle FROM titles WHERE title = @title", mysqlcon);
                getTitleID.Parameters.AddWithValue("@title", title);

                using (DbDataReader res = await getTitleID.ExecuteReaderAsync())
                {
                    await res.ReadAsync();
                    titleid = res.GetInt32(0);
                }

                string[] subs1 = genres.Split(',');
                string[] formed1 = new string[subs1.Length];
                string result1 = "INSERT OR IGNORE INTO genres (idtitle, genre) VALUES ";

                for (int i = 0; i < subs1.Length; i++)
                {
                    formed1[i] = "( @titleid, " + "'" + subs1[i] + "'" + ")";
                }

                result1 = result1 + string.Join(", ", formed1);

                addToDatabaseGenre = new SQLiteCommand(result1 + ";", mysqlcon);
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
                string result = "INSERT OR IGNORE INTO ct_intersect (idtitle, gameid) VALUES ";

                for (int i = 0; i < subs.Length; i++)
                {
                    formed[i] = "( @titleid, " + subs[i] + ")";
                }

                result = result + string.Join(", ", formed);

                addToDatabaseCategory = new SQLiteCommand(result + ";", Data.mysqlcon);
                addToDatabaseCategory.Parameters.AddWithValue("@titleid", titleid);
                addToDatabaseCategory.Prepare();
                await addToDatabaseCategory.ExecuteNonQueryAsync();

                mysqlcon.Close();
            }
        }

        public static async void GetData(List<DatabaseListEntry> DataList)
        {
            if (TrySqlConnection())
            {
                Data.mysqlcon.Open();
                _GetTableTitle = new SQLiteCommand("SELECT titles.idtitle, titles.title , GROUP_CONCAT(DISTINCT genre) as genre, GROUP_CONCAT(DISTINCT name) as category FROM titles, genres, categories, ct_intersect WHERE(titles.idtitle = genres.idtitle and titles.idtitle = ct_intersect.idtitle and ct_intersect.gameid = categories.gameid) GROUP BY titles.idtitle;", Data.mysqlcon);
                using (DbDataReader res = await _GetTableTitle.ExecuteReaderAsync())
                {
                    while (await res.ReadAsync())
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

        }

        public static void SaveSettings()
        {
            localSettings.Values["api clientid"] = ClientID;
            localSettings.Values["api secret"] = Secret;
            localSettings.Values["api token"] = Token;

            localSettings.Values["channel"] = Channel;
        }

        public static void LoadSettings()
        {
            ClientID = localSettings.Values["api clientid"] as string;
            Secret = localSettings.Values["api secret"] as string;
            Token = localSettings.Values["api token"] as string;

            Channel = localSettings.Values["channel"] as string;
        }

        public static async void GenerateTitle(string Selected)
        {
            if (TrySqlConnection())
            {
                mysqlcon.Open();
                _readTitle = new SQLiteCommand("SELECT titles.title, categories.gameid FROM titles, categories, ct_intersect WHERE(titles.idtitle = ct_intersect.idtitle and @Category=categories.name and ct_intersect.gameid = categories.gameid) ORDER BY RAND() LIMIT 1;", mysqlcon);
                _readTitle.Parameters.AddWithValue("@Category", Selected);
                using (DbDataReader res = await _readTitle.ExecuteReaderAsync())
                {
                    await res.ReadAsync();
                    CurrentTitle = res.GetString(0);
                    CurrentGame = res.GetString(1);
                }
                mysqlcon.Close();
            }
        }

        public static void ChangeApiCredentials()
        {
            api.Helix.Settings.ClientId = ClientID;
            api.Helix.Settings.Secret = Secret;
            api.Helix.Settings.AccessToken = Token;
        }

        public static void StartupCheck()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "streamtitles.db3");
            if (!File.Exists(path))
            {


                SQLiteConnection.CreateFile(path);

                mysqlcon = new SQLiteConnection($"Data Source={path};Version=3;");
                mysqlcon.Open();

                string sql = "create table titles (idtitle INTEGER PRIMARY KEY AUTOINCREMENT, title varchar(100)); create table genres (idtitle INT, genre VARCHAR(100)); create table categories (gameid INT NOT NULL, name varchar(100), PRIMARY KEY(gameid)); create table ct_intersect (gameid INT NOT NULL, idtitle INT NOT NULL, PRIMARY KEY(idtitle, gameid));";

                SQLiteCommand command = new SQLiteCommand(sql, mysqlcon);
                command.ExecuteNonQuery();

                mysqlcon.Close();
            }
            else
            {
                mysqlcon = new SQLiteConnection($"Data Source={path};Version=3;");
            }
        }

    }
}
