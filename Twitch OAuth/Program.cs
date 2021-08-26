using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Twitch_OAuth
{
    internal class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var p = new Program();
            var code = await p.DoOAuthAsync(args[0], args[1], args[2]);
            Console.WriteLine(code);
            return 0;
        }

        private async Task<string> DoOAuthAsync(string authEndpoint, string clientID, string redirectURI)
        {
            var http = new HttpListener { Prefixes = { redirectURI} };
            http.Start();

            Process myProcess = new Process();


            myProcess.StartInfo.UseShellExecute = true;
            myProcess.StartInfo.FileName = $"{authEndpoint}?client_id={clientID}&redirect_uri={Uri.EscapeDataString(redirectURI)}&response_type=code&scope=channel:manage:broadcast+openid";

            myProcess.Start();

            // Waits for the OAuth authorization response.
            var context = await http.GetContextAsync();

            await WriteMessageToBrowser(context.Response);
            http.Stop();

            // Checks for errors.
            CheckForErrors(context.Request);

            var queryStringParams = context.Request.RawUrl.Substring(2).Split('&').ToDictionary(c => c.Split('=')[0], c => Uri.UnescapeDataString(c.Split('=')[1]));

            Console.WriteLine(queryStringParams["code"]);

            // extracts the code
            return queryStringParams["code"];
        }

        private async Task WriteMessageToBrowser(HttpListenerResponse response)
        {
            string responseString = "<html><head><meta http-equiv='refresh' content='10;url=https://google.com'></head><body>Please return to the app.</body></html>";
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            var responseOutput = response.OutputStream;
            await responseOutput.WriteAsync(buffer, 0, buffer.Length);
            responseOutput.Close();
        }

        private void CheckForErrors(HttpListenerRequest request)
        {
            string error = request.QueryString.Get("error");
            if (error is object)
            {
                Log($"OAuth authorization error: {error}.");
                throw new InvalidDataException();
            }
        }

        private void Log(string output)
        {
            Console.WriteLine(output);
        }
    }
}