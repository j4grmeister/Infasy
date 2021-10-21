using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponEntry : ItemEntry {
	
	//public Text name;
	public Color defaultColor;
	public Color equippedColor;

	//Weapon weapon;


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
		if (Game.current.playerCharacter.inventory.Equipped ((Weapon)item)) {
			name.color = equippedColor;
		} else {
			name.color = defaultColor;
		}
		*/
	}
}
