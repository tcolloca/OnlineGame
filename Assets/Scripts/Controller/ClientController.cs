﻿using UnityEngine;
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
			if (!client.hasJoined && i % 100 == 0) {
				Debug.Log ("Sending Join...");
				client.Join ();
			}
		}
		if (player != null) {
			player.RecordInput ();
		}
		i++;
	}

	public void Play () {
		int clientPort = int.Parse (clientPortText.text);
		int serverPort = int.Parse (serverPortText.text);
		this.player = new MainPlayer (1, playerGo);
		PlayerDatabase.Instance.Init ();
		this.client = new Client (clientPort, player.id).ConnectTo (serverPort);

		SceneManager.LoadScene ("Game");
	}

	public void OnApplicationQuit () {
		if (client != null) {
			client.Close ();
		}
	}
}
