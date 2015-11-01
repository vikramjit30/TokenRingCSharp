using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookComputing.XmlRpc;

namespace MyCalendar
{
    public interface OperationInterface
    {
          
        [XmlRpcMethod("Calculator.result_update")]
        int result_update(string result);

        [XmlRpcMethod("Node.add")]
        bool addNode(string ipAddress);

        [XmlRpcMethod("Node.delete")]
        bool deleteNode(string ipAddress);

        [XmlRpcMethod("Node.getListOfActiveNode")]
        Object[] getListOfActiveNodes();

        [XmlRpcMethod("TokenRing.takeTheToken")]
        int takeTheToken(int ack);

        }
}
