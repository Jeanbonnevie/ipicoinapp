using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class Client
{
    public Client()
    {
        var handler = new HttpClientHandler()
        {
            // Disable automatic retries
            AutomaticDecompression = System.Net.DecompressionMethods.None,
            AllowAutoRedirect = false,
            UseDefaultCredentials = false,
        };

       client = new HttpClient(handler);
    }

    private static HttpClient client ;

    private const string HOST = "http://localhost:9090/"; //TODO :: change for someOne else
    private const string SEND_TRANSACTION = "newtx?tx=";

    private static readonly Object obj = new Object();
    public async void SendTransaction(string transactionJson)
    {
        lock (obj)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );
        }

        string baseUrl = HOST + SEND_TRANSACTION + transactionJson;

        try
        {
            await ProcessRepositoriesAsync(client, baseUrl);
        }
        catch (Exception ex)
        {
            //Console.WriteLine("Issue with send transaction " + ex.ToString());
        }
    }


    static async Task ProcessRepositoriesAsync(HttpClient client, string command)
    {
        var r = await client.GetStringAsync(command);

        Console.WriteLine(r);
    }
}