using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChooserController : MonoBehaviour {

	void Start () {
		if (ConfigProperties.Instance.isServer) {
			SceneManager.LoadScene ("ServerMain");
		} else {
			SceneManager.LoadScene ("ClientMain");
		}
	}
}
