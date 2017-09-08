using System;
using System.Collections.Generic;

public class PlayerDatabase {

	Dictionary<int, Player> players = new Dictionary<int, Player> ();

	public void addPlayer (Player player) {
		players.Add (player.id, player);
	}

	public Player getPlayer (int id) {
		Player player;
		players.TryGetValue (id, out player);
		return player;
	}
}

