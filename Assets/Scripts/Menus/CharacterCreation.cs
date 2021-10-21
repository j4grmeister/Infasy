using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterCreation : MonoBehaviour, IPointerClickHandler {

	public InputField nameInput;
	public Dropdown raceInput;
	public Dropdown classInput;

	public Text strengthScore;
	public Text dexterityScore;
	public Text constitutionScore;
	public Text intelligenceScore;
	public Text wisdomScore;
	public Text charismaScore;

	public GameObject previousTab;
	public GameObject nextTab;

	Text target;

	// Use this for initialization
	void Start () {
		target = gameObject.GetComponent<Text> ();

		if(target.text == "Reroll") {
			RollAbilityScores ();
		}

		//Game.current = new Game (); //for dev purposes only
	}

	// Update is called once per frame
	void Update () {

	}

	public void OnPointerClick(PointerEventData eventData) {
		if (target.text == "Continue") {
			gameObject.transform.parent.gameObject.SetActive (false);
			nextTab.SetActive (true);
		} else if(target.text == "Reroll") {
			RollAbilityScores ();
		} else if (target.text == "Adventure!") {
			//Game.current.playerCharacter.characterName = nameInput.text;
			Game.current.playerCharacter.name = nameInput.text;
			/*
			Game.current.playerCharacter.tRace = (Race.Type)raceInput.value;
			Game.current.playerCharacter.tClass = (Class.Type)classInput.value;
			Game.current.playerCharacter.ability.Set ("strength", int.Parse(strengthScore.text));
			Game.current.playerCharacter.ability.Set ("dexterity", int.Parse(dexterityScore.text));
			Game.current.playerCharacter.ability.Set ("constitution", int.Parse(constitutionScore.text));
			Game.current.playerCharacter.ability.Set ("intelligence", int.Parse(intelligenceScore.text));
			Game.current.playerCharacter.ability.Set ("wisdom", int.Parse(wisdomScore.text));
			Game.current.playerCharacter.ability.Set ("charisma", int.Parse(charismaScore.text));
			*/
			SceneManager.LoadScene ("Game");
		} else if (target.text == "Back") {
			if (previousTab != null) {
				gameObject.transform.parent.gameObject.SetActive (false);
				previousTab.SetActive (true);
			} else {
				SceneManager.LoadScene ("New Game");
			}
		}
	}

	public void RollAbilityScores() {
		strengthScore.text = GetAbilityScore ().ToString ();
		dexterityScore.text = GetAbilityScore ().ToString ();
		constitutionScore.text = GetAbilityScore ().ToString ();
		intelligenceScore.text = GetAbilityScore ().ToString ();
		wisdomScore.text = GetAbilityScore ().ToString ();
		charismaScore.text = GetAbilityScore ().ToString ();
	}

	int GetAbilityScore() {
		int sum = 0;
		int low = 0;
		for (int i = 0; i < 4; i++) {
			int n = Random.Range (1, 6);
			low = (low == 0) ? n : Mathf.Min (low, n);
			sum += n;
		}
		sum -= low;
		return sum;
	}
}