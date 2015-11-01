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
using System.Threading;

namespace MyCalendar
{
   
    class TokenSender
    {
        private HostIPnPort nextNodeOnRing;

        public TokenSender(HostIPnPort nextNodeOnRing)
        {
            this.nextNodeOnRing = nextNodeOnRing;
        }
        public void run()
        {
            try 
		    {
			    int ack = 5;
                IServiceContract proxy = XmlRpcProxyGen.Create<IServiceContract>();
                proxy.Url = nextNodeOnRing.getFullUrl();
                int respond = proxy.takeTheToken(ack);
	            if(respond != ack +1)
                     Console.WriteLine("Token Ring algorithm has failed.");
		    } catch (Exception e) 
            {
                Console.WriteLine("Token Ring algorithm has failed.");
                Console.WriteLine(e.Message);
		    }
        }
    }
}
