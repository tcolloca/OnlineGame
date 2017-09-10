using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ServerController : MonoBehaviour {

	public Text serverPortText;
	public GameObject playerGo;

	private Server server;

	public void Start () {
		DontDestroyOnLoad (transform.gameObject);
		PlayerDatabase.Instance.SetPlayerGo (playerGo);
	}
		
	public void Update () {
		if (server != null) {
			server.Receive ();
			server.SendSnapshot ();
		}
	}

	public void Open () {
		int serverPort = int.Parse (serverPortText.text);
		this.server = new Server (serverPort);
		PlayerDatabase.Instance.Init ();

		SceneManager.LoadScene ("Game");
	}

	public void OnApplicationQuit () {
		if (server != null) {
			server.Close ();
		}
	}
}
