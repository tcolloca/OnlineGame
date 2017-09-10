using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDatabase : MessageAdapter {

	private static PlayerDatabase instance;

	public static PlayerDatabase Instance {
		get {
			if (instance == null) {
				instance = new PlayerDatabase ();
			}
			return instance;
		}
	}

	private Dictionary<int, Player> players;
	private GameObject playerGo;

	private PlayerDatabase () {
	}

	public void Init () {
		players = new Dictionary<int, Player> ();
		MessageMulticaster.Instance.AddListener (this);
	}

	public ICollection<Player> Players () {
		return players.Values;
	}

	public void SetPlayerGo(GameObject playerGo) {
		this.playerGo = playerGo;
	}

	public void AddPlayer (int id) {
		AddPlayer (new Player(id, playerGo));
	}

	public void AddPlayer (Player player) {
		players.Add (player.id, player);
	}

	public void RemovePlayer (int id) {
		players.Remove (id);
	}

	public Player GetPlayer (int id) {
		Player player;
		players.TryGetValue (id, out player);
		return player;
	}

	public override void onJoin (JoinMessage message) {
		Debug.Log ("on join (PlayerDB)");
		AddPlayer (message.playerId);
	}

	public override void onLeave (LeaveMessage message) {
		RemovePlayer (message.playerId);
	}

	public override void onPlayerJoined (PlayerJoinedMessage message) {
		AddPlayer (message.playerId);
	}

	public override void onPlayerLeft (PlayerLeftMessage message) {
		RemovePlayer (message.playerId);
	}
}

