using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour {
	//public List<MenuTab> tabs;
	public StatsTab home;

	//List<Text> options = new List<Text> ();
	int index = 0;
	int lastTabIndex = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		/*
		if (GameInput.GetKeyDown (GameInput.Bind.left)) {
			index--;
			if (index < 0)
				index = tabs.Count - 1;
			SetTab ();
		}
		if (GameInput.GetKeyDown (GameInput.Bind.right)) {
			index++;
			if (index >= tabs.Count)
				index = 0;
			SetTab ();
		}
		*/
	}

	public void GoHome () {
		for (int i = 0; i < home.subMenus.Count; i++) {
			home.subMenus [i].gameObject.SetActive (false);
		}
		home.UpdateTab ();
		home.ShowFull ();
		home.gameObject.SetActive (true);
	}

	/*
	void SetTab () {
		tabs [lastTabIndex].gameObject.SetActive (false);
		tabs [index].gameObject.SetActive (true);
		tabs [index].UpdateTab ();
		lastTabIndex = index;
	}
	*/
}