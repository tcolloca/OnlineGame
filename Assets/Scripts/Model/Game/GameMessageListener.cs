using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface GameMessageListener {

	void onMovement (MovementMessage message);
}
