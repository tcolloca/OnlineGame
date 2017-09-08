using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputRecorder {

	private static readonly KeyCode[] interestKeyCodes = new KeyCode[] { KeyCode.RightArrow, KeyCode.DownArrow, 
		KeyCode.LeftArrow, KeyCode.UpArrow};

	private Dictionary<KeyCode, bool> keyCodes;
	private Player player;

	public PlayerInputRecorder (Player player) {
		this.player = player;
		keyCodes = new Dictionary<KeyCode, bool> ();
		foreach (KeyCode keyCode in interestKeyCodes) {
			keyCodes.Add (keyCode, false);
		}
	}

	public void Record () {
		InputEvent inputEvent = new InputEvent (player);
		foreach (KeyCode keyCode in interestKeyCodes) {
			bool wasPressed;
			keyCodes.TryGetValue (keyCode, out wasPressed);
			bool isPressed = Input.GetKey (keyCode);
			if (wasPressed != isPressed) {
				keyCodes.Add (keyCode, isPressed);
			}
			if (!wasPressed && isPressed) {
				inputEvent.AddKeyCode (keyCode);
			}
		}
		InputEventMulticaster.Instance.Multicast (inputEvent);
	}
}
