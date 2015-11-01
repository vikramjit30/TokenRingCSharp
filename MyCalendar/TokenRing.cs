using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyCalendar
{
    public class TokenRing
    {
        private static TokenRing instance = new TokenRing(); //just for synchronization locking
        private static bool tokenRingExistance = false;
        private static bool haveToken = false;
        private static bool wantToken = false;

        internal static void initiateTokenRing()
        {
            if (!tokenRingExistance)
            {
                tokenRingExistance = true;
                haveToken = false;
                wantToken = false;
            }
        }
        internal static void startTokenRingAlgorithm()
        {
            Console.WriteLine("### Token Ring running ###");
            tokenRingExistance = true;
            haveToken = false;
            wantToken = false;
            sendToken();
        }
        internal static void stopTokenRingAlgorithm()
        {
            Console.WriteLine("### Token Ring stopped ###");
            tokenRingExistance = false;
            haveToken = true;
            wantToken = false;
        }
        internal static void waitForToken()
        {
            Console.WriteLine("### Waiting for receiving token ###");
            if (!tokenRingExistance)
            {
                Console.WriteLine("### Token Ring is not running ###");
                return;
            }
            else
            {
                wantToken = true;
                bool flag = false;
                while (!flag)
                    lock (instance)
                    {
                        flag = haveToken;
                    }
            }

        }
        internal static void sendToken()
        {
            if (tokenRingExistance)
            {
                haveToken = false;
                wantToken = false;
                TokenSender newClient = new TokenSender(Client.nextHostOnRing());
                Thread oThread1 = new Thread(new ThreadStart(newClient.run));
                oThread1.Start();
            }
        }
        public static int takeTheToken(int ack)
        {
            tokenRingExistance = true;
            lock (instance)
            {
                haveToken = true;
            }
            if (!wantToken)
                sendToken();
            return ack + 1;
        }
    }
}
