using UnityEngine;
using System.Collections;

public class Snapshot : ISerializable {

	public byte[] Serialize () {
		BitBuffer bitBuffer = new BitBuffer ();
		return bitBuffer.Bytes;
	}

	public ISerializable Deserialize (byte[] bytes) {
		BitBuffer bitBuffer = new BitBuffer (bytes);
		return this;
	}
}
