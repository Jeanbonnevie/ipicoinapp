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
            server.Start("127.0.0.1", 9090);
            Console.WriteLine("Server started. Listening on port 8080...");
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

                // Just a simple response to any request
                string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\nHello, World!";

                await writer.WriteAsync(response);
            }

            client.Close();
        }
    }
}