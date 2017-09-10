using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClientController : MonoBehaviour {

	public Text clientPortText;
	public Text serverPortText;
	public GameObject playerGo;

	private Client client;
	private MainPlayer player;
	private bool hasJoined;

	public void Start () {
		DontDestroyOnLoad (transform.gameObject);
		PlayerDatabase.Instance.SetPlayerGo (playerGo);
	}

	public void Update () {
		if (client != null) {
			client.Receive ();
			if (!client.hasJoined) {
				Debug.Log ("Sending Join...");
				client.Join ();
			}
		}
		if (player != null) {
			player.RecordInput ();
		}
	}

	public void Play () {
		int clientPort = int.Parse (clientPortText.text);
		int serverPort = int.Parse (serverPortText.text);
		this.player = new MainPlayer (1, playerGo);
		PlayerDatabase.Instance.Init ();
		this.client = new Client (player.id, clientPort).ConnectTo (serverPort);

		SceneManager.LoadScene ("Game");
	}

	public void OnApplicationQuit () {
		if (client != null) {
			client.Close ();
		}
	}
}
