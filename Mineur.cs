
using System;
using System.Threading;
using System.Threading.Tasks;

public class Mineur
{
    public Mineur()
    {
        Thread myThread = new Thread(new ThreadStart(Mine));
        myThread.Start();
    }

    public void Mine()
    {
        while (true)
        {
            Console.WriteLine("HEHEHEHEHEHEHEHEHE");
        }
    }
}