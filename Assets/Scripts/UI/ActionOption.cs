using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionOption : MonoBehaviour {

	public Text name;
	public Text AP;
	public Text FP;

	Action action;

	/*
	string name;
	int AP;
	int FP;
	int maxFP;
	*/

	// Use this for initialization
	void Start () {
		name = gameObject.transform.Find ("Name").gameObject.GetComponent<Text> ();
		AP = gameObject.transform.Find ("AP").gameObject.GetComponent<Text> ();
		FP = gameObject.transform.Find ("FP").gameObject.GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Init () {
		/*
		Button b = gameObject.GetComponent<Button> ();
		b.onClick.AddListener (DoAction);
		*/
	}
		
	/*
	public void DoAction () {
		//Game.current.playerCharacter.DoAction (name);
		//Game.current.playerCharacter.DoAction (action, null);
		Game.current.playerCharacter.readiedAction = action;
	}
	*/

	public void SetColor (Color c) {
		name.color = c;
		AP.color = c;
		FP.color = c;
	}

	public void SetAction (Action a) {
		action = a;
		name.text = action.name;
		AP.text = action.ap + " AP";
		if (action.maxfp > -1) {
			FP.text = Game.current.playerCharacter.fp + "/" + action.maxfp.ToString () + " FP";
		} else {
			FP.text = "";
		}
	}

	public Action Action() {
		return action;
	}
}
