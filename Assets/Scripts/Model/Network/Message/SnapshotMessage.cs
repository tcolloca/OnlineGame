using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SnapshotMessage : IBitBufferSerializable, Message {

	public Dictionary<int, Position> playerPositions { get; private set; }

	public SnapshotMessage (Dictionary<int, Position> playerPositions) {
		this.playerPositions = playerPositions;
	}

	public SnapshotMessage (BitBuffer buffer) {
		Deserialize (buffer);
	}

	public void Apply (MessageListener listener) {
		listener.onSnapshot (this);
	}

	public byte[] Serialize () {
		BitBuffer buffer = new BitBuffer ();
		Serialize (buffer);
		return buffer.Bytes;
	}

	public void Serialize (BitBuffer buffer) {
		buffer.EnqueueInt (playerPositions.Count);
		foreach (KeyValuePair<int, Position> entry in playerPositions) {
			buffer.EnqueueInt (entry.Key);
			buffer.EnqueueSerializable (entry.Value);
		}
	}

	public ISerializable Deserialize (byte[] bytes) {
		BitBuffer buffer = new BitBuffer (bytes);
		return Deserialize (buffer);
	}

	public IBitBufferSerializable Deserialize (BitBuffer buffer) {
		playerPositions = new Dictionary<int, Position> ();
		int count = buffer.DequeueInt ();
		for (int i = 0; i < count; i++) {
			int playerId = buffer.DequeueInt ();
			Position position = new Position (buffer);
			playerPositions.Add (playerId, position);
		}
		return this;
	}
}
