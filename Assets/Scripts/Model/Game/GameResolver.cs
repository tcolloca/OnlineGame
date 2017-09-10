using System;

public class GameResolver : MessageAdapter {

	private static readonly int SIZE = 12;

	private int[,] board;

	public GameResolver () {
		board = new int[SIZE, SIZE];
		for (int i = 0; i < SIZE; i++) {
			for (int j = 0; j < SIZE; i++) {
				if (i == 0 || j == 0 || i == SIZE - 1 || j == SIZE - 1) {
					board [i, j] = -1;
				}
			}
		}
	}

	public override void onMovement (MovementMessage message) {
		Player player = PlayerDatabase.Instance.GetPlayer (message.playerId);
		int x = (message.rdlu [0] ? 1 : 0) - (message.rdlu [2] ? 1 : 0);
		int y = (message.rdlu [3] ? 1 : 0)  - (message.rdlu [1] ? 1 : 0);
		Position newPos = player.position.Add (new Position (x, y));
		if (board [newPos.x, newPos.y] == 0) {
			board [newPos.x, newPos.y] = player.id;
			board [player.position.x, player.position.y] = 0;
			player.SetPosition (newPos);
		}
	}
}

