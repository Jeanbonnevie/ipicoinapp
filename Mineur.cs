
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
    private event Action<Block> OnBlockfound;

    public Mineur(BlockChain currentChain)
    {
        this.currentChain = currentChain;

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
        this.currentChain = currentChain;
    }

    public void Mine()
    {
        Block block = new Block();
        currentChain.InitBlock(ref block);

        bool isRunning = true;
        while (isRunning) // Pas opti DavidGoodenought
        {
            block.AddTransaction(availibleTransaction.ToList());
            block.GenNonce();

            if (block.CanBeUsed())
            {
                if (currentChain.TryAddBlockToBlockChain(block))
                {
                    isRunning = false;
                    OnBlockfound?.Invoke(block);
                }
            }
        }
    }
}