using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMessageMulticaster : InputEventListener {

	private static readonly KeyCode[] movKeyCodes = new KeyCode[] { KeyCode.RightArrow, KeyCode.DownArrow, 
		KeyCode.LeftArrow, KeyCode.UpArrow};
	
	private static GameMessageMulticaster instance;

	public static GameMessageMulticaster Instance {
		get {
			if (instance == null) {
				instance = new GameMessageMulticaster ();
			}
			return instance;
		}
	}

	private readonly List<GameMessageListener> listeners;

	private GameMessageMulticaster () {
		InputEventMulticaster.Instance.AddListener (this);
		listeners = new List<GameMessageListener> ();
	}

	public void onInput (InputEvent inputEvent) {
		bool[] rdlu = new bool[4];
		int i = 0;
		foreach (KeyCode keyCode in movKeyCodes) {
			rdlu[i++] = inputEvent.pressedKeys.Contains (keyCode);
		}
		MovementMessage message = new MovementMessage (inputEvent.player.id, rdlu);
		foreach (GameMessageListener listener in listeners) {
			listener.onMovement (message);
		}
	}

	public void AddListener (GameMessageListener listener) {
		listeners.Add (listener);
	}
}
