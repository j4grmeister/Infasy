using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsTab : MenuTab {
	public Image character;
	public Text level;
	public Text exp;
	public Text hp;
	public Text ap;
	public GameObject marker;
	public Vector2 markerOffset;
	public GameObject optionContainer;
	//must be the same size and must also have the elements in the same order
	public List<MenuTab> subMenus;
	public List<Text> options;

	int index = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (optionContainer.activeSelf) {
			if (GameInput.GetKeyDown (GameInput.Bind.up)) {
				index--;
				if (index < 0) {
					index = options.Count - 1;
				}
			}
			if (GameInput.GetKeyDown (GameInput.Bind.down)) {
				index++;
				if (index >= options.Count) {
					index = 0;
				}
			}
			if (GameInput.GetKeyDown (GameInput.Bind.interact)) {
				subMenus [index].UpdateTab ();
				subMenus [index].gameObject.SetActive (true);
				gameObject.SetActive (false);
			}

			MoveMarker ();
		}
	}

	void MoveMarker () {
		marker.GetComponent<RectTransform> ().localPosition = options [index].gameObject.GetComponent<RectTransform> ().localPosition + (Vector3)markerOffset;
	}

	public override void UpdateTab () {
		index = 0;

		character.sprite = Character.GetCharacterSprite (Game.current.playerCharacter.race);
		level.text = "Lv. " + Game.current.playerCharacter.level.ToString ();
		exp.text = "Exp. " + Game.current.playerCharacter.exp.ToString () + "\tNext Level " + Character.ExpForLevel (Game.current.playerCharacter.level + 1).ToString ();
		hp.text = "HP " + Game.current.playerCharacter.hp.ToString () + "/" + Game.current.playerCharacter.maxhp.ToString ();
		ap.text = "AP " + Game.current.playerCharacter.ap.ToString ();
	}

	public void ShowFull () {
		ShowStats ();
		ShowOptions ();
	}

	public void ShowStats () {
		gameObject.SetActive (true);
	}

	public void ShowOptions () {
		gameObject.SetActive (true);
		optionContainer.SetActive (true);
	}

	public void HideOptions () {
		optionContainer.SetActive (false);
	}
}
