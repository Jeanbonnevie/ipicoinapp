using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

[Serializable]
public class Transaction
{
    public static Transaction CreateTransaction(string TransactionJSON)
    {
        Transaction transaction = JsonConvert.DeserializeObject<Transaction>(TransactionJSON);

        if (transaction.addrSender.Length != 64) throw new Exception();
        if (transaction.addrRcpt.Length != 64) throw new Exception();
        if (transaction.amount != 64) throw new Exception();
        if (transaction.Sig.Length != 64) throw new Exception();
        if (transaction.timestamp != 64) throw new Exception();

        return transaction;
    }

    public string addrSender;
    public string addrRcpt;
    public float amount;
    public long timestamp;
    public string Sig;

    public static Transaction CreateNewTransaction(string sender, string recipient, float amount, long timestamp, string signature)
    {
        if (string.IsNullOrEmpty(sender) || string.IsNullOrEmpty(recipient))
        {
            throw new ArgumentException("Sender and Recipient cannot be null or empty.");
        }

        Transaction transaction = new Transaction
        {
            addrSender = sender,
            addrRcpt = recipient,
            amount = amount,
            timestamp = timestamp,
            Sig = signature
        };

        return transaction;
    }
}

