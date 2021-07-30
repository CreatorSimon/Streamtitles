using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public static string _ip;
        public static string _port;
        public static string _user;
        public static string _password;

        public static void Generate_ConnectionString()
        {
            _conStr = $"server = {_ip}; port = {_port}; user = {_user}; pwd = {_password}; database = streamtitles;";
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
