using System;
using UnityEngine;

public class MainPlayer {

	private Player player;
	private PlayerInputRecorder inputRecorder;

	public MainPlayer (Player player) {
		this.player = player;
		this.inputRecorder = new PlayerInputRecorder (player);
	}

	public void RecordInput () {
		Debug.Log ("Record");
		inputRecorder.Record ();
	}
}

