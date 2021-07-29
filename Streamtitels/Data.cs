using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streamtitels
{
    class Data
    {
        private static string _conStr;
        private static MainPage _mainPage = new MainPage();
        public static MySqlConnection mysqlcon;
        public static void Generate_ConnectionString(string ip, string port, string user, string pw)
        {
            _conStr = $"server = {ip}; port = {port}; user = {user}; pwd = {pw}; database = streamtitles;";
        }

        public static void Connect_sql()
        {
            mysqlcon = new MySqlConnection(_conStr);
        }

        public static string Get_Connection_String()
        {
            return _conStr;
        }
    }
}
