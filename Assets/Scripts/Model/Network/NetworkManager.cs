using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

public abstract class NetworkManager : MessageAdapter {

	protected IChannel<Datagram> channel;
	protected Serializer serializer;
	private int port;

	public NetworkManager (int port) {
		channel = new UdpChannel ();
		channel.Open (port);
		serializer = new Serializer ();
	}

	public void Send (IBitBufferSerializable obj, IPEndPoint endPoint) {
		Debug.Log ("Sending: " + obj);
		channel.Send (buildDatagram (serializer.Serialize (obj), endPoint));
	}

	public virtual ParsedDatagram Receive () {
		Datagram datagram = channel.Receive ();
		if (datagram != null) {
			Message message = serializer.Deserialize (datagram.bytes);
			Debug.Log ("Received: " + message);
			MessageMulticaster.Instance.onReceived(message);
			Debug.Log ("Returning not null :(");
			return new ParsedDatagram (datagram.endPoint, message);
		}
		return null;
	}

	public void Close () {
		channel.Close ();
	}

	private Datagram buildDatagram (byte[] bytes, IPEndPoint endPoint) {
		return new Datagram (bytes, endPoint);
	}
}
