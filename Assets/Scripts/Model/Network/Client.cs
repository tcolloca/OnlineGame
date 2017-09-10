using System;
using UnityEngine;
using System.Net;

public class Client : NetworkManager {

	public bool hasJoined { get; private set; }

	private int playerId;
	private int serverPort;

	public Client (int port, int playerId) : base (port) {
		this.playerId = playerId;
		this.hasJoined = false;
	}
		
	public Client ConnectTo (int serverPort) {
		this.serverPort = serverPort;
		return this;
	}

	public override void onPlayerJoined (PlayerJoinedMessage message) {
		Debug.Log ("PLAYER JOINED! " + message.playerId);
		if (message.playerId == playerId) {
			hasJoined = true;
		}
	}

	public void Join () {
		Send (new JoinMessage (playerId));
	}

	public void Send (ISerializable obj) {
		Send (obj, new IPEndPoint (IPAddress.Parse("127.0.0.1"), serverPort));
	}
}

