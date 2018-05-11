using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using System;

public class Network_MQTT : MonoBehaviour {
	private MqttClient client;
	private Queue<MQTT_Message> messageQueue;

	public string ipAddress = "104.236.171.74";
	public int port = 1883;

	public delegate void OnRecieveMessage(MQTT_Message messageObject);
	public delegate void OnConnectionChange (bool connected);
	public OnRecieveMessage onRecieveMessage;
	public OnConnectionChange onConnectionChange;

	private string[] savedChannels;

	MqttClient _client;

	public bool connected{
		get {
			return client.IsConnected;
		}
	}

    void Awake() {
        messageQueue = new Queue<MQTT_Message>();
	}
	private void Update(){
		//traverse message queue and interperet
		if(messageQueue.Count > 0){
			MQTT_Message message = messageQueue.Dequeue();

			if(onRecieveMessage != null){
				onRecieveMessage(message);
			}
		}
        if(!client.IsConnected)
        {
            Debug.Log("Lose Connect");
        }

	}
    public void connect()
    {
		try{
	        Debug.Log("Connecting MQTT");
			CancelInvoke("connect");
	        // create client instance 
	        client = new MqttClient(ipAddress, port, false, null);

	        // register to message received 
	        client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
			client.MqttMsgDisconnected += client_MqttMsgDisconnected;

	        messageQueue = new Queue<MQTT_Message>();

			string clientId = Guid.NewGuid().ToString(); 
			client.Connect(clientId); 

			if(savedChannels != null){
				subscribe(savedChannels);
			}
			if(client.IsConnected){
				Debug.Log("Connected MQTT"+ "SERVER is" + ipAddress);
			}else{
				Debug.Log("Unable to connect to MQTT");
				startReconnect ();
			}
		}catch(System.Exception e){
			Debug.LogError ("Error connecting to MQTT: " + e.Message);
			startReconnect ();
		}
	}
	private void startReconnect(){
		InvokeRepeating ("connect", 5,5);
	}
    public void subscribe(string[] channels)
    {
		if (client != null && client.IsConnected) {
			Debug.Log ("Channels: " + channels [0]);
			client.Subscribe (channels, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
			savedChannels = null;
		} else {
			savedChannels = channels;
		}
    }
	private void client_MqttMsgDisconnected(object sender, System.EventArgs e){
		Debug.Log ("Lost MQTT Connection");
		if (onConnectionChange != null) {
			onConnectionChange (connected);
		}
	}
	private void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) 
	{ 
		messageQueue.Enqueue(new MQTT_Message(e.Topic, System.Text.Encoding.UTF8.GetString(e.Message)));
	} 
	public void sendMessage(MQTT_Message message){
		sendMessage (message.topic, message.message);
	}
	public void sendMessage(string Channel, string Payload, bool retain=false){
		//Debug.Log ("Sending Message Channel: " + Channel + " Message: " + Payload);
		client.Publish(Channel, System.Text.Encoding.UTF8.GetBytes(Payload), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, retain);
	}
}
