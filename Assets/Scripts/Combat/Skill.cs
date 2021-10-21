using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill {
	public static Library<Action> skills = new Library<Action> ();

	public static void Init () {
		AddSkill (new FireBolt ());
	}

	public static void AddSkill (Action s) {
		skills.Add (s.name, s);
	}
}