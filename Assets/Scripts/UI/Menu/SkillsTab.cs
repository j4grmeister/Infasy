using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillsTab : MenuTab {
	public Text skillDescription;
	public SkillEntry skill;
	public GameObject skillContainer;
	public GameObject marker;
	public Vector2 markerOffset;
	public Vector2 skillOffset;

	List<SkillEntry> skills = new List<SkillEntry> ();
	int index = 0;
	int lastIndex = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (GameInput.GetKeyDown (GameInput.Bind.down)) {
			index++;
			if (index >= skills.Count)
				index = 0;
			SetSkill ();
		}
		if (GameInput.GetKeyDown (GameInput.Bind.up)) {
			index--;
			if (index < 0)
				index = skills.Count - 1;
			SetSkill ();
		}
		if (GameInput.GetKeyDown (GameInput.Bind.back)) {
			Game.ui.menu.GoHome ();
		}
	}

	void SetSkill () {
		marker.gameObject.GetComponent<RectTransform> ().localPosition = skills [index].gameObject.GetComponent<RectTransform> ().localPosition + (Vector3)markerOffset;

		skillDescription.text = skills [index].GetAction ().ap.ToString () + " AP\n\n" +
		skills [index].GetAction ().fp.ToString () + " FP\n\n" +
		"Target: " + skills [index].GetAction ().target.ToString () + "\n\n" +
		skills [index].GetAction ().description;

		lastIndex = index;
	}

	public override void UpdateTab () {
		for (int i = 0; i < Game.current.playerCharacter.skills.Count; i++) {
			if (i >= skills.Count) {
				SkillEntry e = GameObject.Instantiate (skill, skillContainer.transform);
				e.gameObject.GetComponent<RectTransform> ().localPosition = new Vector3 (skillOffset.x, e.gameObject.GetComponent<RectTransform> ().rect.height * i + skillOffset.y);
				skills.Add (e);
			}
			skills [i].SetAction (Skill.skills.Get (Game.current.playerCharacter.skills [i]));
		}
		for (int i = Game.current.playerCharacter.skills.Count; i < skills.Count;) {
			GameObject.Destroy (skills [i].gameObject);
			skills.RemoveAt (i);
		}
		index = 0;
		SetSkill ();
	}
}
