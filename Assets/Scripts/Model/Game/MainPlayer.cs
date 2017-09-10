using System;
using UnityEngine;

public class MainPlayer : Player {

	private PlayerInputRecorder inputRecorder;

	public MainPlayer (int playerId, GameObject playerGo) : base (playerId, playerGo) {
		inputRecorder = new PlayerInputRecorder (this);
	}

	public void RecordInput () {
		inputRecorder.Record ();
	}
}

