using System;
using System.Net.Http;

public class Client
{
    public Client() 
    {
    }

    private static HttpClient sharedClient = new HttpClient()
    {
        BaseAddress = new Uri(LOCALHOST),
    };

    private const string LOCALHOST = "https://127.0.0.1:9090/";
    private const string SEND_TRANSACTION = "newtx?tx=";

    public async void SendTransaction(string transactionJson)
    {
        try
        {
            string message = SEND_TRANSACTION;// + transactionJson;
            HttpResponseMessage response = await sharedClient.GetAsync(
                message
            );
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();

            Console.WriteLine(jsonResponse);
        }
        catch(Exception ex)
        {
            Console.WriteLine( "Issue with send transaction " + ex.ToString() );
        }
    }
}