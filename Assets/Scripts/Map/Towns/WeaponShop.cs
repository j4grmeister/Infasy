using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShop : Shop {
	static ItemProbability[] probability = {
		new ItemProbability (typeof(Sword.RustySword), 1f),
		new ItemProbability (typeof(Sword.SteelSword), 1f),
		new ItemProbability (typeof(Sword.MythrilSword), 1f)
	};

	public WeaponShop (int x, int y) : base (x, y) {

	}

	public override void Buy (int i) {
		Game.current.playerCharacter.inventory.gold -= inventory [i].cost;
		gold += inventory [i].cost;
		Game.current.playerCharacter.inventory.weapons.Add ((Weapon)inventory [i]);
		inventory.RemoveAt (i);
	}
	public override void Sell (int i) {
		Game.current.playerCharacter.inventory.gold += Game.current.playerCharacter.inventory.weapons [i].cost;
		gold -= Game.current.playerCharacter.inventory.weapons [i].cost;
		inventory.Add (Game.current.playerCharacter.inventory.weapons [i]);
		Game.current.playerCharacter.inventory.weapons.RemoveAt (i);
	}

	protected override void GenerateShop () {
		//add sign to the right side of the door
		tiles [doorX + 1, 0].underType = Tile.UnderType.sign_weapon;
	}

	protected override void GenerateInventory () {
		//inventory.Add (new Sword ());
		for (int i = 0; i < probability.Length; i++) {
			float r = Random.Range (0f, 1f);
			if (r <= probability [i].probability) {
				inventory.Add (Weapon.CreateNewWeapon (probability [i].itemType));
			}
		}
	}
}