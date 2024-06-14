using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Newtonsoft.Json;

public class ConsoleHandler
{
    private Client m_client;

    public ConsoleHandler(Client client)
    {
        m_client = client;

        Thread myThread = null;
        myThread = new Thread(() => {
            HandleConsole();        
        });
        myThread.Start();
    }

    private void HandleConsole() // Add a security system to secure the "wallet" 
    {
        while (true)
        {
            string senderID = GetASha256ID();
            string receiver = GetASha256ID();
            string transactionAmount = GetATransactionAmount();
            AskForConfirmationBeforeSending(senderID, receiver, transactionAmount);
        }
    }

    private string GetASha256ID()
    {
        // ask for a key
        // check for validity (64 char ?)
        return "";
    }

    private string GetATransactionAmount()
    {
        string input = Console.ReadLine();
        if (decimal.TryParse(input, out decimal amount) && amount > 0)
        {
            return input;
        }
        else
        {
            throw new ArgumentException("Invalid transaction amount. It must be a positive number.");
        }
    }

    private void AskForConfirmationBeforeSending(string senderID, string receiver, string transactionAmount)
    {
        Console.WriteLine($"You are about to send {transactionAmount} to {receiver} from {senderID}.");
        Console.WriteLine("Do you want to proceed? (yes/no)");
        string confirmation = Console.ReadLine().ToLower();
        if (confirmation == "yes")
        {
            
            Transaction transaction = Transaction.CreateNewTransaction(senderID, receiver, float.Parse(transactionAmount), DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            
            if (m_client.CheckBalance(senderID, float.Parse(transactionAmount)))
            {
                string transactionJson = JsonConvert.SerializeObject(transaction);
                m_client.SendTransaction(transactionJson);
                Console.WriteLine("Transaction confirmed and sent.");
            }
            else
            {
                Console.WriteLine("Transaction cancelled: insufficient funds.");
            }
        }
        else
        {
            Console.WriteLine("Transaction cancelled.");
        }
    }
}