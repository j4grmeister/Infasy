using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SystemTab : MenuTab {
	public GameObject optionMarker;
	public GameObject saveMarker;
	public Vector2 markerOffset;
	public List<Text> options;
	public GameObject optionContainer;
	public SaveEntry saveEntry;
	public GameObject saveContainer;
	public Vector2 saveOptionOffset;
	public float saveOptionSpacing;
	public SaveInfo saveInfo;

	List<int> index = new List<int> ();
	List<SaveEntry> saveOptions = new List<SaveEntry> ();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (GameInput.GetKeyDown (GameInput.Bind.up)) {
			index [index.Count - 1]--;
			if (index [index.Count - 1] < 0) {
				if (index.Count == 1) {
					index [index.Count - 1] = options.Count - 1;
				} else if (index.Count >= 2) {
					index [index.Count - 1] = saveOptions.Count - 1;
				}
			}
		}
		if (GameInput.GetKeyDown (GameInput.Bind.down)) {
			index [index.Count - 1]++;
			if (index.Count == 1) {
				if (index [index.Count - 1] >= options.Count) {
					index [index.Count - 1] = 0;
				}
			} else if (index.Count >= 2) {
				if (index [index.Count - 1] >= saveOptions.Count) {
					index [index.Count - 1] = 0;
				}
			}
		}
		if (GameInput.GetKeyDown (GameInput.Bind.interact)) {
			if (index.Count == 1) {
				if (index [0] == 0) { //save
					index.Add (0);
					UpdateOptions ();
				} else if (index [0] == 1) { //load
					index.Add (0);
					UpdateOptions ();
				} else if (index [0] == 2) { //quit
					SceneManager.LoadScene ("Main Menu");
				}
			} else if (index.Count == 2) {
				if (index [0] == 0) { //save
					if (index [1] == 0) {
						Game.Save ();
					} else {
						Game.Save (Game.currentGameSave.Count - index [1]);
					}
					Game.SaveGameSaves ();
				} else if (index [0] == 1) { //load
					Game.Load (Game.savedGames [Game.GetSaveIndex (Game.current.playerCharacter.name)].Count - 1 - index [1]);
					for (int i = 0; i < Game.current.characters.Count; i++) {
						DontDestroyOnLoad (Game.current.characters [i]);
					}
					SceneManager.LoadScene ("Game");
					//since GameTime is referenced as a static object, it is separate from the
					//game data itself and so is not reset at the loading of a new scene,
					//therefore we must unpause the game ourselves
					//(now we simply call a static method which re-initializes all essential static objects
					Game.Start ();
				}
				index.RemoveAt (1);
				UpdateOptions ();
			}
		}
		if (GameInput.GetKeyDown (GameInput.Bind.back)) {
			if (index.Count == 1) {
				Game.ui.menu.GoHome ();
			} else if (index.Count == 2) {
				index.RemoveAt (1);
				UpdateOptions ();
			} else if (index.Count == 3) {
				index.RemoveAt (2);
				UpdateOptions ();
			}
		}
		MoveMarker ();
	}

	void MoveMarker () {
		if (index.Count == 1) {
			optionMarker.GetComponent<RectTransform> ().localPosition = options [index [0]].gameObject.GetComponent<RectTransform> ().localPosition + (Vector3)markerOffset;
		} else if (index.Count == 2) {
			saveMarker.GetComponent<RectTransform> ().localPosition = saveOptions [index [index.Count - 1]].gameObject.GetComponent<RectTransform> ().localPosition + (Vector3)markerOffset;
			if (index [0] == 0) { //save
				if (index [1] == 0) {
					saveInfo.SetSaveIndex (-1);
				} else {
					saveInfo.SetSaveIndex (Game.currentGameSave.Count - index [index.Count - 1]);
				}
			} else if (index [0] == 1) { //load
				saveInfo.SetSaveIndex (Game.currentGameSave.Count - index [index.Count - 1] - 1);
			}
		}
	}

	public void UpdateOptions () {
		if (index.Count == 1) {
			optionContainer.SetActive (true);
			saveContainer.SetActive (false);
			saveInfo.gameObject.SetActive (false);
			Game.ui.menu.home.ShowStats ();
			Game.ui.menu.home.HideOptions ();
		} else if (index.Count == 2) {
			Game.UpdateGameInfo ();
			Game.ui.menu.home.gameObject.SetActive (false);
			//optionContainer.SetActive (false);
			saveContainer.SetActive (true);
			saveInfo.gameObject.SetActive (true);
			if (index [0] == 0) { //save
				Game.LoadGameSaves ();
				if (saveOptions.Count == 0) {
					SaveEntry newSave = (SaveEntry)GameObject.Instantiate (saveEntry, saveContainer.transform);
					newSave.gameObject.GetComponent<RectTransform> ().localPosition = (Vector3)saveOptionOffset + Vector3.down;
					saveOptions.Add (newSave);
				}
				saveOptions [0].SetSave (Game.currentGameSave, -1);
				for (int i = 0; i <= Game.savedGames [Game.GetSaveIndex (Game.current.playerCharacter.name)].Count; i++) {
					if (i + 1 >= saveOptions.Count) {
						SaveEntry e = (SaveEntry)GameObject.Instantiate (saveEntry, saveContainer.transform);
						e.gameObject.GetComponent<RectTransform> ().localPosition = (Vector3)saveOptionOffset + Vector3.down * saveOptionSpacing * (i + 1);
						saveOptions.Add (e);
					}
					saveOptions [i + 1].SetSave (Game.savedGames [Game.GetSaveIndex (Game.current.playerCharacter.name)], Game.savedGames [Game.GetSaveIndex (Game.current.playerCharacter.name)].Count - 1 - i);
				}
				for (int i = Game.savedGames [Game.GetSaveIndex (Game.current.playerCharacter.name)].Count + 1; i < saveOptions.Count;) {
					GameObject.Destroy (saveOptions [i].gameObject);
					saveOptions.RemoveAt (saveOptions.Count - 1);
				}
			} else if (index [0] == 1) { //load
				Game.LoadGameSaves ();
				/*
				if (saveOptions.Count <= 0) {
					SaveEntry e = (SaveEntry)GameObject.Instantiate (saveEntry, saveContainer.transform);
					e.gameObject.GetComponent<RectTransform> ().localPosition = (Vector3)saveOptionOffset + Vector3.down * saveOptionSpacing * i;
					saveOptions.Add (e);
				}
				saveOptions [0].SetSave (Game.savedGames [index [1]], -2);
				*/

				for (int i = 0; i <= Game.savedGames [Game.GetSaveIndex (Game.current.playerCharacter.name)].Count; i++) {
					if (i >= saveOptions.Count) {
						SaveEntry e = (SaveEntry)GameObject.Instantiate (saveEntry, saveContainer.transform);
						e.gameObject.GetComponent<RectTransform> ().localPosition = (Vector3)saveOptionOffset + Vector3.down * saveOptionSpacing * i;
						saveOptions.Add (e);
					}
					saveOptions [i].SetSave (Game.savedGames [Game.GetSaveIndex (Game.current.playerCharacter.name)], Game.savedGames [Game.GetSaveIndex (Game.current.playerCharacter.name)].Count - 1 - i);
				}
				for (int i = Game.savedGames [Game.GetSaveIndex (Game.current.playerCharacter.name)].Count; i < saveOptions.Count;) {
					GameObject.Destroy (saveOptions [i].gameObject);
					saveOptions.RemoveAt (saveOptions.Count - 1);
				}
			}
		}
		MoveMarker ();
	}
	
	public override void UpdateTab () {
		index.RemoveRange (0, index.Count);
		index.Add (0);
		UpdateOptions ();
	}
}
