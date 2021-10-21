using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBalance {
	//leveling
	public static float ExpForLevel (float level) {
		//float e = 10000 * Mathf.Pow (1.1, .1f * (level - 100)) - 3755;
		//float e = 100f * Mathf.Pow(level, 2f);
		float e = 100f * Mathf.Pow (2, level);
		return e;
	}

	public static float LevelForExp (float exp) {
		//float l = Mathf.Sqrt (exp) / 10f;
		float l = Mathf.Log (exp / 100f) / Mathf.Log (2);
		return l;
	}

	public static float ExpGained (float enemyExp) {
		//float e = Mathf.Pow(enemyExp, 3f / 10f) + 50f;
		float e = Mathf.Pow (5f * (ExpForLevel (LevelForExp (enemyExp) + 1) - enemyExp), .5f);
		return e;
	}

	public static int GoldGained (int exp) {
		//float e = expGained / 12f;
		//float e = Mathf.Pow (expGained, 2f) / 200f;
		float e = Mathf.Pow (ExpGained (exp), 2f) / Mathf.Pow (1.5f, LevelForExp (exp));
		return Mathf.RoundToInt (e);
	}

	//abilities
	//public static int standardBaseAbilityScore = 7;

	public static int GetMaxHP (int level, int constitution) {
		return constitution * 8 * level;
	}

	public static int GetMaxAP (int dexterity) {
		return dexterity / 3;
	}

	public static int GetMaxFP (int intelligence) {
		return intelligence * 7;
	}

	public static int GetFPRegenPerTurn (int charisma) {
		return charisma * 2;
	}

	//combat
	public static int GetStandardHP (int level) {
		return 20 + 10 * (level - 1);
	}

	public static int GetStandardStrength (int level) {
		return 3;
	}

	public static int GetStandardArmor (int level) {
		return 2;
	}
}