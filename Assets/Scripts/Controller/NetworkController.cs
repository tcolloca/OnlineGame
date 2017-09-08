using UnityEngine;
using System.Collections;

public class NetworkController : MonoBehaviour, GameEventListener {

	private int clientPort;
	private int serverPort;
	private NetworkManager networkManager;

	public void Start () {
		DontDestroyOnLoad (transform.gameObject);
	}

	public NetworkController ListenOn (int clientPort) {
		this.clientPort = clientPort;
		return this;
	}

	public NetworkController ConnectTo (int serverPort) {
		this.serverPort = serverPort;
		return this;
	}

	public void Connect () {
		networkManager = new NetworkManager (clientPort, serverPort);
	}

	public void onMovement (MovementEvent movementEvent) {
		networkManager.Send (movementEvent);
	}

	public void OnApplicationQuit () {    
		if (networkManager != null) {
			networkManager.Close ();
		}
	}
}
