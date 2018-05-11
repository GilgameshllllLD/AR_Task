using System;
using System.Net;
using System.Net.Sockets;

public class UdpReceiver {
	private UdpClient mClient = null;
	private IPEndPoint mEndPoint = null;
	
    protected UdpReceiver()
    {
    }

    public static UdpReceiver NewUnicast(string _address, int port, int timeout = 0)
    {
        IPAddress address = IPAddress.Parse(_address);
        /*IPAddress any_address;
        // IPv4
        if (address.AddressFamily == AddressFamily.InterNetwork)
        {
            any_address = IPAddress.Any;
        }
        // IPv6
        else if (address.AddressFamily == AddressFamily.InterNetworkV6)
        {
            any_address = IPAddress.IPv6Any;
        }
        else
        {
            throw new System.Exception("Address (" + _address + ") must be either IPv4 or IPv6");
        }*/

        UdpReceiver ans = new UdpReceiver();
        ans.mClient = new UdpClient(port, address.AddressFamily);
//        ans.mClient.Client.Bind(new IPEndPoint(any_address, port));
        ans.mEndPoint = new IPEndPoint(address, port);
        if (timeout > 0)
        {
            ans.mClient.Client.ReceiveTimeout = timeout;
        }
        return ans;
    }

    public static UdpReceiver NewMulticast(string address, int port, int timeout)
    {
        IPAddress multicast_address = IPAddress.Parse(address);
		/*IPAddress any_address;
        // IPv4
        if (multicast_address.AddressFamily == AddressFamily.InterNetwork)
        {
            any_address = IPAddress.Any;
        }
        // IPv6
        else if (multicast_address.AddressFamily == AddressFamily.InterNetworkV6)
        {
            any_address = IPAddress.IPv6Any;
        }
        else
        {
            throw new System.Exception("Address (" + address + ") must be either IPv4 or IPv6");
        }*/

        UdpReceiver ans = new UdpReceiver();
        //IPEndPoint local_ep = new IPEndPoint(any_address, port);

        ans.mClient = new UdpClient(port, multicast_address.AddressFamily);
        ans.mClient.JoinMulticastGroup(multicast_address);
        ans.mEndPoint = new IPEndPoint(multicast_address, port);
        if (timeout > 0)
        {
            ans.mClient.Client.ReceiveTimeout = timeout;
        }

        return ans;
    }

    public int Available()
    {
        return mClient.Available;
    }

    public byte[] Receive()
    {
        try
        {
            return mClient.Receive(ref mEndPoint);
        }
        catch (System.Exception e)
        {
			throw new System.ArgumentException ("Error Recieving byte data: " + e.Message);
        }
    }

    public void Release(){
        mClient.Close ();
    }
}