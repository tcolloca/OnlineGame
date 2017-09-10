using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ServerController : MonoBehaviour {

	public Text serverPortText;
	public GameObject playerGo;

	private Server server;

	int i = 0;

	public void Start () {
		DontDestroyOnLoad (transform.gameObject);
		PlayerDatabase.Instance.SetPlayerGo (playerGo);
	}
		
	public void Update () {
		if (server != null) {
			server.Receive ();
			if (i % 100 == 0) {
				server.SendSnapshot ();
			}
		}
		i++;
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
