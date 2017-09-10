using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FocusSwitcher : MonoBehaviour {

	public Selectable next;
	public bool shouldStartFocused;
	private bool shouldChange;

	void Start () {
		if (shouldStartFocused) {
			this.GetComponent<Selectable> ().Select ();
		}
	}

	// Update is called once per frame
	void Update () {
		
		if (Input.GetKeyDown(KeyCode.Tab) && EventSystem.current.currentSelectedGameObject.Equals(gameObject)) {
			shouldChange = true;
		} else if (shouldChange && Input.GetKeyUp(KeyCode.Tab) && EventSystem.current.currentSelectedGameObject.Equals(gameObject)) {
			shouldChange = false;
			next.Select ();
		}
	}
}
