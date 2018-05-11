using UnityEngine;
using System.Collections;

public class MQTT_Message : object {
	public string message;
	public string topic;
	public MQTT_Message(string Topic, string Message){
		topic = Topic;
		message = Message;
	}
}
