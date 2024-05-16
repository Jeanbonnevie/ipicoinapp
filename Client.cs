using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

public class Client
{
    public Client() 
    {
    }

    private static HttpClient client = new HttpClient();

    private const string LOCALHOST = "http://127.0.0.1:9090/";
    private const string SEND_TRANSACTION = "newtx?tx=";

    public async void SendTransaction(string transactionJson)
    {
        string baseUrl = LOCALHOST + SEND_TRANSACTION;

        string urlWithParams = $"{baseUrl}{transactionJson}";
        try
        {
            HttpResponseMessage response = await client.GetAsync(urlWithParams);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response from server: {responseContent}");
            }
            else
            {   
                Console.WriteLine($"Failed to send request. Status code: {response.StatusCode}");
            }   
        }
        catch(Exception ex)
        {
            Console.WriteLine( "Issue with send transaction " + ex.ToString() );
        }
    }
}