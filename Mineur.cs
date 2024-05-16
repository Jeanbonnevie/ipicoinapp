
using ipiblockChain;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class Mineur
{
    Thread myThread = null;
    Block currentBlock;
    private BlockChain currentChain;

    List<Transaction> availibleTransaction = new List<Transaction>();

    public Mineur(BlockChain currentChain)
    {
        this.currentChain = currentChain;

        currentChain.OnTransactionsReceived += t =>
        {
            availibleTransaction.AddRange(t);
        };

        currentChain.OnExternalBlockModifiesBlockChain += b =>
        {
            //Todo :: synch transactions to never lose any

            myThread.Abort();
            myThread = new Thread(new ThreadStart(Mine));
            myThread.Start();
        };

        myThread = new Thread(new ThreadStart(Mine));
        myThread.Start();
        this.currentChain = currentChain;
    }

    public void Mine()
    {
        Block block = new Block();
        currentBlock = block;
        currentChain.InitBlock(ref block);

        bool isRunning = true;
        while (isRunning) // Pas opti DavidGoodenought
        {
            if (availibleTransaction.Count > 0)
                if (block.data == null)
                    block.AddTransaction(availibleTransaction.Last());
                else if (block.data.ToList().Find(t => t.timestamp == availibleTransaction.Last().timestamp) == null)
                    block.AddTransaction(availibleTransaction.Last());
           
            block.GenNonce();

            if (block.CanBeUsed())
            {
                if (currentChain.TryAddBlockToBlockChain(block))
                {
                    isRunning = false;

                    myThread = new Thread(new ThreadStart(Mine));
                    myThread.Start();
                }
            }
        }
    }
}