using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputEventMulticaster {

	private static InputEventMulticaster instance;

	public static InputEventMulticaster Instance {
		get {
			if (instance == null) {
				instance = new InputEventMulticaster ();
			}
			return instance;
		}
	}

	private readonly List<InputEventListener> listeners;

	private InputEventMulticaster () {
		listeners = new List<InputEventListener> ();
	}

	public void Multicast (InputEvent inputEvent) {
		foreach (InputEventListener listener in listeners) {
			listener.onInput (inputEvent);
		}
	}

	public void AddListener (InputEventListener listener) {
		listeners.Add (listener);
	}
}
