using System;
using System.Net.Http;

public class Client
{
    public Client() 
    {
    }

    private static HttpClient sharedClient = new HttpClient()
    {
        BaseAddress = new Uri("https://localhost:9009"),
    };

    private const string SEND_TRANSACTION_LOCALHOST = "https://localhost:9090/";

    public async void SendTransaction(string transactionJson)
    {
        try
        {
            string message = SEND_TRANSACTION_LOCALHOST + transactionJson;
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