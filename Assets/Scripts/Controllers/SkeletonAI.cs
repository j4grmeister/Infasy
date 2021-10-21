using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonAI : CharacterAI {

	protected override void ControllerStart () {
		
	}

	public override Battle.Turn GetTurn () {
		Battle.Turn turn;
		turn.action = new List<Action> ();
		turn.target = new List<Character> ();

		List<Action> actions = character.AvailableActions ();
		//determine the closest combatant and make it the target of the next attack
		List<Character> otherCharacters = character.EngagedCharacters ();
		Character target = otherCharacters [0];
		float targetDistance = Vector2.Distance (character.position, target.position);
		for (int i = 1; i < otherCharacters.Count; i++) {
			if (Vector2.Distance (character.position, otherCharacters [i].position) < targetDistance) {
				target = otherCharacters [i];
				targetDistance = Vector2.Distance (character.position, otherCharacters [i].position);
			}
		}
		//select the best action
		/*
		Action bestAction = actions [0];
		for (int i = 1; i < actions.Count; i++) {

		}
		//add the action to the turn
		turn.action.Add (bestAction);
		turn.target.Add (target);
		*/
		turn.action.Add (character.inventory.equippedWeapon.action);
		turn.target.Add (target);

		return turn;
	}
}
