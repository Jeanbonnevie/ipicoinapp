using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ipiblockChain
{
    public class Program
    {
        static void Main(string[] args)
        {
            BlockChain blockchain = new BlockChain();
            Mineur mineur = new Mineur(blockchain);   

            Thread myThread = null;
            myThread = new Thread(() => {
                Client client = new Client();
                Simulator simulator = new Simulator(client);
                simulator.SimulateTransactions();
            });

            myThread.Start();

            HttpServer.Init("0.0.0.0", 9090, blockchain);
        }
    }
}
