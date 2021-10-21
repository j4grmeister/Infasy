using UnityEngine;
using System.Collections;

[System.Serializable]
public class Armor : Item {
	public int armor;

	public Armor() {
		//type = Type.armor;
		name = "armor";
		description = "desription";
	}

	public enum Class {
		robe
	}

	public static Armor CreateNewArmor (System.Type t) {
		Armor a = (Armor)System.Activator.CreateInstance (t);
		return a;
	}
}