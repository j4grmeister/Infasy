using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Inventory {

	public int gold;

	public List<Item> allItems {
		get {
			List<Item> l = new List<Item> ();
			for (int i = 0; i < items.Count; i++) {
				l.Add (items [i]);
			}
			for (int i = 0; i < weapons.Count; i++) {
				l.Add ((Item)weapons [i]);
			}
			for (int i = 0; i < armor.Count; i++) {
				l.Add ((Item)armor [i]);
			}
			return l;
		}
	}
	//List<bool> equipped = new List<bool> ();
	public List<Weapon> weapons = new List<Weapon> ();
	Weapon ew;
	public Weapon equippedWeapon {
		get {
			if (ew == null) {
				ew = Weapon.CreateNewWeapon (typeof(Weapon.Fists));
			}
			return ew;
		}
		set {
			if (value == null) {
				ew = Weapon.CreateNewWeapon (typeof(Weapon.Fists));
			} else {
				ew = value;
			}
		}
	}
	public List<Item> items = new List<Item> ();
	public List<Armor> armor = new List<Armor> ();
	public List<Armor> equippedArmor = new List<Armor> ();

	public void Add (Item i) {
		if (i.GetType ().IsSubclassOf (typeof(Weapon))) { //weapon
			weapons.Add ((Weapon)i);
		} else if (i.GetType ().IsSubclassOf (typeof(Armor))) { //armor
			armor.Add ((Armor)i);
		} else { //item
			items.Add (i);
		}
	}
	public void Remove (Item i) {
		if (i.GetType ().IsSubclassOf (typeof(Weapon))) {
			weapons.Remove ((Weapon)i);
		} else if (i.GetType ().IsSubclassOf (typeof(Armor))) {
			armor.Remove ((Armor)i);
		} else {
			items.Remove (i);
		}
	}

	public void RemoveAt (int i) {
		Remove (allItems [i]);
	}

	public void Equip (Item i) {
		if (i.GetType ().IsSubclassOf (typeof(Weapon))) {
			Equip ((Weapon)i);
		} else if (i.GetType ().IsSubclassOf (typeof(Armor))) {
			Equip ((Armor)i);
		} else {
			
		}
	}
	public void Equip (Weapon w) {
		equippedWeapon = w;
	}
	public void Equip (Armor a) {
		for (int i = 0; i < equippedArmor.Count; i++) {
			if (equippedArmor [i].GetType ().BaseType == a.GetType ().BaseType) {
				equippedArmor.RemoveAt (i);
				break;
			}
		}
		equippedArmor.Add ((Armor)a);
	}

	public void UnequipWeapon () {
		equippedWeapon = null;
	}

	public void Unequip (Armor a) {
		equippedArmor.Remove (a);
	}

	public bool Equipped (Weapon w) {
		return (equippedWeapon == w);
	}

	public bool Equipped (Armor a) {
		return equippedArmor.Contains (a);
	}

	/*
	public void Equip (int i) {
		equipped [i] = true;
	}

	public void Equip (Item i) {
		equipped [items.IndexOf (i)] = true;
	}

	public void Unequip (int i) {
		equipped [i] = false;
	}

	public void Unequip (Item i) {
		equipped [items.IndexOf (i)] = false;
	}

	public List<Item> GetEquippedItems() {
		List<Item> e = new List<Item> ();
		for (int i = 0; i < equipped.Count; i++) {
			if (equipped [i])
				e.Add (items [i]);
		}
		return e;
	}
	*/
}