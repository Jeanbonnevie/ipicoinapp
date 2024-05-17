using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

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
            AskForConfirmationBeforeSending();
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
        //ask for whose the user wants to send to key
        // check for validity (is a number ?)
        // does the user  has such kind of money on its wallet ?
        return "";
    }

    private void AskForConfirmationBeforeSending()
    {
        //Ask for condifmation with a recap of the transaction
        //Create the trransaction
        //Send Transaction
    }
}