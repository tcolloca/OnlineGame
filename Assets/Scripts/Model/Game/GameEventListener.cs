using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface GameEventListener {

	void onMovement (MovementEvent gameEvent);
}
