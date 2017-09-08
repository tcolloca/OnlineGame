using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementEvent : GameEvent {

	private Player player;
	private bool[] rdlu;

	public MovementEvent (Player player, bool[] rdlu) {
		this.player = player;
		this.rdlu = rdlu;
	}

	public MovementEvent () {
	}

	public byte[] Serialize () {
		BitBuffer buffer = new BitBuffer ();
		buffer.EnqueueBits ((byte) player.id, 3);
		foreach (bool mov in rdlu) {
			buffer.EnqueueBool (mov);
		}
		return buffer.Bytes;
	}

	public ISerializable Deserialize (byte[] bytes) {
		BitBuffer buffer = new BitBuffer (bytes);
		int playerId = buffer.DequeueBits (3);
		rdlu = new bool[4];
		for (int i = 0; i < 4; i++) {
			rdlu [i] = buffer.DequeueBool ();
		}
		return this;
	}
}
