using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillEntry : MonoBehaviour {
	public Text name;
	public Text info;

	Action action;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetAction (Action action) {
		this.action = action;
		name.text = action.name;
		info.text = action.ap.ToString () + "/" + action.fp.ToString ();
	}

	public Action GetAction () {
		return action;
	}
}
