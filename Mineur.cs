
using ipiblockChain;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class Mineur
{
    Thread myThread = null;
    int difficulty = 2;
    Block currentBlock;

    ConcurrentBag<Transaction> availibleTransaction = new ConcurrentBag<Transaction>();
    private event Action<Block> OnBlockfound;

    public Mineur()
    {
        OnBlockfound += block =>
        {
            currentBlock = block;

            myThread.Abort();

            myThread = new Thread(new ThreadStart(Mine));
            myThread.Start();
        };

        HttpServer.OnNewTransactionReceived += t =>
        {
            availibleTransaction.Add(t);
        };

        myThread = new Thread(new ThreadStart(Mine));
        myThread.Start();
    }

    public void Mine()
    {
        Block block = new Block();
        bool isRunning = true;
        while (isRunning)
        {
            block.AddTransaction(availibleTransaction.ToList());
            block.GenNonce();

            if (block.CanBeUsed())
            {
                // TestHash with blocj cahin
                isRunning = false;
                OnBlockfound?.Invoke(block);
            }
        }
    }
}