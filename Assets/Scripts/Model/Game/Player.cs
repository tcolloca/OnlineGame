using UnityEngine;
using System.Collections;

public class Player : MessageAdapter {

	public int id { get; private set; }
	public Position position;
	private GameObject playerGo;

	public Player (int id, GameObject playerGo) {
		this.id = id;
		this.playerGo = Object.Instantiate (playerGo);
		this.playerGo.SetActive (true);
		this.position = new Position (0, 0);
		MessageMulticaster.Instance.AddListener (this);
	}

	public void SetPosition (Position position) {
		Debug.Log (position.x + " " + position.y);
		this.position = position;
	}

	public override void onSnapshot (SnapshotMessage snapshot) {
		SetPosition (snapshot.playerPositions [id]);
		playerGo.transform.position = position.toVector3 ();
	}
}
