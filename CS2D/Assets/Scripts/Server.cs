﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

public class Server : MonoBehaviour {

	[Header("Connection")]
	public int serverPort;
	public int clientPort;
	private Channel channel;
	public float fakeDelay;
	public float fakePacketLoss;

	[Header("Game")]
	public Object playerPrefab;
	List<Player> players = new List<Player>();
	List<ServerMessage> outMessages = new List<ServerMessage>();

	int i = 0;

	void Start() {
		channel = new Channel(null, serverPort, clientPort);
	}

	void OnDestroy() {
		channel.Disconnect();
	}

	void Update() {
		if (i++ % 100 == 0) {
			return;
		}
		Packet inPacket = channel.GetPacket ();
		if (inPacket != null) {
			//read it!
			BitBuffer bitBuffer = inPacket.buffer;
			int messageCount = bitBuffer.GetInt ();
			for (int i = 0; i < messageCount; i++) {
				//parse message
				ClientMessage clientMessage = ReadClientMessage(bitBuffer, inPacket.fromEndPoint);
				if (clientMessage != null) {
					ProcessClientMessage(clientMessage);
				}
			}
		}

		SnapshotMessage snapshotMessage = new SnapshotMessage (BuildGameData());
		snapshotMessage.DelayToSend = fakeDelay;
		outMessages.Add (snapshotMessage);

		int messagesReadyToSend = 0;
		for (int i = 0; i < outMessages.Count; i++) {			
			ServerMessage serverMessage = outMessages [i];
			serverMessage.Update (Time.deltaTime);
			if (serverMessage.NeedsToBeSent) {
				messagesReadyToSend++;
			}
		}

		if (messagesReadyToSend > 0) {
			Packet outPacket = new Packet ();
			outPacket.buffer.PutInt (messagesReadyToSend);
			for (int i = 0; i < outMessages.Count; i++) {
				ServerMessage serverMessage = outMessages [i];
				if (serverMessage.NeedsToBeSent) {
					serverMessage.Save (outPacket.buffer);
					outMessages.RemoveAt (i);
					i--;
				}
			}

			outPacket.buffer.Flip ();
			bool shouldDropPacket = Random.Range (0.0001f, 100.0f) < fakePacketLoss;
			if (!shouldDropPacket) {
				for (int i = 0; i < players.Count; i++) {
					Player player = players [i];
					channel.Send (outPacket, player.endPoint);
				}		
			}
		}
	}

	ClientMessage ReadClientMessage(BitBuffer bitBuffer, IPEndPoint clientEndPoint) {
		ClientMessageType messageType = bitBuffer.GetEnum<ClientMessageType> ((int)ClientMessageType.TOTAL);
		ClientMessage clientMessage = null;
		switch (messageType) {
		case ClientMessageType.CONNECT_PLAYER:
			clientMessage = new ConnectPlayerMessage (clientEndPoint);
			break;
		case ClientMessageType.DISCONNECT_PLAYER:
			{
				Player player = GetPlayerWithEndPoint (clientEndPoint);
				if (player == null) {
					return null;
				}
				clientMessage = new DisconnectPlayerMessage (player.Id);
			} break;
		case ClientMessageType.PLAYER_INPUT:
			{
				Player player = GetPlayerWithEndPoint (clientEndPoint);
				if (player == null) {
					return null;
				}
				clientMessage = new PlayerInputMessage (player.Id);
			} break;
		default:
			Debug.LogError("Got a client message that cannot be understood");
			return null;
		}
		clientMessage.Load(bitBuffer);
		return clientMessage;
	}

	void ProcessClientMessage(ClientMessage clientMessage) {
		switch (clientMessage.Type) {
		case ClientMessageType.CONNECT_PLAYER:
			ProcessConnectPlayer(clientMessage as ConnectPlayerMessage);
			break;
		case ClientMessageType.DISCONNECT_PLAYER:
			ProcessDisconnectPlayer(clientMessage as DisconnectPlayerMessage);
			break;
		case ClientMessageType.PLAYER_INPUT:
			ProcessPlayerInput (clientMessage as PlayerInputMessage);
			break;
		}
	}

	public void ProcessConnectPlayer(ConnectPlayerMessage connectPlayerMessage) {
		int playerId = connectPlayerMessage.PlayerId;
		Player player = GetPlayerWithId (playerId);
		if (player != null) {
			DisconnectPlayer (player);
		}

		GameObject playerGO = Instantiate (playerPrefab) as GameObject;
		playerGO.name = "Player " + playerId; 
		player = playerGO.GetComponent<Player> ();
		player.endPoint = connectPlayerMessage.EndPoint;
		player.Id = playerId;
		players.Add(player);

		PlayerConnectedMessage playerConnectedMessage = new PlayerConnectedMessage (playerId);
		outMessages.Add (playerConnectedMessage);
	}	

	public void ProcessDisconnectPlayer(DisconnectPlayerMessage disconnectPlayerMessage) {
		int playerId = disconnectPlayerMessage.PlayerId;
		Player player = GetPlayerWithId (playerId);
		if (player != null) {
			DisconnectPlayer (player);
		}		

		PlayerDisconnectedMessage playerDisconnectedMessage = new PlayerDisconnectedMessage (playerId);
		outMessages.Add (playerDisconnectedMessage);
	}

	public void ProcessPlayerInput(PlayerInputMessage playerInputMessage) {
		Player player = GetPlayerWithId(playerInputMessage.PlayerId);
		if (player != null) {
			player.Input = playerInputMessage.Input;
		}
	}

	public void DisconnectPlayer(Player player) {
		Destroy(player.gameObject);
		players.Remove(player);
	}

	Player GetPlayerWithId(int playerId) {
		for (int i = 0; i < players.Count; i++) {
			if (players[i].Id == playerId) {
				return players[i];
			}
		}
		return null;
	}

	public Player GetPlayerWithEndPoint(IPEndPoint endPoint) {
		for (int i = 0; i < players.Count; i++) {
			if (players[i].endPoint.Equals(endPoint)) {
				return players[i];
			}
		}
		return null;
	}

	private GameData BuildGameData() {
		GameData gameData = new GameData ();
		gameData.Time = Time.realtimeSinceStartup;
		for (int i = 0; i < players.Count; i++) {
			gameData.Players.Add (players [i].BuildPlayerData ());
		}
		return gameData;
	}
}
