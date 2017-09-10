using System;
using UnityEngine;

public class Position : IBitBufferSerializable {

	public int x { get; private set; }
	public int y { get; private set; }

	public Position (int x, int y) {
		this.x = x;
		this.y = y;
	}

	public Position (BitBuffer buffer) {
		Deserialize (buffer);
	}

	public Position Add (Position pOther) {
		return new Position (x + pOther.x, y + pOther.y);
	}

	public Vector3 toVector3 () {
		return new Vector3 (x, 0, y);
	}

	public byte[] Serialize () {
		BitBuffer buffer = new BitBuffer ();
		Serialize (buffer);
		return buffer.Bytes;
	}

	public void Serialize (BitBuffer buffer) {
		buffer.EnqueueInt (x);
		buffer.EnqueueInt (y);
	}

	public ISerializable Deserialize (byte[] bytes) {
		BitBuffer buffer = new BitBuffer ();
		return Deserialize (buffer);
	}

	public IBitBufferSerializable Deserialize (BitBuffer buffer) {
		x = buffer.DequeueInt ();
		y = buffer.DequeueInt ();
		return this;
	}
}

