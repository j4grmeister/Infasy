using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownsPersonAI : CharacterAI {
	static int minDaysBetweenQuests = 1;
	static int maxDaysBetweenQuests = 10;

	static float maxTimeBetweenRoamingMoves = 1.5f;
	static float maxRoamingMoveDistance = 3f;

	public House house;
	ushort townID = 0;
	ushort houseID = 0;

	public Town town {
		get {
			return Game.current.map.GetTownByID (townID);
		}
	}

	float timeBeforeNextMove = 0f;
	bool inHouse = true;

	protected override void ControllerStart () {
		townID = (ushort)character.characterInfo.Get ("townID");
		houseID = (ushort)character.characterInfo.Get ("houseID");
		house = (House)town.GetBuildingByID (houseID);
		//WalkTo (house.doorPosition + new Vector2 (3, -2));
		//timeBeforeNextMove = Walk (Map.Direction.down, 3f) + Random.Range (0f, maxTimeBetweenRoamingMoves); //for dev purposes only
	}

	protected override void ControllerUpdate () {
		//only roam during the day time
		if (Game.current.timeOfDay > WorldMap.sunriseTime && Game.current.timeOfDay < WorldMap.sunsetTime) { //day time
			//head outside if the character is in their house
			if (inHouse) {
				WalkTo (house.doorPosition + Vector2.down);
				inHouse = false;
			} else {
				//make the character roam around the town
				if (hasReachedTargetPosition) {
					timeBeforeNextMove -= GameTime.deltaTime;
				}
				if (timeBeforeNextMove <= 0) {
					List<Map.Movement> possibleMovements = town.GetValidMovementsOnRoad (character.position);
					if (possibleMovements.Count > 0) {
						int index = Random.Range (0, possibleMovements.Count);
						//Map.Direction moveDir = (Map.Direction)Random.Range (0, 4);
						Map.Direction moveDir = possibleMovements [index].direction;
						float moveDis = Random.Range (0f, Mathf.Min (possibleMovements [index].distance, maxRoamingMoveDistance));
						timeBeforeNextMove = Random.Range (0f, maxTimeBetweenRoamingMoves);
						Walk (moveDir, moveDis);
					}
				}
			}
		} else { //night time
			//return to house if it is night time
			if (!inHouse) {
				WalkTo (house.doorPosition + Vector2.up);
				inHouse = true;
			}
		}
	}

	public override void CharacterInit () {
		character.characterInfo.Add ("daysUntilNextQuest", -1);
		character.characterInfo.Add ("linearQuestIndex", 0);
	}

	public override void DailyUpdate () {
		if ((int)character.characterInfo.Get ("daysUntilNextQuest") > -1) {
			character.characterInfo.Set ("daysUntilNextQuest", (int)character.characterInfo.Get ("daysUntilNextQuest") - 1);
		} else {
			GenerateNewQuest ();
		}
	}

	//generates quests in a linear fashion
	//these quests are not time dependant
	//they are simply assigned in relation to the previous quest
	void GenerateNewQuest () {
		Random.InitState (Game.current.seed * ((int)character.characterInfo.Get ("linearQuestIndex") + 1) - (int)character.id);
		//select random quest type
		System.Type questType = Quest.questTypes [Random.Range (0, Quest.questTypes.Length)];
		Quest quest = (Quest)System.Activator.CreateInstance (questType, new object[] { character.id });
		quest.RandomizeParameters ();
		quest.Generate ();
		quest.id = Game.current.RequestID ();
		character.quests.Clear ();
		character.quests.Add (quest);
	}

	public override void OnPlayerInteract () {
		bool normalDialogue = false;
		if ((int)character.characterInfo.Get ("daysUntilNextQuest") == -1) {
			if (Game.current.playerCharacter.HasQuest (character.quests [0])) {
				character.Prompt ("hello, again. have you finished what i asked yet?", new string[] { "yes", "no" }, new System.Action[] {
					() => {
						if (character.quests [0].IsComplete ()) {
							character.quests [0].Complete ();
							character.characterInfo.Set ("linearQuestIndex", (int)character.characterInfo.Get ("linearQuestIndex") + 1); //increment the character's linear quest index by 1
							character.Say ("i am very grateful. i'll remember you next time i need help.");
						} else
							character.Say ("no you haven't. please come back to me when you have finished the task i gave you.");
					},
					() => character.Say ("oh, well if you wouldn't mind completing that, i would be very grateful.")
				});
			} else {
				normalDialogue = !character.quests [0].AssignToPlayer ();
			}
		} else {
			normalDialogue = true;
		}
		if (normalDialogue) {
			character.Say ("hi there! my name is " + character.name + ". we don't often have visitors here in " + Game.current.map.GetTownByID (townID).name + ", so it's nice to see a new face every once in a while.");
		}
	}
}