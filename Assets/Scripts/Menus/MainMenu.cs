using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//public class MainMenu : MonoBehaviour, IPointerClickHandler {
public class MainMenu : MonoBehaviour {
	
	//Text target;
	public GameObject marker;
	public Vector2 markerOffset;
	public List<Text> options;

	int index = 0;

	// Use this for initialization
	void Start () {
		//target = gameObject.GetComponent<Text> ();

		//Game.current = new Game ();
		GameInput.Init ();

		NewGame (); //for dev purposes only
	}
	
	// Update is called once per frame
	void Update () {
		if (GameInput.GetKeyDown (GameInput.Bind.up)) {
			index--;
			if (index < 0)
				index = options.Count - 1;
		}
		if (GameInput.GetKeyDown (GameInput.Bind.down)) {
			index++;
			if (index >= options.Count)
				index = 0;
		}
		if (GameInput.GetKeyDown (GameInput.Bind.interact)) {
			if (index == 0) { //new game
				NewGame ();
			} else if (index == 1) { //load game

			} else if (index == 2) { //quit
				Application.Quit ();
			}
		}
		SetMarker ();
	}

	void SetMarker () {
		marker.GetComponent<RectTransform> ().position = options [index].gameObject.GetComponent<RectTransform> ().position + (Vector3)markerOffset;
	}

	void NewGame () {
		Game.CreateDeveloperGame ();
		for (int i = 0; i < Game.current.characters.Count; i++) {
			DontDestroyOnLoad (Game.current.characters [i]);
		}
		SceneManager.LoadScene ("Game");
	}

	/*
	public void OnPointerClick(PointerEventData eventData) {
		if (target.text == "New Game") {
			SceneManager.LoadScene ("New Game");
		} else if (target.text == "Load Game") {
			SceneManager.LoadScene ("Load Game");
		} else if (target.text == "Quit") {
			Application.Quit ();
		}
	}
	*/
}
