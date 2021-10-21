using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScript : MonoBehaviour {

	//public Character player;
	public CodeCanvas codeCanvas;
	public UIScript ui;

	void Awake() {
		//for dev purposes only
		//Game.current = new Game ();
		//Game.current.map = new Map ();
		Game.codeCanvas = codeCanvas;
		//Game.ui = ui;

		/*
		//Character player = Character.CreateCharacter (Character.Race.human_male, Character.Alignment.good);
		//player.gameObject.AddComponent<PlayerController> ();
		Game.current.playerCharacter = Character.CreateCharacter (Character.Race.human_male, Character.Alignment.good);
		Game.current.playerCharacter.gameObject.AddComponent<PlayerController> ();
		Game.current.playerCharacter.position = new Vector2 (105, 140);
		Game.current.playerCharacter.name = "Magnus Burnsides";
		//Weapon sword = new Weapon (Weapon.Type.sword, 10, 5);
		Sword sword = new Sword ();
		Game.current.playerCharacter.inventory.Add (sword);
		Game.current.playerCharacter.inventory.Equip (sword);
		Game.current.playerCharacter.LearnSkill ("Fire Bolt");


		Character skeleton = Character.CreateCharacter (Character.Race.skeleton, Character.Alignment.evil);
		skeleton.gameObject.AddComponent<CharacterAI> ();
		skeleton.name = "Skeleton";
		skeleton.position = new Vector2 (101, 145);
		Game.current.AddCharacter (skeleton);
		*/
	}

	// Use this for initialization
	void Start () {
		ui.menu.gameObject.SetActive (false);

		GameTime.ResetPlayTime ();
		//for dev purposes only
		/*
		Armor a = new Armor ();
		a.armorClass = Armor.Class.robe;
		a.armorType = Armor.ArmorType.torso;
		player.inventory.Add(a);
		player.inventory.Equip (0);
		*/


		//player.UpdateEquipment ();

		for (int i = 0; i < Game.current.characters.Count; i++) {
			//Debug.Log (Game.current.characters [i].name);
			//Game.current.characters [i].transform.SetParent (gameObject.transform);
		}
		gameObject.transform.DetachChildren ();
		Game.Start ();
	}
		
	// Update is called once per frame
	void Update () {
		//call the GameTime.Update method. this is necessary for certain operations to take place
		Game.Update ();
	}
}
