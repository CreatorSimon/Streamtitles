using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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

        private static readonly string clientId = "";
        private static readonly string token = "";
        private static readonly string userName = "DunkingBot";
        private static readonly string channel = "dunkingsimon";
        private static TwitchAPI api = new TwitchAPI();

        public static string _ip;
        public static string _port;
        public static string _user;
        public static string _password;

        private static BackgroundWorker background = new BackgroundWorker();


        public static void Generate_ConnectionString()
        {
            _conStr = $"server = {_ip}; port = {_port}; user = {_user}; pwd = {_password}; database = streamtitles;";
        }

        public static void Change_Twitch_Title()
        {
            api.Helix.Settings.ClientId = "gp762nuuoqcoxypju8c569th9wz7q5";
            api.Helix.Settings.Secret = "qgfr6bh812cwrx8bpifysym1c0t559";
            api.Helix.Settings.AccessToken = "mr020ny5zw9js6abt0nzx2xjb8fm8p";

            api.Helix.Settings.Scopes = new List<AuthScopes>() { AuthScopes.Any, AuthScopes.Helix_Channel_Manage_Broadcast };
            BackgroundWorker d = new BackgroundWorker();
            d.DoWork += async (a, s) =>
            {
                var user = await api.Helix.Users.GetUsersAsync(logins: new List<string>() { "dunkingsimon" });
                api.Helix.Settings.AccessToken = api.Helix.Channels.GetAccessToken();

                var t = new ModifyChannelInformationRequest();
                t.Title = "C#";
                await api.Helix.Channels.ModifyChannelInformationAsync(user.Users[0].Id, t);
                var subscription =
                    await api.Helix.Channels.GetChannelInformationAsync(user.Users[0].Id);
                Debug.WriteLine(token);
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

    }
}
