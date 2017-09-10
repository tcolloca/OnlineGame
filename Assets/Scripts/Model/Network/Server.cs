using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Server : NetworkManager {

	private HashSet<IPEndPoint> clientEndPoints;

	public Server(int port) : base (port) {
		clientEndPoints = new HashSet<IPEndPoint> ();
		MessageMulticaster.Instance.AddListener (this);
	}

	public override ParsedDatagram Receive () {
		Datagram datagram = channel.Receive ();
		if (datagram != null) {
			Message message = serializer.Deserialize (datagram.bytes);
			Debug.Log ("Received Message: " + message);
			if (message is JoinMessage) {
				Debug.Log ("Join");
				clientEndPoints.Add (datagram.endPoint);
			} else if (message is LeaveMessage) {
				clientEndPoints.Remove (datagram.endPoint);
			}
			MessageMulticaster.Instance.onReceived(message);
			return new ParsedDatagram (datagram.endPoint, message);
		}
		return null;
	}

	public void SendSnapshot () {
		Dictionary<int, Position> playerPositions = new Dictionary<int, Position> ();
		foreach (Player player in PlayerDatabase.Instance.Players ()) {
			playerPositions.Add (player.id, player.position);
		}
		Broadcast (new SnapshotMessage (playerPositions));
	}

	public override void onJoin (JoinMessage joinMessage) {
		Debug.Log ("Sending joined (on join)!");
		Broadcast (new PlayerJoinedMessage (joinMessage.playerId));
	}

	public override void onLeave (LeaveMessage joinMessage) {
		Broadcast (new PlayerLeftMessage (joinMessage.playerId));
	}

	private void Broadcast (ISerializable serializable) {
		foreach (IPEndPoint endPoint in clientEndPoints) {
			Debug.Log ("Sending broadcast (broadcast) " + endPoint);
			Send (serializable, endPoint);
		}
	}
}

