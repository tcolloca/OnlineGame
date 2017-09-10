using System;

public interface IBitBufferSerializable : ISerializable {

	void Serialize (BitBuffer bitBuffer);

	IBitBufferSerializable Deserialize (BitBuffer bitBuffer);
}

