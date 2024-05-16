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
                var sz = JsonConvert.SerializeObject(Transaction.CreateRandomTransaction());
                 m_client.SendTransaction(sz);
                Thread.Sleep(1000);
            }
        });
        myThread.Start();
    }
}