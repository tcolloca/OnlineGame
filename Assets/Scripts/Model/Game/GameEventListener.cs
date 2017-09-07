using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface GameEventListener {

	void onGameEvent<T> (GameEvent<T> gameEvent);
}
