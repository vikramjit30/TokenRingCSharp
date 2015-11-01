using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace MyCalendar
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The starting number is 30 ");
            using (StreamWriter writer = new StreamWriter("Calendar.txt"))
            {
                writer.WriteLine("30");
            }
            Console.WriteLine("Input port:");
            int port = int.Parse(Console.ReadLine());
            Client client = new Client(port);
            Server server = new Server(port);
            Thread oThread1 = new Thread(new ThreadStart(client.run));
            Thread oThread2 = new Thread(new ThreadStart(server.run));
            oThread1.Start();
            oThread2.Start();
           
         }
    }
}
