using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Class {
	public enum Type {
		wizard
	}

	public static int GetHitDice(Type t) {
		int d = 0;
		switch (t) {
		case Type.wizard:
			d = 6;
			break;
		}
		return d;
	}

	public static Library< List<string> > GetProficiencies(Type t) {
		Library< List<string> > prof = new Library< List<string> > ();
		List<string> armor = new List<string> ();
		List<string> weapons = new List<string> ();
		List<string> tools = new List<string> ();
		List<string> saveThrows = new List<string> ();
		List<string> skills = new List<string> ();
		switch (t) {
		case Type.wizard:
			weapons.Add ("daggers");
			weapons.Add ("darts");
			weapons.Add ("slings");
			weapons.Add ("quarterstaffs");
			weapons.Add ("light crossbows");
			prof.Add ("weapons", weapons);
			saveThrows.Add ("intelligence");
			saveThrows.Add ("wisdom");
			prof.Add ("saving throws", saveThrows);
			skills.Add ("arcana");
			skills.Add ("history");
			skills.Add ("insight");
			skills.Add ("investigation");
			skills.Add ("medicine");
			skills.Add ("religion");
			prof.Add ("skills", skills);
			break;
		}
		return prof;
	}

	public static List<string> GetStartingEquipment(Type t) {
		List<string> equip = new List<string> ();
		switch (t) {
		case Type.wizard:
			equip.Add ("dagger");
			equip.Add ("spellbook");
			break;
		}
		return equip;
	}

	public static int GetProficiencyBonus(Type t, int lv) {
		int b = 0;
		//switch (t) {
		//case Type.wizard:
			if (lv < 5)
				b = 2;
			else if (lv < 9)
				b = 3;
			else if (lv < 13)
				b = 4;
			else if (lv < 17)
				b = 5;
			else
				b = 6;
			//break;
		//}
		return b;
	}

	public static int GetSpellSlots(Type t, int lv, int slv) {
		int q = 0;
		switch (t) {
		case Type.wizard:
			if (slv == 1) {
				if (lv == 1)
					q = 2;
				if (lv == 2)
					q = 3;
				if (lv < 10)
					q = 4;
				else
					q = 5;
			} else if (slv == 2) {
				if (lv < 3)
					q = 0;
				else if (lv == 3)
					q = 2;
				else
					q = 3;
			} else if (slv == 3) {
				if (lv < 5)
					q = 0;
				else if (lv == 5)
					q = 2;
				else
					q = 3;
			} else if (slv == 4) {
				if (lv < 7)
					q = 0;
				else if (lv == 7)
					q = 1;
				else if (lv == 8)
					q = 2;
				else
					q = 3;
			} else if (slv == 5) {
				if (lv < 9)
					q = 0;
				else if (lv == 9)
					q = 1;
				else if (lv < 18)
					q = 2;
				else
					q = 3;
			} else if (slv == 6) {
				if (lv < 11)
					q = 0;
				else if (lv < 19)
					q = 1;
				else
					q = 2;
			} else if (slv == 7) {
				if (lv < 13)
					q = 0;
				else if (lv < 20)
					q = 1;
				else
					q = 2;
			} else if (slv == 8) {
				if (lv < 15)
					q = 0;
				else
					q = 1;
			} else if (slv == 9) {
				if (lv < 17)
					q = 0;
				else
					q = 1;
			}
				
			break;
		}
		return q;
	}
}