using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmorEntry : ItemEntry {
	//public Text name;
	public Color defaultColor;
	public Color equippedColor;

	//Armor armor;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void SetItem (Item i) {
		item = i;
		name.text = item.name;
		/*
		if (Game.current.playerCharacter.inventory.Equipped ((Armor)item)) {
			name.color = equippedColor;
		} else {
			name.color = defaultColor;
		}
		*/
	}
}
