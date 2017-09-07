using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Serializable<T> {

	byte[] Serialize ();

	T Deserialize (byte[] bytes);
}
