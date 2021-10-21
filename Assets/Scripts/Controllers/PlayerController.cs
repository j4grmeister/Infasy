using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Controller {
	static int maxInteractDistance = 2;
	
	// Update is called once per frame
	protected override void ControllerUpdate () {
		//Debug.Log (character.InCombat ());
		//only move the player's character if it isn't in battle
		if (!character.InCombat ()) {
			//move the player's character
			//float horizontal = Input.GetAxisRaw ("Horizontal");
			//float vertical = Input.GetAxisRaw ("Vertical");
			//character.Walk (new Vector2 (horizontal, vertical));
			if (GameInput.GetKey (GameInput.Bind.up)) {
				character.Walk (Map.Direction.up);
			} else if (GameInput.GetKey (GameInput.Bind.down)) {
				character.Walk (Map.Direction.down);
			} else if (GameInput.GetKey (GameInput.Bind.left)) {
				character.Walk (Map.Direction.left);
			} else if (GameInput.GetKey (GameInput.Bind.right)) {
				character.Walk (Map.Direction.right);
			}
		}

		if (GameInput.GetKeyDown (GameInput.Bind.interact)) {
			bool interacted = false;
			//check for living NPCs to talk to
			Vector2 dir = Map.Vector2FromDirection (character.faceDirection);
			for (int d = 1; d <= maxInteractDistance; d++) {
				Vector2 tp = character.position + dir * d;
				Character c = Game.current.GetCharacterAt (tp + new Vector2 (.5f, .5f));
				if (c != null) {
					interacted = true;
					c.controller.OnPlayerInteract ();
					break;
				}
			}
			//check for dead NPCs to loot
			if (!interacted) {
				List<Character> c = Game.current.GetDeadCharactersAt (character.position + new Vector2 (.5f, .5f));
				if (c.Count > 0) {
					List<Inventory> loot = new List<Inventory> ();
					for (int i = 0; i < c.Count; i++) {
						loot.Add (c [i].inventory);
						//Debug.Log (c [i].inventory.weapons.Count);
					}
					//Debug.Log (loot [0].allItems.Count);
					Game.lootMenu.Loot (loot);
				}
			}
		}
		/*
		//end the player's turn
		if (Input.GetKeyDown (KeyCode.E)) {
			character.EndTurn ();
		}
		*/

		/*
		//open the map
		if (Input.GetKeyDown (KeyCode.M)) {
			GameTime.TogglePause (); //pause or unpause the game the game
			mapMenu.gameObject.SetActive (!mapMenu.gameObject.activeSelf);
		}
		*/
	}
}
