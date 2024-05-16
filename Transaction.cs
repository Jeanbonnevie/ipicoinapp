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
    private const string MON_COMPTE = "08EC48042C5A5502C4F9674532DC91BDDF0ACD8A";

    public static Transaction CreateTransaction(string TransactionJSON)
    {
        Transaction transaction = JsonConvert.DeserializeObject<Transaction>(TransactionJSON);

        if (transaction.amount <= 0) throw new Exception();
        if (transaction.timestamp > DateTimeOffset.UtcNow.ToUnixTimeSeconds()) throw new Exception();

        return transaction;
    }

    public string addrSender;
    public string addrRcpt;
    public float amount;
    public long timestamp;

    public static Transaction CreateNewTransaction(string sender, string recipient, float amount, long timestamp)
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
        };

        return transaction;
    }

    public static Transaction CreateRandomTransaction()
    {
        return new Transaction
        {
            addrSender = "3436C778A660A2A06F73F9AB7BF090BF40CE3F79",
            addrRcpt = MON_COMPTE,
            amount = 0.1f,
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
        };
    }
}

