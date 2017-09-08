using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISerializable {

	byte[] Serialize ();

	ISerializable Deserialize (byte[] bytes);
}
