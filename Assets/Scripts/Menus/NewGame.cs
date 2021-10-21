using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class NewGame : MonoBehaviour, IPointerClickHandler {

	public InputField seed;

	Text target;

	// Use this for initialization
	void Start () {
		target = gameObject.GetComponent<Text> ();
	}

	// Update is called once per frame
	void Update () {

	}

	public void OnPointerClick(PointerEventData eventData) {
		if (target.text == "Adventure!") {
			if (seed.text != "") {
				Game.current = new Game (int.Parse (seed.text));
			} else {
				Game.current = new Game ();
			}
			SceneManager.LoadScene ("Character Creation");
		} else if (target.text == "Cancel") {
			SceneManager.LoadScene ("Main Menu");
		}
	}
}
