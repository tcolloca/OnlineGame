using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputEvent {

	public List<KeyCode> pressedKeys { get; private set; }

	public InputEvent () {
		pressedKeys = new List<KeyCode> ();
	}

	public void AddKeyCode (KeyCode keyCode) {
		pressedKeys.Add (keyCode);
	}
}
