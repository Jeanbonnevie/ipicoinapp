using System;
using System.Collections.Generic;
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

        if (transaction.addrSender.Length != 256) throw new Exception();
        if (transaction.addrRcpt.Length != 256) throw new Exception();
        if (transaction.amount != 256) throw new Exception();
        if (transaction.Sig.Length != 256) throw new Exception();
        if (transaction.timestamp != 256) throw new Exception();

        return transaction;
    }

    public string addrSender;
    public string addrRcpt;
    public float amount;
    public long timestamp;
    public string Sig;
}
