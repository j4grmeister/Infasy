using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionMenu : MonoBehaviour {

	//public GameObject actionOption;
	public GameObject optionContainer;
	public GameObject marker;
	//public ActionInfo actionInfo;
	//public Vector3 actionOffset;
	public Vector3 markerOptionOffset;
	public Vector3 markerTargetOffset;

	public Slider hpSlider;
	public Text hpText;
	public Slider fpSlider;
	public Text fpText;

	//public List<Text> text1;
	public List<List<string>> option1 = new List<List<string>> ();
	List<List<Text>> text1 = new List<List<Text>> ();
	List<int> index1 = new List<int> ();
	//public Sprite markerImage;
	List<GameObject> marker1 = new List<GameObject> ();
	public Text optionObject;
	//public Vector2 optionDistance;
	//public Vector2 optionOffset;
	public int optionsPerColumn;
	List<int> firstOptionIndex = new List<int> ();
	//public Text turnDisplay;
	public APUI ap;

	bool chooseTarget = false;
	bool updateThisFrame = true;
	bool isPlayerTurn = true;
	//List<ActionOption> options = new List<ActionOption> ();
	//int index = 0;

	Battle.Turn playerTurn;

	// Use this for initialization
	void Start () {
		//hide the object initially
		//gameObject.SetActive (false);
		index1.Add (0);
		GameObject mi = GameObject.Instantiate (marker, optionContainer.transform);
		marker1.Add (mi);

		List<string> home = new List<string> ();
		home.Add ("Attack");
		home.Add ("Skills");
		home.Add ("Items");
		option1.Add (home);
		firstOptionIndex.Add (0);
		LoadOptions ();

		ResetTurn ();
	}
		
	// Update is called once per frame
	void Update () {
		if (updateThisFrame && isPlayerTurn) {
			if (GameInput.GetKeyDown (GameInput.Bind.up)) {
				//index--;
				index1 [index1.Count - 1]--;
				//if (index < 0)
				if (index1 [index1.Count - 1] < 0)
					index1 [index1.Count - 1] = option1 [option1.Count - 1].Count - 1;
				if (index1 [index1.Count - 1] - firstOptionIndex [firstOptionIndex.Count - 1] < 0) {
					firstOptionIndex [firstOptionIndex.Count - 1]--;
				}
			}
			if (GameInput.GetKeyDown (GameInput.Bind.down)) {
				//index++;
				index1 [index1.Count - 1]++;
				//if (index >= options.Count)
				if (index1 [index1.Count - 1] >= option1 [option1.Count - 1].Count)
					index1 [index1.Count - 1] = 0;
				if (index1 [index1.Count - 1] - firstOptionIndex [firstOptionIndex.Count - 1] >= optionsPerColumn) {
					firstOptionIndex [firstOptionIndex.Count - 1]++;
				}
			}
			if (GameInput.GetKeyDown (GameInput.Bind.interact)) {
				if (index1.Count == 1) {
					if (index1 [0] == 0) { //Attack
						AddAction (Game.current.playerCharacter.inventory.equippedWeapon.action);
					} else if (index1 [0] == 1) { //Skills
						GameObject mi = GameObject.Instantiate (marker, optionContainer.transform);
						marker1.Add (mi);
						List<string> newColumn = new List<string> ();
						for (int i = 0; i < Game.current.playerCharacter.skills.Count; i++) {
							Action skill = Skill.skills.Get (Game.current.playerCharacter.skills [i]);
							newColumn.Add (skill.name + " : " + skill.ap.ToString () + "/" + Game.current.playerCharacter.fp.ToString ());
						}
						option1.Add (newColumn);
						index1.Add (0);
					} else if (index1 [0] == 2) { //Items

					}
				} else if (index1.Count == 2) {
					if (index1 [0] == 1) { //skills
						AddAction (Skill.skills.Get (Game.current.playerCharacter.skills [index1 [index1.Count - 1]]));
					}
				}

				//AddAction (options [index].Action ());
				//Hide ();
			}
			if (GameInput.GetKeyDown (GameInput.Bind.back)) {
				if (index1.Count > 1) {
					Back ();
				}
			}
			/*
			if (Input.GetKeyDown (KeyCode.Q)) {
				GameTime.paused = false;
				Deactivate ();
			}
			*/
			//perform the player's turn
			if (GameInput.GetKeyDown (GameInput.Bind.end_turn)) {
				Game.current.playerCharacter.GetBattle ().SubmitTurn (playerTurn);
				System.Action endTurn = () => {
					Game.current.playerCharacter.EndTurn ();
				};
				ActionSchedule.ScheduleAction (endTurn, Game.camera.transitionTime);
				Game.camera.SetTarget (Game.current.playerCharacter);
				ResetTurn ();
				Deactivate ();
			}

			/*
			if (optionContainer.activeSelf) {
				marker.GetComponent<RectTransform> ().position = options [index].gameObject.GetComponent<RectTransform> ().position + markerOptionOffset;
				actionInfo.SetAction (options [index].Action ());
			}
			*/

			marker1 [marker1.Count - 1].GetComponent<RectTransform> ().localPosition = text1 [text1.Count - 1] [index1 [index1.Count - 1]].GetComponent<RectTransform> ().localPosition + markerOptionOffset;
			//Debug.Log (index1 [index1.Count - 1]);
			LoadOptions ();
		}

		if (updateThisFrame == chooseTarget) {
			updateThisFrame = !chooseTarget;
		}
	}

	void Back () {
		Game.camera.SetTarget (Game.current.playerCharacter);
		index1.RemoveAt (index1.Count - 1);
		option1.RemoveAt (option1.Count - 1);
		GameObject.Destroy (marker1 [marker1.Count - 1]);
		marker1.RemoveAt (marker1.Count - 1);

		//Game.DestroyComponentList (text1 [text1.Count - 1]);
		for (int i = 0; i < text1 [text1.Count - 1].Count;) {
			GameObject.Destroy (text1 [text1.Count - 1] [i].gameObject);
			text1 [text1.Count - 1].RemoveAt (i);
		}
		text1.RemoveAt (text1.Count - 1);

		firstOptionIndex.RemoveAt (firstOptionIndex.Count - 1);
	}

	/*
	 * Dear Future Self,
	 * I was feeling super lazy when I wrote this method,
	 * so I didn't include any comments. I pray that
	 * you will not have to go back and edit it, but
	 * if you do, good luck figuring out how it works.
	 * Sincerely,
	 * Nick
	 */
	void LoadOptions () {
		if (text1.Count < option1.Count) {
			firstOptionIndex.Add (0);
			List<Text> newText = new List<Text> ();
			for (int i = 0; i < optionsPerColumn; i++) {
				string str = option1 [option1.Count - 1] [i % option1 [option1.Count - 1].Count];
				Text tobj = GameObject.Instantiate (optionObject, optionContainer.transform);
				tobj.gameObject.SetActive (true);
				tobj.text = str;
				tobj.gameObject.GetComponent<RectTransform> ().localPosition = new Vector2 (-optionContainer.GetComponent<RectTransform> ().rect.width / 2, optionContainer.GetComponent<RectTransform> ().rect.height / 2) + new Vector2 (optionObject.GetComponent<RectTransform> ().rect.width * (option1.Count - 1), -optionObject.GetComponent<RectTransform> ().rect.height * i);
				newText.Add (tobj);
			}
			text1.Add (newText);
		} else if (text1.Count > option1.Count) {
			text1.RemoveAt (text1.Count - 1);
		} else {
			for (int i = 0; i < optionsPerColumn; i++) {
				string str = option1 [option1.Count - 1] [i % option1 [option1.Count - 1].Count];
				//Debug.Log ((i + firstOptionIndex [firstOptionIndex.Count - 1]) % text1 [text1.Count - 1].Count);
				text1 [text1.Count - 1] [(i + firstOptionIndex [firstOptionIndex.Count - 1]) % text1 [text1.Count - 1].Count].text = str;
			}
		}
	}

	public void AddAction (Action a) {
		switch (a.target) {
		case Action.Target.single:
			SelectTarget ();

			//add the next marker two makers
			GameObject m0 = GameObject.Instantiate (marker, optionContainer.transform);
			marker1.Add (m0);
			GameObject m1 = GameObject.Instantiate (marker, optionContainer.transform);
			marker1.Add (m1);
			//update all the required lists
			List<string> newColumn = new List<string> ();
			for (int i = 0; i < Game.current.playerCharacter.EngagedCharacters ().Count; i++) {
				newColumn.Add (Game.current.playerCharacter.EngagedCharacters () [i].name);
			}
			option1.Add (newColumn);
			index1.Add (0);
			LoadOptions ();

			/*
			List<Character> engaged = Game.current.playerCharacter.EngagedCharacters ();
			//remove invalid targets
			for (int i = 0; i < engaged.Count; i++) {
				if (!a.InRange (Game.current.playerCharacter, engaged [i])) {
					engaged.RemoveAt (i);
					i--;
				}
			}
			*/

			Game.camera.SetTarget (Game.current.playerCharacter.EngagedCharacters () [index1 [index1.Count - 1]]);

			System.Func<bool> f = () => {
				//select the target
				if (GameInput.GetKeyDown (GameInput.Bind.down)) {
					index1 [index1.Count - 1]++;
					if (index1 [index1.Count - 1] >= Game.current.playerCharacter.EngagedCharacters ().Count)
						index1 [index1.Count - 1] = 0;
					Game.camera.SetTarget (Game.current.playerCharacter.EngagedCharacters () [index1 [index1.Count - 1]]);
				}
				if (GameInput.GetKeyDown (GameInput.Bind.up)) {
					index1 [index1.Count - 1]--;
					if (index1 [index1.Count - 1] < 0)
						index1 [index1.Count - 1] = Game.current.playerCharacter.EngagedCharacters ().Count - 1;
					Game.camera.SetTarget (Game.current.playerCharacter.EngagedCharacters () [index1 [index1.Count - 1]]);
				}
				//change the position of both markers
				marker1 [marker1.Count - 2].GetComponent<RectTransform> ().localPosition = text1 [text1.Count - 1] [index1 [index1.Count - 1]].GetComponent<RectTransform> ().localPosition + markerOptionOffset;
				marker1 [marker1.Count - 1].GetComponent<RectTransform> ().position = GameObject.FindObjectOfType<Camera> ().WorldToScreenPoint(Game.current.playerCharacter.EngagedCharacters () [index1 [index1.Count - 1]].gameObject.transform.position) + markerTargetOffset;
				//marker.GetComponent<RectTransform> ().position = GameObject.FindObjectOfType<Camera> ().WorldToScreenPoint (engaged [index].gameObject.transform.position + markerTargetOffset);
				//cancels the action
				if (GameInput.GetKeyDown (GameInput.Bind.back)) {
					//GameTime.paused = false;!
					SelectAction ();
					return true;
				}
				//add the action to the player's turn
				if (GameInput.GetKeyDown (GameInput.Bind.interact)) {
					//Game.current.playerCharacter.DoAction (a, engaged [index]);
					playerTurn.action.Add (a);
					playerTurn.target.Add (Game.current.playerCharacter.EngagedCharacters () [index1 [index1.Count - 1]]);
					//GameTime.paused = false;!
					GameObject.Destroy (marker1 [marker1.Count - 1]);
					marker1.RemoveAt (marker1.Count - 1);
					Back ();
					SelectAction ();
					//SetTurnDisplay ();
					return true;
				}
				return false;
			};
			ActionSchedule.AddRepeatedAction (f);
			break;
		}
	}

	void ResetTurn () {
		playerTurn.action = new List<Action> ();
		playerTurn.target = new List<Character> ();
		ap.Reset ();
		//SetTurnDisplay ();
	}

	/*
	void SetTurnDisplay () {
		turnDisplay.text = "";
		for (int i = 0; i < playerTurn.action.Count; i++) {
			turnDisplay.text += playerTurn.action [i].name + " (" + playerTurn.target [i].name + ")";
			if (playerTurn.action.Count - i > 1) {
				turnDisplay.text += " -> ";
			}
		}
	}
	*/

	/*
	//the following methods simply switch tabs
	public void AttackTab () {

	}
	public void SkillsTab () {
		
	}
	public void ItemsTab () {

	}
	*/

	/*
	public void UpdateOptions () {
		List<Action> actions = Game.current.playerCharacter.AllActions ();
		for (int i = 0; i < actions.Count; i++) {
			//add an object if necessary
			if (i >= options.Count) {
				options.Add (GameObject.Instantiate (actionOption).GetComponent<ActionOption> ());
				options [i].gameObject.transform.SetParent (optionContainer.transform);
				//set the position of the action option object
				RectTransform rt = options [i].gameObject.GetComponent<RectTransform> ();
				//Vector3 pos = rt.position;
				rt.localPosition = new Vector3 (0, - 50 * i, 0) + actionOffset;
				rt.localScale = Vector3.one;
			}
			options [i].SetAction (actions [i]);
			//set the color of the option based on whether the player can actually do it
			if (actions [i].CanDo (Game.current.playerCharacter)) {
				//make the color black
				options [i].SetColor (Color.black);
			} else {
				//make the color red
				options [i].SetColor (Color.red);
			}
		}
		//destroy and delete excess objects
		for (int i = actions.Count; i < options.Count;) {
			GameObject.Destroy (options [i].gameObject);
			options.RemoveAt (i);
		}
	}
	*/

	public void Activate () {
		isPlayerTurn = true;
		gameObject.SetActive (true);
		optionContainer.SetActive (true);
		//actionInfo.gameObject.SetActive (true);
	}

	//deactivates everything but the marker
	public void Hide () {
		gameObject.SetActive (false);
	}

	public void Deactivate () {
		//gameObject.SetActive (false);
		isPlayerTurn = false;
		while (index1.Count > 1) {
			Back ();
		}
		index1 [0] = 0;
	}

	public void SelectAction () {
		isPlayerTurn = true;
		chooseTarget = false;
		gameObject.SetActive (true);
		optionContainer.gameObject.SetActive (true);
		//actionInfo.gameObject.SetActive (true);
		//marker.gameObject.SetActive (true);
	}

	public void SelectTarget () {
		chooseTarget = true;
		updateThisFrame = false;
		gameObject.SetActive (true);
		//optionContainer.gameObject.SetActive (false);
		//actionInfo.gameObject.SetActive (false);
		//marker.gameObject.SetActive (true);
	}

	/*
	public GameObject CreateOption (string name, int APcost, int FP, int maxFP) {
		ActionOption ao = GameObject.Instantiate (actionOption).GetComponent<ActionOption> ();
		//ao.Init ();
		return ao.gameObject;
	}
	*/
}
