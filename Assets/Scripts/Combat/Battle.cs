   using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Battle {
	public struct Turn {
		public List<Action> action;
		public List<Character> target;
	};

	public static float timeBetweenActions = 2f;

	List<Character> combatants = new List<Character> ();
	Turn currentTurn;
	//List<int> order = new List<int> (); //list of indexes of the combatants in the order of their turns
	int turn = 0; //the current place in the order
	bool active = false;
	public int playerExpReward = 0;

	//starts the battle; called after all combatants are added
	public void Start () {
		//make the player the first in the order if they are involved in this battle
		if (combatants.Contains (Game.current.playerCharacter)) {
			turn = combatants.IndexOf (Game.current.playerCharacter);
		}
		MoveCombatantsToPositions ();
		active = true;
	}

	public void MoveCombatantsToPositions () {
		//freeze all combatants by disabling their controller scripts
		for (int i = 0; i < combatants.Count; i++) {
			FreezeCombatant (i);
		}

		//move all combatants to their starting positions before unfeezing the first combatant
		float time = 0f; //the time until the last combatant will arrive in its position
		for (int i = 0; i < combatants.Count; i++) {
			Vector2 tPos = new Vector2 ((int)combatants [i].position.x, (int)combatants [i].position.y);
			Vector2 move = tPos - combatants [i].position;
			time = Mathf.Max (time, (move.x + move.y) / combatants [i].speed);
			Character c = combatants [i];
			System.Func<bool> moveCombatantY = () => {
				float distance = tPos.y - c.position.y;
				if (distance <= GameTime.deltaWorldTime * c.speed) {
					Vector2 p = c.position;
					p.y = tPos.y;
					c.position = p;
					return true;
				}
				c.Walk ((move.y >= 0) ? Map.Direction.up : Map.Direction.down);
				return false;
			};
			System.Func<bool> moveCombatantX = () => {
				float distance = tPos.x - c.position.x;
				if (distance <= GameTime.deltaWorldTime * c.speed) {
					Vector2 p = c.position;
					p.x = tPos.x;
					c.position = p;
					ActionSchedule.AddRepeatedAction (moveCombatantY);
					return true;
				}
				c.Walk ((move.x >= 0) ? Map.Direction.right : Map.Direction.left);
				return false;
			};
			ActionSchedule.AddRepeatedAction (moveCombatantX);
		}
		//unfreeze the first combatant after all combatants have arrived at their locations
		System.Action unfreezeFirst = () => {
			//UnfreezeCombatant (turn);
			NextCombatant ();
		};
		ActionSchedule.ScheduleAction (unfreezeFirst, time);
	}

	//ends the battle
	public void End () {
		bool playerWon = false;
		//perform necessary actions on all combatants
		for (int i = 0; i < combatants.Count; i++) {
			UnfreezeCombatant (i);
			//combatants [i].ReplenishAP ();
			combatants [i].Disengage ();
			if (combatants [i].id == Game.current.playerCharacter.id)
				playerWon = true;
		}
		//remove all combatants from this battle
		combatants.Clear ();
		//hide the action menu
		Game.actionMenu.Hide ();
		//remove this battle from the game's list of ongoing battles
		Game.current.battles.Remove (this);
		//if the player won, give them the reward for winning the battle
		if (playerWon)
			GiveSpoils ();
	}

	void GiveSpoils () {
		//dialogue must be added before awarding exp
		//this ensures that in the event of a level up, dialogue is presented in the correct order
		Game.dialogueMenu.AddDialogue (null, "you gained " + playerExpReward + " exp!");
		Game.current.playerCharacter.GainExp (playerExpReward);
		//award gold
		int goldReward = GameBalance.GoldGained (playerExpReward);
		Game.dialogueMenu.AddDialogue (null, "you found " + goldReward + " gold.");
		Game.current.playerCharacter.inventory.gold += goldReward;
	}

	//ends the given character's turn and proceed's to the next character in the order
	public void EndTurn (Character c) {
		//check that it is actually the turn of the character who is trying to ending its turn
		//that way characters can't end the turn of other characters
		if (combatants.IndexOf (c) == turn) {
			//perform the combatant's turn
			DoTurn (combatants [turn], currentTurn);

			//freeze the combatant which just finished his/her turn
			//FreezeCombatant (turn);
		}	
	}

	public void StartTurn () {
		CleanUp ();

		//end the battle if it should be ended
		//continue if it should keep going
		if (IsOver ()) {
			End ();
		} else {
			//advance to the next turn
			turn++;
			if (turn >= combatants.Count)
				turn = 0;
			//unfreeze the next combatant and replenish his/her action points
			//combatants [turn].ReplenishAP ();
			//UnfreezeCombatant (turn);
			NextCombatant ();
		}
	}

	//performs the provided turn as the provided character
	public void DoTurn (Character c, Turn t) {
		int i = 0;
		System.Action doNextAction = null;
		doNextAction = () => {
			float time = c.DoAction (t.action [i], t.target [i]) + timeBetweenActions;
			i++;
			if (i < t.action.Count) {
				ActionSchedule.ScheduleAction (doNextAction, time);
			} else {
				System.Action startNextTurn = () => { StartTurn (); };
				ActionSchedule.ScheduleAction (startNextTurn, time);
			}
		};
		doNextAction ();
	}

	public void NextCombatant () {
		//open the action menu if it is the player's turn
		if (combatants [turn] == Game.current.playerCharacter) {
			//GameTime.paused = true;!
			//Game.current.actionMenu.UpdateOptions ();
			if (combatants.Contains (Game.current.playerCharacter)) {
				Game.camera.SetTarget (combatants [turn]); 
			}
			Game.actionMenu.SelectAction ();
		} else {
			SubmitTurn (combatants [turn].controller.GetTurn ());
			System.Action et = () => { EndTurn (combatants [turn]); };
			ActionSchedule.ScheduleAction (et, Game.camera.transitionTime);
			if (combatants.Contains (Game.current.playerCharacter)) {
				Game.camera.SetTarget (combatants [turn]);
			}
		}
	}

	public void SubmitTurn (Turn t) {
		currentTurn = t;
	}

	public void Add (Character c) {
		if (active) {
			combatants.Add (c);
			FreezeCombatant (combatants.Count - 1);
		} else {
			combatants.Add (c);
		}
	}

	//returns true if this battle should end
	public bool IsOver () {
		for (int i = 1; i < combatants.Count; i++) {
			if (Character.AreHostile (combatants [i - 1], combatants [i])) {
				return false;
			}
		}
		return true;
	}

	//returns true if the given character is a combatant in this battle
	public bool IsCombatant (Character c) {
		return combatants.Contains (c);
	}

	public Character CurrentCombatant () {
		return combatants [turn];
	}

	//performs some basic essential operations
	void CleanUp () {
		//check for deaths
		for (int i = 0; i < combatants.Count; i++) {
			//remove the combatant if he/she is dead
			if (combatants [i].IsDead ()) {
				combatants.RemoveAt (i);
			}
		}
		//check for any disengaged combatants and remove them
		for (int i = 0; i < combatants.Count; i++) {
			//remove the combatant if he/she is disengaged from everybody
			//also unfreeze them
			if (!combatants [i].InCombat ()) {
				UnfreezeCombatant (i);
				combatants.RemoveAt (i);
			}
		}
	}

	void FreezeCombatant (int i) {
		combatants [i].Freeze ();
	}

	void UnfreezeCombatant (int i) {
		combatants [i].Unfreeze ();
	}
}