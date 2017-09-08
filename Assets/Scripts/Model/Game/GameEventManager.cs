using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager : InputEventListener {

	private static readonly KeyCode[] movKeyCodes = new KeyCode[] { KeyCode.RightArrow, KeyCode.DownArrow, 
		KeyCode.LeftArrow, KeyCode.UpArrow};
	
	private static GameEventManager instance;

	public static GameEventManager Instance {
		get {
			if (instance == null) {
				instance = new GameEventManager ();
			}
			return instance;
		}
	}

	private readonly List<GameEventListener> listeners;

	private GameEventManager () {
		InputEventMulticaster.Instance.AddListener (this);
		listeners = new List<GameEventListener> ();
	}

	public void onInput (InputEvent inputEvent) {
		bool[] rdlu = new bool[4];
		int i = 0;
		foreach (KeyCode keyCode in movKeyCodes) {
			rdlu[i++] = inputEvent.pressedKeys.Contains (keyCode);
		}
		MovementEvent movementEvent = new MovementEvent (inputEvent.player, rdlu);
		foreach (GameEventListener listener in listeners) {
			listener.onMovement (movementEvent);
		}
	}

	public void AddListener (GameEventListener listener) {
		listeners.Add (listener);
	}
}
