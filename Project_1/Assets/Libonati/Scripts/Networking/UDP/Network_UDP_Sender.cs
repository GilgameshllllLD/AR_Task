using UnityEngine;
using System.Collections;
using SimpleJSON;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Network_UDP_Sender : PortConnection
{
	private UdpSender udp;
	public override void initializePort ()
	{
		base.initializePort ();
		udp = UdpSender.NewUnicast (ip, port);
	}

	public void SendMessage(byte[] data){
#if UNITY_EDITOR
		//Debug.Log ("Sending Message: " + System.Text.Encoding.ASCII.GetString (data));
#endif
		udp.Send (data, data.Length);
	}
}