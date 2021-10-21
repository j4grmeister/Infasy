using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionInfo : MonoBehaviour {
	public Text name;
	public Text AP;
	public Text FP;
	public Text target;
	public Text description;

	Action action;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetAction (Action a) {
		action = a;
		Load ();
	}

	void Load () {
		name.text = action.name;
		FP.text = action.ap.ToString () + " AP";
		if (action.maxfp == -1) {
			FP.text = "-/- FP";
		} else {
			FP.text = Game.current.playerCharacter.fp + "/" + action.maxfp.ToString () + " FP";
		}
		target.text = "Target: " + action.target.ToString ();
		description.text = action.description;
	}
}