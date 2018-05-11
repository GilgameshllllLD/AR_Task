/*
    -----------------------
    UDP-Receive (send to)
    -----------------------
    // [url]http://msdn.microsoft.com/de-de/library/bb979228.aspx#ID0E3BAC[/url]
   
   
    // > receive
    // 127.0.0.1 : 8051
   
    // send
    // nc -u 127.0.0.1 8051
 
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Network_UDP_Reciever : PortConnection {
	private byte[] byteData;

	// receiving Thread
	private Thread receiveThread;
	private UdpReceiver udp;

	public delegate void OnRecieveData(byte[] data);
	public OnRecieveData onRecieveData;
	private Stack<byte[]> messageQueue;

	// init
	public override void initializePort()
	{
		base.initializePort ();
		messageQueue = new Stack<byte[]> ();
		// Define local endpoint ( where messages are received ).
		// Create a new thread to receive incoming messages.
		receiveThread = new Thread(new ThreadStart(ReceiveData));
		receiveThread.IsBackground = true;
		receiveThread.Start();
		
	}
	private void Update(){
		if (connected && messageQueue != null) {
			if (messageQueue.Count > 0) {
				if (onRecieveData != null) {
					onRecieveData (messageQueue.Pop ());
					if (messageQueue.Count > 0) {
						Debug.LogWarning ("Dropped Message Frame");
						messageQueue.Clear ();
					}
				} else {
					messageQueue.Clear();
				}
			}
		}
	}
	// receive thread
	private  void ReceiveData()
	{
		string realIp = ip;
		log("Connecting to: " + realIp, true);

		int ipTest = int.Parse(realIp.Split('.')[0]);
		if (ipTest < 224) {
			Debug.Log ("Using Unicast");
			udp = UdpReceiver.NewUnicast (realIp, port, 0);
		} else {
			Debug.Log ("Using Multicast");
			udp = UdpReceiver.NewMulticast (realIp, port, 0);
		}
		
		log("Connected to: " + realIp,true);
		//Infinite Loop processing data
		while (true)
		{
			try
			{
				Loop ();
			}
			catch (Exception err)
			{
				print(err.ToString());
				log("Connection Error");
			}
		}
	}

	private void Loop()
	{
		byteData = udp.Receive();
		messageQueue.Push (byteData);
		System.Threading.Thread.Sleep(1);
	}

	void OnDisable() 
	{ 
		if ( receiveThread!= null) 
			receiveThread.Abort(); 
		Debug.Log ("Closing UDP");
		if (udp != null) {
			udp.Release ();
		}
	} 
}