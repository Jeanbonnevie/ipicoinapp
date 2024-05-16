using Newtonsoft.Json;
using System.Threading;

public class Simulator
{
    public  Simulator(Client client)
    {
        m_client = client;
    }

    private Client m_client;

    public void SimulateTransactions()
    {
        Thread myThread = null;
        myThread = new Thread(() => {
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(4000);
                var sz = JsonConvert.SerializeObject(Transaction.CreateRandomTransaction());
                 m_client.SendTransaction(sz);
            }
        });
        myThread.Start();
    }
}