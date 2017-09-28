using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour {

	public int serverPort;
	public int clientPort;
	public Object playerPrefab;
	Channel channel;

	List<PlayerNetworkView> players = new List<PlayerNetworkView>();
	public int playerId;

	void Start() {
		channel = new Channel("127.0.0.1", clientPort, serverPort);
	}

	void OnDestroy() {
		channel.Disconnect();
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Space)) {
			//send player connect message
			Packet p = new Packet();
			ConnectPlayerMessage connectPlayerMessage = new ConnectPlayerMessage(playerId);
			p.buffer.PutInt(1);
			connectPlayerMessage.Save(p.buffer);
			p.buffer.Flip ();
			channel.Send(p);
		}

		Packet inPacket = channel.GetPacket ();
		if (inPacket != null) {
			Debug.Log (inPacket);
			BitBuffer bitBuffer = inPacket.buffer;
			int messageCount = bitBuffer.GetInt ();
			for (int i = 0; i < messageCount; i++) {
				//parse message
				ServerMessage serverMessage = ReadServerMessage(bitBuffer);
				if (serverMessage != null) {
					ProcessClientMessage(serverMessage);
				}
			}
		}	
	}

	ClientMessage ReadServerMessage(BitBuffer bitBuffer) {
		ServerMessageType messageType = bitBuffer.GetEnum<ServerMessageType> ((int)ServerMessageType.TOTAL);
		ServerMessage serverMessage = null;
		switch (messageType) {
		case ServerMessageType.PLAYER_CONNECTED:
			serverMessage = new PlayerConnectedMessage ();
			break;
		case ServerMessageType.PLAYER_DISCONNECTED:
			serverMessage = new PlayerDisconnectedMessage ();
			break;
		case ServerMessageType.SNAPSHOT:
			serverMessage = new SnapshotMessage ();
			break;
		default:
			Debug.LogError("Got a server message that cannot be understood");
			return null;
		}
		serverMessage.Load(bitBuffer);
		return serverMessage;
	}

	void ProcessServerMessage(ServerMessage serverMessage) {
		switch (serverMessage.Type) {
		case ServerMessageType.PLAYER_CONNECTED:
			ProcessPlayerConnected(serverMessage as PlayerConnectedMessage);
			break;
		case ServerMessageType.PLAYER_DISCONNECTED:
			ProcessPlayerDisconnected(serverMessage as PlayerDisconnectedMessage);
			break;
		case ServerMessageType.SNAPSHOT:
			ProcessSnapshot(serverMessage as SnapshotMessage);
			break;
		}
	}

	public void ProcessPlayerConnected(PlayerConnectedMessage message) {
		int playerId = message.PlayerId;
		PlayerNetworkView player = GetPlayerWithId (playerId);
		if (player != null) {
			DisconnectPlayer (player);
		}

		GameObject playerGO = Instantiate (playerPrefab) as GameObject;
		playerGO.name = "Player " + playerId; 
		player = playerGO.GetComponent<PlayerNetworkView> ();
		if (playerId.Equals(this.playerId)) {
			player = playerGO.AddComponent<PlayerController> ();
		}
		players.Add(PlayerNetworkView);
	}

	public void ProcessPlayerDisconnected(PlayerDisconnectedMessage message) {
		// TODO
	}	

	public void ProcessSnapshot(SnapshotMessage snapshot) {
		GameData gameData = snapshot.GameSnapshot;
		List<PlayerData> playerDatas = gameData.Players;
		foreach (PlayerData playerData in playerDatas) {
			playerData.position;
		}
	}

	public void DisconnectPlayer(Player player) {
		Destroy(player.gameObject);
		players.Remove(player);
	}

	Player GetPlayerWithId(int playerId) {
		for (int i = 0; i < players.Count; i++) {
			if (players[i].id == playerId) {
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
}
