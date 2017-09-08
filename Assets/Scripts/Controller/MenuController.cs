using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

	public GameObject networkController;
	public Text clientPortText;
	public Text serverPortText;
	
	public void Play() {
		Debug.Log ("Play");

		networkController.GetComponent<NetworkController> ()
			.ListenOn (int.Parse (clientPortText.text))
			.ConnectTo (int.Parse (serverPortText.text))
			.Connect ();

		SceneManager.LoadScene ("game");
	}
}
