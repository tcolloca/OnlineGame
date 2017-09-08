using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputEvent {

	public List<KeyCode> pressedKeys { get; private set; }
	public Player player { get; private set; }

	public InputEvent (Player player) {
		this.player = player;
		pressedKeys = new List<KeyCode> ();
	}

	public void AddKeyCode (KeyCode keyCode) {
		pressedKeys.Add (keyCode);
	}
}
