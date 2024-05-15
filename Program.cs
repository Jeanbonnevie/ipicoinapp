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
            Mineur mineur = new Mineur();   
            HttpServer.Init("0.0.0.0",9090);
            BlockChain blockChain = new BlockChain();
        }
    }
}
