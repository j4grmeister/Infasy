using UnityEngine;
using System.Collections;

[System.Serializable]
public class Item {
	//public Type type;
	public string name;
	public string description;
	public int cost;

	/*
	public enum Type {
		armor,
		weapon,
		consumable
	};
	*/

	public Item () {
		name = "Item";
		description = "Desription";
		cost = 0;
	}

	public virtual string InfoString () {
		return name + "\t" + cost + "gp";
	}
}