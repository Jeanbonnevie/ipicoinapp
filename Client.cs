using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ipiblockChain;

public class Client
{
    private BlockChain m_blockChain;
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
    private const string SEND_BLOCk = "newblock?block=";

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
        catch (Exception ex) // commenter un debug ça fix le bug?? non ? ??
        {
            //Console.WriteLine("Issue with send transaction " + ex.ToString());
        }
    }

    public async void SendBlock(string blockJson)
    {
        lock (obj)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );
        }

        string baseUrl = HOST + SEND_BLOCk + blockJson;

        try
        {
            await ProcessRepositoriesAsync(client, baseUrl);
        }
        catch (Exception ex) // commenter un debug ça fix le bug?? non ? ??
        {
            //Console.WriteLine("Issue with send transaction " + ex.ToString());
        }
    }

    public bool CheckBalance(string walletId, float transactionAmount)
    {
        float balance = m_blockChain.GetBalance(walletId);
        return balance >= transactionAmount;
    }


    static async Task ProcessRepositoriesAsync(HttpClient client, string command)
    {
        var r = await client.GetStringAsync(command);

        Console.WriteLine(r);
    }
}