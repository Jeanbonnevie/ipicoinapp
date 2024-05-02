using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ipiblockChain
{
    public class Program
    {
        static void Main(string[] args)
        {
            HttpServer.Init("0.0.0.0",9090);
        }
    }
}
