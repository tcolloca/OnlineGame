using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementMessage : GameEvent {

	public int playerId { get; private set; }
	public bool[] rdlu { get; private set; }

	public MovementMessage (int playerId, bool[] rdlu) {
		this.playerId = playerId;
		this.rdlu = rdlu;
	}

	public MovementMessage (BitBuffer bitBuffer) {
		Deserialize (bitBuffer);
	}

	public void Apply (MessageListener listener) {
		listener.onMovement (this);
	}

	public byte[] Serialize () {
		BitBuffer buffer = new BitBuffer ();
		Serialize (buffer);
		return buffer.Bytes;
	}

	public void Serialize (BitBuffer buffer) {
		buffer.EnqueueInt (playerId);
		foreach (bool mov in rdlu) {
			buffer.EnqueueBool (mov);
		}
	}

	public ISerializable Deserialize (byte[] bytes) {
		BitBuffer buffer = new BitBuffer (bytes);
		return Deserialize (buffer);
	}

	public IBitBufferSerializable Deserialize (BitBuffer buffer) {
		playerId = buffer.DequeueInt ();
		rdlu = new bool[4];
		for (int i = 0; i < 4; i++) {
			rdlu [i] = buffer.DequeueBool ();
		}
		return this;
	}
}
