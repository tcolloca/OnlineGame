using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Client : MonoBehaviour {

	public int serverPort;
	public int clientPort;
	public GameObject playerPrefab;
	Channel channel;
	List<ClientMessage> outMessages = new List<ClientMessage>();

	List<PlayerNetworkView> players = new List<PlayerNetworkView>();
	public int playerId;

	private PlayerController ownPlayer;

	public int buffDesiredLength;

	public double simSpeed;

	public double frameRate;
	private int frame = 1;
	private double iniTime;
	private double simIniTime;
	private double simTime;
	List<GameData> snapshots = new List<GameData> ();

	int i = 0;

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

		if (simIniTime != 0) {
			if (Input.GetKey (KeyCode.K)) {
				;
			} else {
				Interpolate ();
			}
		}

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
		if (simIniTime == 0) {
			simIniTime = gameData.Time;
		}
		snapshots.Add (gameData);
		snapshots.OrderBy (snap => snap.Time).ToList ();
	}

	public void Interpolate () {
		Debug.Log ("snaps count: " + snapshots.Count);
		Debug.Log ("speed: " + simSpeed);

		UpdateSimSpeed ();

		simTime += Time.deltaTime * simSpeed;

		RemoveOldSnapshots ();

		UpdateSimSpeed ();

		if (snapshots.Count > 1) {
			GameData start = snapshots [0];
			GameData end = snapshots [1];
			//GameData interpolated = start;
			GameData interpolated = InterpolateDatas (start, end);
			Debug.Log ("Using: " + (interpolated.Time - simIniTime));
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
/*
		simTime = Time.realtimeSinceStartup - iniTime; 
		if (snapshots.Count < buffDesiredLength && simSpeed > 1
			|| snapshots.Count > buffDesiredLength && simSpeed < 1) {
			simSpeed = 1;
		}
		double expectedSimTime = frame * frameRate * simSpeed;
		Debug.Log ("simSpeed: " + simSpeed);
		Debug.Log ("simTime: " + simTime);
		Debug.Log ("frameTime: " + expectedSimTime);
		Debug.Log ("frame: " + frame);

		Debug.Log ("snap count: " + snapshots.Count);
		UpdateSimSpeed (expectedSimTime, simTime);

		if (expectedSimTime > simTime || snapshots.Count == 0) {
			if (snapshots.Count > buffDesiredLength) {
				simSpeed = 1;
			}
			Debug.Log ("nothing to do...");
			return;
		}

		RemoveOldSnapshots (expectedSimTime);

		GameData interpolated = snapshots [0];
		snapshots.RemoveAt (0);
		Debug.Log ("Using: " + (interpolated.Time - simIniTime));
		List<PlayerData> playerDatas = interpolated.Players;
		foreach (PlayerData playerData in playerDatas) {
			int playerId = playerData.PlayerId;
			PlayerNetworkView player = GetPlayerWithId (playerId);
			if (player == null) {
				ConnectPlayer (playerId);
			}
			player.UpdatePosition (playerData.Position);
		}
		expectedSimTime = interpolated.Time - simIniTime;
		frame = (int) Math.Max(frame, expectedSimTime / frameRate); */
	}

	private GameData InterpolateDatas (GameData start, GameData end) {
		Debug.Log ("start gd: " + (start.Time - simIniTime));
		Debug.Log ("end gd: " + (end.Time - simIniTime));
		Debug.Log ("sim: " + simTime);
		if (end.Time == start.Time) {
			return end;
		}
		float p = (float) (simTime - (start.Time - simIniTime)) / (end.Time - start.Time);
		Debug.Log ("prop: " + p);
		List<PlayerData> interpolatedPlayerDatas = new List<PlayerData> ();
		// TODO : Player is gone from one snap to another
		for (int i = 0; i < end.Players.Count; i++) {
			PlayerData endPlayer = end.Players [i];
			Vector2 interPosition = Vector2.zero;
			foreach (PlayerData playerData in start.Players) {
				if (playerData.PlayerId.Equals (endPlayer.PlayerId)) {
					Debug.Log ("start pos: " + playerData.Position);
					Debug.Log ("end pos: " + endPlayer.PlayerId);
					interPosition = Vector2.Lerp (playerData.Position, endPlayer.Position, p);
					break;
				}
			}
			Debug.Log ("inter pos: " + interPosition);
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
				Debug.Log ("Removing useless. snap Time: " + (interpolated.Time - simIniTime));
				snapshots.RemoveAt (0);
			}
		}
		if (prev != null) {
			Debug.Log ("Reinserting: " + (prev.Time - simIniTime));
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
/*		if (buffDesiredLength == snapshots.Count) {
			return;
		}
		int freshSnapsCount = snapshots.FindAll (snap => snap.Time - simIniTime >= limitTime).Count;
		double rawFactor = 1 - 1.0 / (Mathf.Abs(buffDesiredLength - freshSnapsCount) + 1);
		double minSpeedLimit = 0.05;
		double maxSpeedLimit = 10;
		double maxFactor = 1.01;
		double factor = (maxFactor - 1) * rawFactor + 1;

		Debug.Log ("factor: " + factor);

		if (snapshots.Count < buffDesiredLength && simSpeed > minSpeedLimit) {
			simSpeed /= factor;
		} else if (snapshots.Count >= buffDesiredLength && simSpeed < maxSpeedLimit) {
			if (limitTime > simTime) {
				Debug.Log ("WTF!!!!: " + (snapshots [0].Time - simIniTime));
			}
			simSpeed *= factor;
		} */
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
		Packet inPacket;
		while ((inPacket = channel.GetPacket ()) != null) {
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
		channel.Send (outPacket);				
	}
}
