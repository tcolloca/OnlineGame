using System;
using UnityEngine;

public class GameResolver : MessageAdapter {

	private static readonly int HALF_SIZE = 6;

	private int[,] board;

	public GameResolver () {
		MessageMulticaster.Instance.AddListener (this);
		int size = HALF_SIZE * 2;
		board = new int[size, size];
		for (int i = 0; i < size; i++) {
			for (int j = 0; j < size; j++) {
				if (i == 0 || j == 0 || i == size - 1 || j == size - 1) {
					board [i, j] = -1;
				}
			}
		}
	}

	public override void onPlayerJoined (PlayerJoinedMessage message) {
		Player player = PlayerDatabase.Instance.GetPlayer (message.playerId);
		player.SetPosition (new Position (0, 0));
		setId (new Position(0, 0), player.id);
	}

	public override void onMovement (MovementMessage message) {
		Debug.Log ("ON MOVE");
		foreach (bool b in message.rdlu) {
			Debug.Log (b);
		}
		Player player = PlayerDatabase.Instance.GetPlayer (message.playerId);
		int x = (message.rdlu [0] ? 1 : 0) - (message.rdlu [2] ? 1 : 0);
		int y = (message.rdlu [3] ? 1 : 0)  - (message.rdlu [1] ? 1 : 0);
		Position newPos = player.position.Add (new Position (x, y));
		Debug.Log ("New pos: " + newPos.x + ", " + newPos.y);
		if (getId (newPos) == 0) {
			setId (newPos, player.id);
			setId (player.position, 0);
			player.SetPosition (newPos);
		} else if (getId (newPos) < 0) {
			Debug.Log ("Wrong move!!");
		}
	}

	private int getId (Position pos) {
		return board [pos.x + HALF_SIZE, pos.y + HALF_SIZE];
	}

	private void setId (Position pos, int playerId) {
		board [pos.x + HALF_SIZE, pos.y + HALF_SIZE] = playerId;
	}
}

