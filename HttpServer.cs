using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ipiblockChain
{
    public class HttpServer
    {
        public static void Init()
        {
            var server = new HttpServer();
            server.Start("0.0.0.0", 9090);
            Console.WriteLine("Server started. Listening on port 9090...");
            Console.ReadLine(); // Keeps the server running until Enter is pressed
        }

        public void Start(string ipAddress, int port)
        {
            TcpListener listener = new TcpListener(IPAddress.Parse(ipAddress), port);
            listener.Start();

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                HandleClient(client);
            }
        }

        private async void HandleClient(TcpClient client)
        {
            using (var stream = client.GetStream())
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            using (var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
            {
                string request = await reader.ReadLineAsync();
                if (request == null) return;

                string[] parts = request.Split(' ');
                string method = parts[0];
                string url = parts[1];

                string[] queryParams = url.Split('?');
                string endpoint = queryParams[0];

                string blockJSON = null;

                if (queryParams.Length > 1)
                {
                    string[] param = queryParams[1].Split('=');
                    if (param.Length > 1) 
                    {
                        blockJSON = param[1];
                    }
                }
                    

                // Handle different endpoints
                string response;
                if (endpoint == "/verify" && !string.IsNullOrEmpty(blockJSON))
                {
                    response = $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\nVerification result for block {blockJSON}: OK";
                }
                else
                {
                    response = "HTTP/1.1 404 Not Found\r\nContent-Type: text/plain\r\n\r\n404 Not Found";
                }

                await writer.WriteAsync(response);
            }

            client.Close();
        }
    } 
}