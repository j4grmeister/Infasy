using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScript : MonoBehaviour {
	public ActionMenu action;
	public MenuScript menu;
	public DialogueMenu dialogue;
	public ShopMenu shop;
	public LootMenu loot;

	Vector2 lastScreenSize;

	// Use this for initialization
	void Start () {
		Game.ui = this;
		Game.actionMenu = action;
		Game.dialogueMenu = dialogue;
		Game.shopMenu = shop;
		Game.lootMenu = loot;
	}
		
	// Update is called once per frame
	void Update () {
		/*
		Vector2 screenSize = new Vector2 (Screen.width, Screen.height);
		if (screenSize != lastScreenSize) {
			map.OnWindowResize ();
			lastScreenSize = screenSize;
		}
		*/

		/*
		//open the action menu
		if (Input.GetKeyDown (KeyCode.Q)) {
			action.UpdateOptions ();
			GameTime.paused = true;
			action.Activate ();
		}
		*/

		//open or close the menu
		if (GameInput.GetKeyDown (GameInput.Bind.pause)) {
			GameTime.paused = !GameTime.paused;
			menu.gameObject.SetActive (!menu.gameObject.activeSelf);
			menu.GoHome ();
		}
	}
}
