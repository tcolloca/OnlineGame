using System;
using UnityEngine;
using System.Net;

public class Client : NetworkManager {

	public bool hasJoined { get; private set; }

	private int playerId;
	private int serverPort;
	private MainPlayer player;

	public Client (int port, int playerId) : base (port) {
		this.playerId = playerId;
		this.hasJoined = false;
		MessageMulticaster.Instance.AddListener (this);
	}
		
	public Client ConnectTo (int serverPort) {
		this.serverPort = serverPort;
		return this;
	}

	public void RecordInput () {
		if (player != null) {
			player.RecordInput ();
		}
	}

	public override void onPlayerJoined (PlayerJoinedMessage message) {
		Debug.Log ("PLAYER JOINED! " + message.playerId);
		if (message.playerId == playerId) {
			player = new MainPlayer(PlayerDatabase.Instance.GetPlayer (playerId));
			hasJoined = true;
		}
	}

	public void Join () {
		Send (new JoinMessage (playerId));
	}

	public void Send (IBitBufferSerializable obj) {
		Send (obj, new IPEndPoint (IPAddress.Parse("192.168.0.116"), serverPort));
	}
}

