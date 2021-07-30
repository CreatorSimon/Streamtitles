using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streamtitles
{
    class SecretData
    {
        private static string _clientID = "gp762nuuoqcoxypju8c569th9wz7q5";
        private static string _secret = "qkve74p00ruuhp6ojffpqmtou2ob5o";
        private static string _token = "63yz4grwnuryz1f8se6l26lrhtvpdl";

        public static string ClientID
        {
            get { return _clientID; }
            set { _clientID = value; }
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
    }
}
