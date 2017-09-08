using System;

public class Serializer {

	public byte[] Serialize (ISerializable obj) {
		BitBuffer bitBuffer = new BitBuffer ();
		if (obj is GameEvent) {
			bitBuffer.EnqueueBit (0);
			if (obj is MovementEvent) {
				bitBuffer.EnqueueBits (0, 2);
			}
		}
		bitBuffer.EnqueueBytes (obj.Serialize ());
		return bitBuffer.Bytes;
	}

	public 	ISerializable Deserialize<T> (byte[] bytes) {
		BitBuffer bitBuffer = new BitBuffer (bytes);
		byte eventTypeBit = bitBuffer.DequeueBit ();
		if (eventTypeBit == 0) {
			byte gameEventTypeBits = bitBuffer.DequeueBits (2); 
			if (gameEventTypeBits == 0) {
				return new MovementEvent ().Deserialize (bitBuffer.Bytes);
			}
		}
		throw new Exception ("Unknown object!");
	}
}

