using System;
using System.Collections.Generic;
using UnityEngine;

public class MessageMulticaster {

	private static MessageMulticaster instance;

	public static MessageMulticaster Instance {
		get {
			if (instance == null) {
				instance = new MessageMulticaster ();
			}
			return instance;
		}
	}

	private readonly List<MessageListener> listeners;

	private MessageMulticaster () {
		listeners = new List<MessageListener> ();
	}

	public void onReceived (Message obj) {
		List<MessageListener> aux = new List<MessageListener> (listeners);
		foreach (MessageListener listener in aux) {
			try {
				obj.Apply (listener);
			} catch (Exception e) {
				Debug.Log (e);
			}
		}
	}

	public void AddListener (MessageListener listener) {
		listeners.Add (listener);
	}
}

