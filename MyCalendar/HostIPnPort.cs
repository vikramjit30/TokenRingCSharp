using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCalendar
{
    public class HostIPnPort 
{
	private String Ip;
	private int port;
	public String getIp() {
		return Ip;
	}
	public void setIp(String ip) {
		Ip = ip;
	}
	public int getPort() {
		return port;
	}
	public void setPort(int port) {
		this.port = port;
	}
	public HostIPnPort(String ip, int port) {
		Ip = ip;
		this.port = port;
	}
	public HostIPnPort(String IpnPort) {
		String[] obj = IpnPort.Split(':');
		this.Ip = obj[0];
		this.port = Convert.ToInt32(obj[1]);
	}
	public String getIPnPort()
	{
		return this.Ip+":"+this.port;
	}
	public String getFullUrl()
	{
		return "http://"+this.Ip+":"+this.port+"/xmlrpc";
	}
	public String toString() {
		return "http://"+this.Ip+":"+this.port+"/";
	}
	public long getHostId()
	{
		String[] parts = this.Ip.Split('.');
		String ID = "";
		for(int i=0;i<parts.Length;i++)
			ID += parts[i];
		ID += port;
		return Convert.ToInt64(ID);
	}
	public int compare(HostIPnPort obj2)
	{
		if(this.getHostId()<obj2.getHostId())
			return -1;
		if(this.getHostId()==obj2.getHostId())
			return 0;
		else
			return 1;
	}
}
}
