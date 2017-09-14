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

	int i = 0;

	public void Start () {
		DontDestroyOnLoad (transform.gameObject);
		PlayerDatabase.Instance.SetPlayerGo (playerGo);
	}

	public void Update () {
		if (client != null) {
			client.Receive ();
			if (i % 10 == 0) {
				if (!client.hasJoined) {
					Debug.Log ("Sending Join...");
					client.Join ();
				}
				client.RecordInput ();
			}
		}
		i++;
	}

	public void Play () {
		int clientPort = int.Parse (clientPortText.text);
		int serverPort = int.Parse (serverPortText.text);
		PlayerDatabase.Instance.Init ();
		this.client = new Client (clientPort, 1).ConnectTo (serverPort);

		SceneManager.LoadScene ("Game");
	}

	public void OnApplicationQuit () {
		if (client != null) {
			client.Close ();
		}
	}
}
