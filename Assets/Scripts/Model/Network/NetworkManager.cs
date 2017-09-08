using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

public class NetworkManager {

	private IChannel<Datagram> channel;
	private int serverPort;
	private Serializer serializer;

	public NetworkManager (int clientPort, int serverPort) {
		channel = new UdpChannel ();
		channel.Open (clientPort);
		this.serverPort = serverPort;
		serializer = new Serializer ();
	}

	public void Close () {
		channel.Close ();
	}

	public void Send (ISerializable obj) {
		channel.Send (buildDatagram (serializer.Serialize (obj)));
	}

	public ISerializable Receive () {
		Datagram datagram = channel.Receive ();
		if (datagram != null) {
			return serializer.Deserialize (datagram.bytes);
		}
		return null;
	}

	private Datagram buildDatagram (byte[] bytes) {
		return new Datagram (bytes, new IPEndPoint (IPAddress.Parse("127.0.0.1"), serverPort));
	}
}
