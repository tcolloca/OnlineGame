using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Client : MonoBehaviour {

	public int serverPort;
	public int clientPort;
	public GameObject playerPrefab;
	private Channel channel;
	private bool isConnected = false;
	private CommunicationManager communicationManager = new CommunicationManager ();

	List<PlayerNetworkView> players = new List<PlayerNetworkView>();
	public int playerId;

	private PlayerController ownPlayer;

	public int buffDesiredLength;
	public double maxDiffTime;

	public double simSpeed;

	public double frameRate;
	private double simIniTime;
	private double simTime;
	private List<GameData> snapshots = new List<GameData> ();

	void Start() {
		channel = new Channel("127.0.0.1", clientPort, serverPort);
	}

	void OnDestroy() {
		channel.Disconnect();
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Space) && !isConnected) {
			communicationManager.SendMessage (ConnectPlayerMessage.CreateConnectPlayerMessageToSend (playerId));
		}

		if (Input.GetKeyDown(KeyCode.Escape) && isConnected) {
			communicationManager.SendMessage (
				new DisconnectPlayerMessage (UnityEngine.Random.Range (0, int.MaxValue), playerId));
			DisconnectPlayer (GetPlayerWithId (playerId));
			simIniTime = 0;
			ownPlayer = null;
			isConnected = false;
		}

		ReadMessages ();	

		if (simIniTime != 0) {
			if (Input.GetKey (KeyCode.K)) {
				;
			} else {
				Interpolate ();
			}
		}

		if (ownPlayer != null) {
			// outMessages.Add(new PlayerInputMessage (playerId, ownPlayer.playerInput));
			communicationManager.SendMessage (new PlayerInputMessage (playerId, ownPlayer.playerInput));
		}

		//SendMessages ();
		Packet outPacket = communicationManager.BuildPacket ();
		if (outPacket != null) {
			channel.Send (outPacket);
		}
	}

	Message ReadServerMessage(BitBuffer bitBuffer) {
		MessageType messageType = bitBuffer.GetEnum<MessageType> ((int)MessageType.TOTAL);
		Message serverMessage = null;
		switch (messageType) {
		case MessageType.PLAYER_CONNECTED:
			serverMessage = PlayerConnectedMessage.CreatePlayerConnectedMessageToReceive ();
			break;
		case MessageType.PLAYER_DISCONNECTED:
			serverMessage = PlayerDisconnectedMessage.CreatePlayerDisconnectedMessageToReceive ();
			break;
		case MessageType.SNAPSHOT:
			serverMessage = new SnapshotMessage ();
			break;
		case MessageType.ACK_RELIABLE_MAX_WAIT_TIME:
			serverMessage = AckReliableMessage.CreateAckReliableMessageMessageToReceive ();			
			break;
		case MessageType.ACK_RELIABLE_SEND_EVERY_PACKET:
			serverMessage = AckReliableSendEveryFrameMessage.CreateAckReliableSendEveryFrameMessageMessageToReceive ();
			break;
		default:
			Debug.LogError("Got a server message that cannot be understood");
			return null;
		}
		serverMessage.Load(bitBuffer);
		return serverMessage;
	}

	void ProcessServerMessage(Message serverMessage) {
		switch (serverMessage.Type) {
		case MessageType.PLAYER_CONNECTED:
			ProcessPlayerConnected(serverMessage as PlayerConnectedMessage);
			break;
		case MessageType.PLAYER_DISCONNECTED:
			ProcessPlayerDisconnected(serverMessage as PlayerDisconnectedMessage);
			break;
		case MessageType.SNAPSHOT:
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
		PlayerNetworkView player = GetPlayerWithId (message.PlayerId);
		if (player != null) {
			DisconnectPlayer (player);
		}
	}	

	public void ProcessSnapshot(SnapshotMessage snapshot) {
		GameData gameData = snapshot.GameSnapshot;
		if (simIniTime == 0) {
			simIniTime = gameData.Time;
		}
		snapshots.Add (gameData);
		snapshots.OrderBy (snap => snap.Time).ToList ();
	}

	public void Interpolate () {

		UpdateSimSpeed ();

		simTime += Time.deltaTime * simSpeed;

		RemoveOldSnapshots ();

		UpdateSimSpeed ();

		if (snapshots.Count > 1) {
			GameData start;
			GameData end;
			if (snapshots[snapshots.Count - 1].Time - simIniTime - simTime > maxDiffTime) {
				start = end = snapshots [snapshots.Count - 1];
				simTime = end.Time - simIniTime;
			} else {
				start = snapshots [0];
				end = snapshots[1];
			}
			GameData interpolated = InterpolateDatas (start, end);
			List<PlayerData> playerDatas = interpolated.Players;
			foreach (PlayerData playerData in playerDatas) {
				int playerId = playerData.PlayerId;
				PlayerNetworkView player = GetPlayerWithId (playerId);
				if (player == null) {
					ConnectPlayer (playerId);
				}
				player.UpdatePosition (playerData.Position);
			}
		}
	}

	private GameData InterpolateDatas (GameData start, GameData end) {
		if (end.Time == start.Time) {
			return end;
		}
		float p = (float) (simTime - (start.Time - simIniTime)) / (end.Time - start.Time);
		List<PlayerData> interpolatedPlayerDatas = new List<PlayerData> ();
		// TODO : Player is gone from one snap to another
		for (int i = 0; i < end.Players.Count; i++) {
			PlayerData endPlayer = end.Players [i];
			Vector2 interPosition = Vector2.zero;
			foreach (PlayerData playerData in start.Players) {
				if (playerData.PlayerId.Equals (endPlayer.PlayerId)) {
					interPosition = Vector2.Lerp (playerData.Position, endPlayer.Position, p);
					break;
				}
			}
			PlayerData interData = new PlayerData ();
			interData.PlayerId = endPlayer.PlayerId;
			interData.Position = interPosition;
			interpolatedPlayerDatas.Add (interData);
		}
		GameData interGameData = new GameData ();
		interGameData.playersData = interpolatedPlayerDatas;
		interGameData.Time = (float) simTime;
		return interGameData;
	}

	private void RemoveOldSnapshots () {
		if (snapshots.Count == 0) {
			return;
		}
		GameData prev = null;
		GameData interpolated = snapshots [0];
		while (simTime > interpolated.Time - simIniTime && snapshots.Count > 0) {
			prev = interpolated;
			interpolated = snapshots [0];
			if (simTime > interpolated.Time - simIniTime) {
				snapshots.RemoveAt (0);
			}
		}
		if (prev != null) {
			snapshots.Insert (0, prev);
		}
	}

	private void UpdateSimSpeed() {
		double rawFactor = 1 - 1.0 / (Mathf.Abs(buffDesiredLength - snapshots.Count) + 1);
		double maxFactor = 1.1;
		double factor = (maxFactor - 1) * rawFactor + 1;
		if (snapshots.Count < buffDesiredLength) {
			if (simSpeed > 1) {
				simSpeed = 1;
			}
			simSpeed /= factor; 
		} else if (snapshots.Count > buffDesiredLength) {
			if (simSpeed < 1) {
				simSpeed = 1;
			}
			simSpeed *= factor;
		} else {
			simSpeed = 1;
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
			isConnected = true;
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
		Packet inPacket;

		while ((inPacket = channel.GetPacket ()) != null) {
			BitBuffer bitBuffer = inPacket.buffer;
			int messageCount = bitBuffer.GetInt ();
			for (int i = 0; i < messageCount; i++) {
				//parse message
				Message serverMessage = ReadServerMessage(bitBuffer);
				if (serverMessage != null) {
					communicationManager.ReceiveMessage (serverMessage);
				}
			}
		}

		Message inMessage;
		while ((inMessage = communicationManager.GetMessage ()) != null) {
			ProcessServerMessage (inMessage);
		}
	}
}
