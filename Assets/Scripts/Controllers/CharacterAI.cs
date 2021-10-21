using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAI : Controller {
	// Update is called once per frame
	protected override void ControllerUpdate () {
		CheckSight ();
	}

	protected void CheckSight () {
		//check the sight of the character
		float maxAngle = character.sightWidth / (2 * character.sightWidth);
		for (int i = 0; i < Game.current.characters.Count; i++) {
			if (Game.current.characters [i] == character) {
				continue;
			}
			//switch x and y coordinates if the character is facing north or south, for sake of simplifying calculation
			//also make sure x is positive
			Vector2 relative = Game.current.characters [i].position - character.position;
			if (relative.magnitude <= character.sightDistance) {
				if (Mathf.Deg2Rad * Vector2.Angle (Map.Vector2FromDirection (character.faceDirection), relative.normalized) <= maxAngle) {
					//at this point, the character has seen another character
					OnCharacterSeen (Game.current.characters [i]);
				}
			}
		}
	}

	protected virtual void OnCharacterSeen (Character c) {
		if (!character.EngagedWith (c) && c.alignment != character.alignment) {
			Game.current.Engage (character, c);
		}
	}
}
