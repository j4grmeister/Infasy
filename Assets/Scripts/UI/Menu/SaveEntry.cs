using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveEntry : MonoBehaviour {
	public Text saveTitle;

	GameSave target;
	int targetIndex;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public int GetIndex () {
		return targetIndex;
	}

	public void SetSave (GameSave target, int i) {
		this.target = target;
		targetIndex = i;
		if (i == -1) {
			saveTitle.text = "New Save";
		} else {
			saveTitle.text = target.name + " " + (i + 1).ToString ();
		}
	}
}
