using System;
using System.Net;
using System.Net.Sockets;

public class UdpSender
{
    private UdpClient Client = null;
    private IPEndPoint Endpoint = null;

    public static UdpSender NewUnicast(string _address, int port)
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
        }
		*/
        UdpSender ans = new UdpSender();
        try
        {
            ans.Client = new UdpClient(address.AddressFamily);
            ans.Endpoint = new IPEndPoint(address, port);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine("UdpSender error=" + ex.Message);
        }
        return ans;
    }

    public static UdpSender NewMulticast(string _address, int port)
    {
        return NewUnicast(_address, port);
    }

    public int Send(byte[] dgram, int bytes)
    {
        try
        {
            return Client.Send(dgram, bytes, Endpoint);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine("UdpSender::Send error=" + ex.Message);
        }
        return 0;
    }
}
