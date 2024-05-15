
using ipiblockChain;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class Mineur
{
    Thread myThread = null;
    Block currentBlock;
    private BlockChain currentChain;

    ConcurrentBag<Transaction> availibleTransaction = new ConcurrentBag<Transaction>();

    public Mineur(BlockChain currentChain)
    {
        this.currentChain = currentChain;

        HttpServer.OnNewTransactionReceived += t =>
        {
            availibleTransaction.Add(t);
        };

        myThread = new Thread(new ThreadStart(Mine));
        myThread.Start();
        this.currentChain = currentChain;
    }

    public void Mine()
    {
        Block block = new Block();
        currentChain.InitBlock(ref block);

        bool isRunning = true;
        while (isRunning) // Pas opti DavidGoodenought
        {
            if(availibleTransaction.TryTake(out Transaction transaction))
            {
                block.AddTransaction(transaction);
            }

            block.GenNonce();

            if (block.CanBeUsed())
            {
                if (currentChain.TryAddBlockToBlockChain(block))
                {
                    isRunning = false;

                    new Thread(new ThreadStart(Mine)).Start();
                }
            }
        }
    }
}