using UnityEngine;
using System.Collections;

public class Ability {
	public enum Type {
		strength,
		dexterity,
		constitution,
		intelligence,
		wisdom,
		charisma
	}

	public Type type;
	public int score;
	public int modifier;

	public Ability(Type t, int s, int m) {
		type = t;
		score = s;
		modifier = m;
	}
}