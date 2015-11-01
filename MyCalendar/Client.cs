using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.IO;
using CookComputing.XmlRpc;

namespace MyCalendar
{
    
    public interface IServiceContract : IXmlRpcProxy
    {
        [XmlRpcMethod("Calendar.addEntry")]
        int addEntry(string s);

        [XmlRpcMethod("Calendar.deleteEntry")]
        int deleteEntry(string lineToRemove);

        [XmlRpcMethod("Calendar.modifyEntry")]
        int modifyEntry(string lineToRemove, string newData);

        [XmlRpcMethod("Calendar.getList")]
        Object[] getList();
        
        [XmlRpcMethod("Calendar.result_update")]
        int result_update(String result);

        [XmlRpcMethod("Node.add")]
        bool addNode(string ipAddress);

        [XmlRpcMethod("Node.delete")]
        bool deleteNode(string ipAddress);

        [XmlRpcMethod("Node.getListOfActiveNode")]
        Object[] getListOfActiveNodes();

        [XmlRpcMethod("TokenRing.takeTheToken")]
        int takeTheToken(int ack);
        
        
    }
    

    class Client
    {
        public static List<HostIPnPort> activeNodes = new List<HostIPnPort>();

        private static HostIPnPort thisMachineIpnPort; 
        private static HostIPnPort firstActiveNode;

        private bool isOnline = false;

        private int port;

        public int result;

        public Client(int port)
        {
            this.port = port;
        }
        //Token Ring section
        public static HostIPnPort nextHostOnRing()
        {
            int i = activeNodes.IndexOf(thisMachineIpnPort);
            //make circularity in the ring
            if (i == -1)
                return null;
            else if (i == activeNodes.Count - 1) //if the i refers to the last one we must return the first one
                return activeNodes[0];
            else
                return activeNodes[i + 1];	//else we must return next one
        }
        //End Token Ring Section
        public void run()
        {
            initialize();

            Console.WriteLine("Own IP Adress: " + thisMachineIpnPort.getIp());
            Console.WriteLine("Do you want to change it? (y\\n)");
            String answer = Console.ReadLine();
            String newOwnIpAddress = null;
            if (answer.Equals("y")){
                Console.WriteLine("Input correct IP: ");
                newOwnIpAddress = Console.ReadLine();
            }
            if (newOwnIpAddress != null)
                thisMachineIpnPort.setIp(newOwnIpAddress);
            activeNodes.Add(thisMachineIpnPort);
            while (true){

                Console.WriteLine("------------------------");
                Console.WriteLine("Own IP address: " + thisMachineIpnPort.getIPnPort());
                if (isOnline)
                {
                    Console.WriteLine("Node's status: online");
                }
                else
                {
                    Console.WriteLine("Node's status: offline");
                }

                if (isOnline)
                {
                    Console.WriteLine("List of all online nodes:");
                    for (int i = 0; i < activeNodes.Count; i++) 
                    {
                        Console.WriteLine(activeNodes[i].toString());
                    }
                }
                Console.WriteLine("------------------------");

                Console.WriteLine("Your choice:\n 1 - Addition \n 2 - Subtraction \n" +
                    " 3 - Multiplication \n 4 - Division \n 5 - Show the sequence" +
                    "\n 6 - Join the network \n 7 - Signout \n 8 - Show all the active nodes \n \n 0 - exit ");

                int choice = int.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        Console.WriteLine("===Addition===");
                        additionEntry();
                        break;
                    case 2:
                        Console.WriteLine("===Subtraction===");
                        subtractionEntry();
                        break;
                    case 3:
                        Console.WriteLine("===Multiplication===");
                        multiplicationEntry();
                        break;
                    case 4:
                        Console.WriteLine("===Division===");
                        divisionEntry();
                        break;
                    case 5:
                        Console.WriteLine("===Getting sequence of all events===");
                        getListOfEvents(false);
                        break;
                    case 6:
                        Console.WriteLine("===Joining the network===");
                        join();
                        break;
                    case 7:
                        Console.WriteLine("===Signing off===");
                        signOff();
                        break;
                    case 8:
                        Console.WriteLine("=== Show all active nodes===");
                        listNodes();
                        break;
                     
                    case 0:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Wrong input!");
                        break;
            }
            
            }
        }

        private void listNodes()
        {
            if (isOnline)
            {
                for (int i = 0; i < activeNodes.Count; i++)
                {
                    Console.WriteLine(activeNodes[i].toString());//changed
                }
            }
            else
            {
                Console.WriteLine("Node is offline.");
            }

        } 
        
        private void initialize()
        {
            //if file already exists do nothing
            //else create new file
            String ownIPAddress = "127.0.0.1";
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ownIPAddress = ip.ToString();
                    break;
                }
            }
            thisMachineIpnPort = new HostIPnPort(ownIPAddress, port);
            string curFile = System.Environment.CurrentDirectory + "\\Calendar.txt";
            if (File.Exists(curFile))
            {
                //do nothing
            }
            else
            {
                File.Create(curFile).Dispose();
                
            }
        }

        private void additionEntry()
        {
            if (isOnline)
            {   int b= 0;
                TokenRing.waitForToken();
                getListOfEvents(false);

                Console.WriteLine("Input the number: ");
                String date = Console.ReadLine();
                b = Convert.ToInt32(date);

                String lastLine = File.ReadLines("Calendar.txt").Last();
                result = Convert.ToInt32(lastLine);
                calculation_entry entry = new calculation_entry(result,b);
                entry.writeToFile_addition();
               
                 foreach (HostIPnPort h in activeNodes)
                {
                    if (!h.Equals(thisMachineIpnPort))
                    {
                        addOverRPC(h.getFullUrl(), entry.makeString());
                    }
                }
                TokenRing.sendToken();
            }
            else
            { 
                Console.WriteLine("Node is offline! Operation is not allowed");
            }
        }

        private void subtractionEntry()
        {
            if (isOnline)
            {   int b= 0;
                TokenRing.waitForToken();
                getListOfEvents(false);

                Console.WriteLine("Input the number: ");
                String date = Console.ReadLine();
                b = Convert.ToInt32(date);

                String lastLine = File.ReadLines("Calendar.txt").Last();
                result = Convert.ToInt32(lastLine);
                calculation_entry entry = new calculation_entry(result,b);
                entry.writeToFile_subtraction();
               
                 foreach (HostIPnPort h in activeNodes)
                {
                    if (!h.Equals(thisMachineIpnPort))
                    {
                        addOverRPC(h.getFullUrl(), entry.makeString());
                    }
                }
                TokenRing.sendToken();
            }
            else
            { 
                Console.WriteLine("Node is offline! Operation is not allowed");
            }
        }

        private void multiplicationEntry()
        {
            if (isOnline)
            {   int b= 0;
                TokenRing.waitForToken();
                getListOfEvents(false);

                Console.WriteLine("Input the number: ");
                String date = Console.ReadLine();
                b = Convert.ToInt32(date);

                String lastLine = File.ReadLines("Calendar.txt").Last();
                result = Convert.ToInt32(lastLine);
                calculation_entry entry = new calculation_entry(result,b);
                entry.writeToFile_multiplication();
               
                 foreach (HostIPnPort h in activeNodes)
                {
                    if (!h.Equals(thisMachineIpnPort))
                    {
                        addOverRPC(h.getFullUrl(), entry.makeString());
                    }
                }
                TokenRing.sendToken();
            }
            else
            { 
                Console.WriteLine("Node is offline! Operation is not allowed");
            }
        }

        private void divisionEntry()
        {
            if (isOnline)
            {   int b= 0;
                TokenRing.waitForToken();
                getListOfEvents(false);

                Console.WriteLine("Input the number: ");
                String date = Console.ReadLine();
                b = Convert.ToInt32(date);

                String lastLine = File.ReadLines("Calendar.txt").Last();
                result = Convert.ToInt32(lastLine);
                calculation_entry entry = new calculation_entry(result,b);
                entry.writeToFile_division();
               
                 foreach (HostIPnPort h in activeNodes)
                {
                    if (!h.Equals(thisMachineIpnPort))
                    {
                        addOverRPC(h.getFullUrl(), entry.makeString());
                    }
                }
                TokenRing.sendToken();
            }
            else
            { 
                Console.WriteLine("Node is offline! Operation is not allowed");
            }
        }

        private void getListOfEvents(bool getFromAnotherNode)
        {
            
            
            if (getFromAnotherNode) //only when node joins the network
            {
               
                IServiceContract proxy = XmlRpcProxyGen.Create<IServiceContract>();
                proxy.Url = firstActiveNode.getFullUrl();
                Object[] list = proxy.getList();

                using (StreamWriter writer = new StreamWriter("Calendar.txt", false))
                {
                    for (int i = 0; i < list.Length; i++)
                    {
                        
                        writer.WriteLine(list[i].ToString());
                    }
                        
                }
                //if true - configuration of the xmlrpc and getting string[] Calendar.getList
                //clear the file
                //write to file new entries
            }
            else
            {
                //else read from the file and print
                try
                {
                    using (StreamReader sr = new StreamReader("Calendar.txt"))
                    {
                        String line = sr.ReadToEnd();
                        Console.WriteLine(line);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(e.Message);
                }
            }
        }

        private void join()
        {
            //if offline
            //make it online
            //input ip of active node, if it is first onlinenode input 17.0.0.1
            //if 127.0.0.1 then isOnline = true
            //else
            //get list of all events
            //ip to url
            //config xmlrpc
            //get list of active nodes
            //for all active nodes - joinoverrpc
            //else node is already online
            if (!isOnline)
            {
                isOnline = true;
                Console.WriteLine("Input IP-address of active node");
                Console.WriteLine("If it is the first online node input 127.0.0.1");
                String ipAddress = Console.ReadLine();
                if (!ipAddress.Equals("127.0.0.1"))
                {
                    //TODO set port
            	    Console.WriteLine("Enter the port for this machine : ");
                    int port = int.Parse(Console.ReadLine());
            	    firstActiveNode = new HostIPnPort(ipAddress, port);
            	
                    getListOfEvents(true);
                    IServiceContract proxy = XmlRpcProxyGen.Create<IServiceContract>();
                    proxy.Url = firstActiveNode.getFullUrl();
                    Object[] listOfNodes = proxy.getListOfActiveNodes();
                    for (int i = 0; i < listOfNodes.Length; i++)
                    {
                        HostIPnPort newObj = new HostIPnPort(listOfNodes[i].ToString());
                        int k = 0;
                        while (k < activeNodes.Count && activeNodes[k].compare(newObj) < 0) k++;
                        activeNodes.Insert(k, newObj);
                        //activeNodes.Add(list[i].ToString());
                    }
                    bool tokenRingStarter = false;
                    if (activeNodes.Count == 2)
                        tokenRingStarter = true;
                    foreach (HostIPnPort h in activeNodes)
                    {
                        if (!h.Equals(thisMachineIpnPort))
                            joinOverRPC(h.getFullUrl());
                    }
                    //config xnlrpc with firstactivenode to url
                    //call Node.getListOfActive Nodes
                    //add all nodes to the list of active nodes
                    //for all activenodes - joinoverRPC(s)
                    if (tokenRingStarter)
                        TokenRing.startTokenRingAlgorithm(); //for the time that you are starter of the token ring
                    if (activeNodes.Count > 2)
                        TokenRing.initiateTokenRing(); //for the time that token does not rotating in the Network Ring and it need to be monitored by this client
                }
                else
                {
                    isOnline = true;
                }
            }
            else
            {
                Console.WriteLine("Node is already online!");
            }
        }

        private void signOff()
        {
            if (isOnline)
            {
                isOnline = false;
                foreach (HostIPnPort h in activeNodes)
                {
                    if (!h.Equals(thisMachineIpnPort))
                        signOffOverRPC(h.getFullUrl());
                }
                TokenRing.stopTokenRingAlgorithm();
                activeNodes.Clear();
                activeNodes.Add(thisMachineIpnPort);
            }
            else
            {
                Console.WriteLine("Node is already online");
            }
        }

        private void signOffOverRPC(String fullUrl)
        {
            IServiceContract proxy = XmlRpcProxyGen.Create<IServiceContract>();
            proxy.Url = fullUrl;
            bool result = proxy.deleteNode(thisMachineIpnPort.getIPnPort());
         }

        private void addOverRPC(String fullUrl, String message)
        {
            IServiceContract proxy = XmlRpcProxyGen.Create<IServiceContract>();
            proxy.Url = fullUrl;
            int result = proxy.result_update(message);
         }

        private void joinOverRPC(String fullURL)
        {
            IServiceContract proxy = XmlRpcProxyGen.Create<IServiceContract>();
            proxy.Url = fullURL;
            bool result = proxy.addNode(thisMachineIpnPort.getIPnPort());
        }

     }
}
