using System;

public class PlayerLeftMessage : IBitBufferSerializable, Message {

	public int playerId { get; private set; }

	public PlayerLeftMessage (int playerId) {
		this.playerId = playerId;
	}

	public PlayerLeftMessage (BitBuffer buffer) {
		Deserialize (buffer);
	}

	public void Apply (MessageListener listener) {
		listener.onPlayerLeft (this);
	}

	public byte[] Serialize () {
		BitBuffer buffer = new BitBuffer ();
		Serialize (buffer);
		return buffer.Bytes;
	}

	public void Serialize (BitBuffer buffer) {
		buffer.EnqueueInt (playerId);
	}

	public ISerializable Deserialize (byte[] bytes) {
		BitBuffer buffer = new BitBuffer ();
		return Deserialize (buffer);
	}

	public IBitBufferSerializable Deserialize (BitBuffer buffer) {
		playerId = buffer.DequeueInt ();
		return this;
	}
}

