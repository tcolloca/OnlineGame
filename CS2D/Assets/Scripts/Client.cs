using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour {

	public int serverPort;
	public int clientPort;
	public Object playerPrefab;
	Channel channel;
	List<ClientMessage> outMessages = new List<ClientMessage>();

	List<PlayerNetworkView> players = new List<PlayerNetworkView>();
	public int playerId;

	private PlayerController ownPlayer;

	void Start() {
		channel = new Channel("127.0.0.1", clientPort, serverPort);
	}

	void OnDestroy() {
		channel.Disconnect();
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Space)) {
			//send player connect message
			outMessages.Add(new ConnectPlayerMessage(playerId));
		}

		ReadMessages ();	

		if (ownPlayer != null) {
			outMessages.Add(new PlayerInputMessage (ownPlayer.playerInput));
		}

		SendMessages ();
	}

	ServerMessage ReadServerMessage(BitBuffer bitBuffer) {
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
		PlayerNetworkView player = GetPlayerWithId (message.PlayerId);
		if (player != null) {
			DisconnectPlayer (player);
		}
		ConnectPlayer (message.PlayerId);
	}

	public void ProcessPlayerDisconnected(PlayerDisconnectedMessage message) {
		// TODO
	}	

	public void ProcessSnapshot(SnapshotMessage snapshot) {
		GameData gameData = snapshot.GameSnapshot;
		List<PlayerData> playerDatas = gameData.Players;
		foreach (PlayerData playerData in playerDatas) {
			int playerId = playerData.PlayerId;
			PlayerNetworkView player = GetPlayerWithId (playerId);
			if (player == null) {
				ConnectPlayer (playerId);
			}
			player.UpdatePosition (playerData.Position);

		}
	}

	private void ConnectPlayer (int playerId) {
		PlayerNetworkView player = GetPlayerWithId (playerId);
		if (player != null) {
			DisconnectPlayer (player);
		}

		GameObject playerGO = Instantiate (playerPrefab) as GameObject;
		playerGO.name = "Player " + playerId; 
		player = playerGO.GetComponent<PlayerNetworkView> ();
		player.id = playerId;
		if (playerId.Equals(this.playerId)) {
			ownPlayer = playerGO.AddComponent<PlayerController> ();
			ownPlayer.playerInput = new PlayerInput ();
		}
		players.Add(player);
	}

	private void DisconnectPlayer(PlayerNetworkView player) {
		Destroy(player.gameObject);
		players.Remove(player);
	}

	private PlayerNetworkView GetPlayerWithId(int playerId) {
		for (int i = 0; i < players.Count; i++) {
			if (players[i].id == playerId) {
				return players[i];
			}
		}
		return null;
	}

	private void ReadMessages () {
		Packet inPacket = channel.GetPacket ();
		if (inPacket != null) {
			Debug.Log (inPacket);
			BitBuffer bitBuffer = inPacket.buffer;
			int messageCount = bitBuffer.GetInt ();
			for (int i = 0; i < messageCount; i++) {
				//parse message
				ServerMessage serverMessage = ReadServerMessage(bitBuffer);
				if (serverMessage != null) {
					ProcessServerMessage(serverMessage);
				}
			}
		}
	}

	private void SendMessages () {
		if (outMessages.Count <= 0) {
			return;
		}
		Packet outPacket = new Packet ();
		outPacket.buffer.PutInt (outMessages.Count);
		for (int i = 0; i < outMessages.Count; i++) {
			ClientMessage clientMessage = outMessages [i];
			clientMessage.Save (outPacket.buffer);
			outMessages.RemoveAt (i);
			i--;
		}

		outPacket.buffer.Flip ();
		Debug.Log ("Sending from client: " + outPacket);
		channel.Send (outPacket);				
	}
}
