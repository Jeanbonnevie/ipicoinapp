using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;


namespace ipiblockChain
{
    public class HttpServer
    {
        const string UNKNOWN_ACTION = "UNKNOWN_ACTION";
        const string BLOCK_MISSING = "BLOCK_MISSING";
        const string OK = "OK";

        private int Port;
        private string Addr;
        public HttpServer(string addr, int port) {
            this.Addr = addr;
            this.Port = port;
        }

        public static void Init(string Addr, int Port)
        {
            var server = new HttpServer(Addr, Port);
            server.Start();
        }

        public void Start()
        {
            TcpListener listener = new TcpListener(IPAddress.Parse(this.Addr), this.Port);
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
                string command = "";

                if (queryParams.Length > 1)
                {
                    string[] param = queryParams[1].Split('=');
                    command = queryParams[0];
                    if (param.Length > 1) 
                    {
                        blockJSON = param[1];
                    }
                }
                    
                // Handle different endpoints
                string response;
                if (endpoint == "/verify" && !string.IsNullOrEmpty(blockJSON) && command == "block")
                {
                    blockJSON = WebUtility.UrlDecode(blockJSON);
                    try
                    {
                        Block block = Block.CreateBlock(blockJSON);
                        response = $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\nVerification result for block {blockJSON}: OK";
                    }catch (Exception ex)
                    {
                        response = $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\nVerification result for block {blockJSON}: OK" +
                            "\n" + $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n {ex.Message}";
                    }

                }
                else if (endpoint == "/newblock" && !string.IsNullOrEmpty(blockJSON) && command == "block")
                {
                    Block block = Block.CreateBlock(blockJSON);
                    block.GetBiggestBlock(block);
                    response = $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n {OK}";
                }
                else if (endpoint == "/newtx" && !string.IsNullOrEmpty(blockJSON) && command == "tx")
                {
                    Transaction transaction = Transaction.CreateTransaction(blockJSON);

                    response = $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n {OK}";
                }
                else
                {
                    response = $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n {UNKNOWN_ACTION}";
                }

                await writer.WriteAsync(response);
            }

            client.Close();
        }
    } 
}