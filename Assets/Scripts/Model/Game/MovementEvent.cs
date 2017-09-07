using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementEvent : GameEvent<MovementEvent> {

	private bool[] rdlu;

	public MovementEvent (bool[] rdlu) {
		this.rdlu = rdlu;
	}

	public byte[] Serialize () {
		BitBuffer buffer = new BitBuffer ();
		foreach (bool mov in rdlu) {
			buffer.EnqueueBool (mov);
		}
		return buffer.Bytes;
	}

	public MovementEvent Deserialize (byte[] bytes) {
		BitBuffer buffer = new BitBuffer (bytes);
		bool[] rdlu = new bool[4];
		for (int i = 0; i < 4; i++) {
			rdlu [i] = buffer.DequeueBool ();
		}
		return new MovementEvent (rdlu);
	}
}
