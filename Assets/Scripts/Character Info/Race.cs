using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Race {
	public enum Type {
		human
	}

	public enum Gender {
		male,
		female
	}

	public static Library<int> GetAbilityScoreBonus(Type t) {
		Library<int> abilityBonus = new Library<int> ();
		switch (t) {
		case Type.human:
			abilityBonus.Add ("strength", 1);
			abilityBonus.Add ("dexterity", 1);
			abilityBonus.Add ("constitution", 1);
			abilityBonus.Add ("intelligence", 1);
			abilityBonus.Add ("wisdom", 1);
			abilityBonus.Add ("charisma", 1);
			break;
		}
		return abilityBonus;
	}

	public static int GetBaseSpeed(Type t) { //base speed in feet
		int s = 0;
		switch (t) {
		case Type.human:
			s = 30;
			break;
		}
		return s;
	}

	//generates a full name for the given race and gender
	public static string GenerateName(Type t, Gender g) {
		string genderStr = g.ToString ();
		string path = "Data/Random/" + t.ToString() + "Names";
		TextAsset file = Resources.Load<TextAsset> (path);
		string data = file.text;
		string first = "";
		string last = "";

		//find beginning index of first names
		int i = 0;
		for (; true; i++) {
			if (data [i] == '<') {
				string match = data.Substring (i + 1, 5);
				if (match == "first") {
					break;
				}
			}
		}
		for (; true; i++) {
			if (data [i] == '<') {
				string match = data.Substring (i + 1, genderStr.Length);
				if (match == genderStr) {
					i += genderStr.Length + 3;
					break;
				}
			}
		}
		//create a list of first names
		List<string> fnames = new List<string> ();
		for (int l = 0; true; l++) {
			if (data [i] == '<') {
				string match = data.Substring (i + 1, genderStr.Length + 1);
				if (match == "/" + genderStr) {
					break;
				}
			}
			if (data [i + l] == '\n') {
				fnames.Add (data.Substring (i, l));
				i += l + 1;
				l = 0;
			}
		}

		//find beginning index of last names
		i = 0;
		for (; true; i++) {
			if (data [i] == '<') {
				string match = data.Substring (i + 1, 4);
				if (match == "last") {
					break;
				}
			}
		}
		//create a list of first names
		List<string> lnames = new List<string> ();
		for (int l = 0; true; l++) {
			if (data [i] == '<') {
				string match = data.Substring (i + 1, 5);
				if (match == "/last") {
					break;
				}
			}
			if (data [i + l] == '\n') {
				lnames.Add (data.Substring (i, l));
				i += l + 1;
				l = 0;
			}
		}

		//generate and return a random name
		first = fnames [Random.Range(0, fnames.Count)];
		last = lnames [Random.Range(0, lnames.Count)];
		return first + " " + last;
	}
}