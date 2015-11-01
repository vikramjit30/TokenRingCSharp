using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookComputing.XmlRpc;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.IO;


namespace MyCalendar
{
    class Server: MarshalByRefObject, OperationInterface
    {
        private static int port;
        public Server(int _port)
        {

            port = _port;
        }
        public Server()
        {
        }
        public void run()
        {
            IDictionary props = new Hashtable();
            props["name"] = "MyHttpChannel";
            props["port"] = port;
            HttpChannel channel = new HttpChannel(
               props,
               null,
               new XmlRpcServerFormatterSinkProvider()
            );
            ChannelServices.RegisterChannel(channel, false);

            RemotingConfiguration.RegisterWellKnownServiceType(
              typeof(Server),
              "xmlrpc",
              WellKnownObjectMode.Singleton);
           }

        public int addEntry(string s)
        {
            Console.WriteLine("Adding new event...");
            using (StreamWriter writer = new StreamWriter("Calendar.txt", true))
            {
                writer.WriteLine(s);
            }
            Console.WriteLine("Updation Done!");
            return 42;
        }


    public int result_update(String result)
	{
	        Console.WriteLine(" Result is updating..");
	        using (StreamWriter writer = new StreamWriter("Calendar.txt", true))
            {
                writer.WriteLine(result);
            }
            Console.WriteLine("Updation Done!");
            return 42;
	     
	}


        public int deleteEntry(string lineToRemove)
        {
            Console.WriteLine("Deleting event...");

            List<string> quotelist = File.ReadAllLines("Calendar.txt").ToList();
            quotelist.Remove(lineToRemove);
            File.WriteAllLines("Calendar.txt", quotelist.ToArray());
            Console.WriteLine("Deletion Done!");
            return 42;
        }

        public int modifyEntry(string lineToRemove, string newData)
        {
            Console.WriteLine("Modifying event...");

            List<string> quotelist = File.ReadAllLines("Calendar.txt").ToList();
            int index = quotelist.IndexOf(lineToRemove);
            quotelist[index] = newData;
            File.WriteAllLines("Calendar.txt", quotelist.ToArray());
            Console.WriteLine("Modification Done!");
            return 42;
        }

        public Object[] getList()
        {
            Console.WriteLine("Returning a listof all events");
            String[] list = System.IO.File.ReadAllLines("Calendar.txt");
            Console.WriteLine("Done!");
            return list;
        }
        public Object[] getListOfActiveNodes()
        {
            Console.WriteLine("Returning list of active nodes.");
            String[] result = new String[Client.activeNodes.Count];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Client.activeNodes[i].getIPnPort();
            }
            Console.WriteLine("List Passed!");
            return result;
        }
        public bool addNode(string IpnPort)
        {
            Console.WriteLine("New node has joined the network.");
            //Client.activeNodes.Add(ipAddress);
            HostIPnPort newObj = new HostIPnPort(IpnPort);
            int k = 0;
            while (k < Client.activeNodes.Count && Client.activeNodes[k].compare(newObj) < 0) k++;
            Client.activeNodes.Insert(k, newObj); 
            return true;
        }

        public bool deleteNode(string IpnPort)
        {
            Console.WriteLine("Node has signed off the network.");
            //Client.activeNodes.Remove(ipAddress);
            int k = 0;
            while (k < Client.activeNodes.Count && !Client.activeNodes[k].getIPnPort().Equals(IpnPort)) k++;
            if (k < Client.activeNodes.Count)
            {
                HostIPnPort obj = Client.activeNodes[k];
                Client.activeNodes.Remove(obj);
            }
            if (Client.activeNodes.Count == 1)
                TokenRing.stopTokenRingAlgorithm(); //If you are alone you must stop Token Ring
            return true;
        }

        public int takeTheToken(int ack)
        {
            return TokenRing.takeTheToken(ack);
        }
    }
}
