using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this type of quests requires the player to hunt down a specific entity and defeat it
//significance basically acts as a difficulty for this type of quest
public class KillQuest : Quest {
	//first index corresponds to the level of significance; READ BELOW COMMENT
	static Character.Race[][] possibleTargetRaces = {
		new Character.Race[] { Character.Race.skeleton},
		new Character.Race[] { Character.Race.skeleton},
		new Character.Race[] { Character.Race.skeleton},
		new Character.Race[] { Character.Race.skeleton}
	};


	static float maxUnimportantTargetDistanceFromTown = 20;

	ushort targetID = 0;
	Character target {
		get {
			return Game.current.GetCharacterByID (targetID);
		}
	}

	ushort targetMapID;
	Vector2 targetSpawnPosition;
	Character.Race targetRace;
	Character.Alignment targetAlignment;
	System.Type targetControllerType;

	string caveName;

	public KillQuest (ushort giver) : base (giver) {
		
	}

	public override void Generate () {
		base.Generate ();
		//significance basically acts as a difficulty
		targetRace = possibleTargetRaces [0][Random.Range (0, possibleTargetRaces [0].Length)];
		targetAlignment = Character.Alignment.evil;
		targetControllerType = Character.GetDefaultControllerType (targetRace);
		switch (significance) {
		case Significance.unimportant:
			SpawnNearTown ();
			dialogue = "i noticed a " + targetRace.ToString () + " near town recently and he makes me a little uncomfortable. if you wouldn't mind taking care of it, i would be very grateful.";
			acceptDialogue = "awesome! he should still be around town somewhere. i'm sure you can find him without a problem.";
			expReward = 20;
			break;
		case Significance.inconvenient:
			SpawnNearTown ();
			dialogue = "this " + targetRace.ToString () + " has been causing me some trouble lately. i would appreciate it if you could fix the problem.";
			acceptDialogue = "awesome! he should still be around town somewhere. i'm sure you can find him without a problem.";
			expReward = 45;
			break;
		case Significance.minor_conflict:
			SpawnInCaveNearTown ();
			dialogue = "i've heard there may be a strong " + targetRace.ToString () + " in " + caveName + ". i'm sure that he must have some pretty good treasure. i'll reward you if you defeat him.";
			acceptDialogue = "awesome! i can mark the location of " + caveName + " on your map.";
			expReward = 85;
			break;
		case Significance.major_conflict:
			SpawnInCaveNearTown ();
			dialogue = "there's rumors of a very powerful " + targetRace.ToString () + " somewhere in " + caveName + ". Surely there must be plenty of valuable treasure. i'll reward you handsomely for defeating him.";
			acceptDialogue = "awesome! i can mark the location of " + caveName + " on your map.";
			expReward = 115;
			break;
		}
		description = dialogue;
	}

	void SpawnNearTown () {
		ushort townID = (ushort)questGiver.characterInfo.Get ("townID");
		Town t = Game.current.map.GetTownByID (townID);
		float distance = Random.Range (Mathf.Max (t.sizeInTiles.x, t.sizeInTiles.y) / 2, Mathf.Max (t.sizeInTiles.x, t.sizeInTiles.y) / 2 + maxUnimportantTargetDistanceFromTown);
		Vector2 direction = new Vector2 (Random.Range (-1f, 1f), Random.Range (-1f, 1f)).normalized;
		targetMapID = Game.current.map.id;
		targetSpawnPosition = t.position + t.geometricCenter + direction * distance;
	}

	void SpawnInCaveNearTown () {
		//pick a cave near the town
		ushort townID = (ushort)questGiver.characterInfo.Get ("townID");
		Town t = Game.current.map.GetTownByID (townID);
		targetMapID = t.nearbyCaveIDs [Random.Range (0, t.nearbyCaveIDs.Count)];
		CaveMap cm = (CaveMap)Game.current.map.GetMapByID (targetMapID);
		if (!cm.generated)
			cm.Generate ();
		targetSpawnPosition = cm.GetSignificantSpawnPoint ();
		caveName = cm.name;
	}

	public override void StartQuest () {
		//create the target
		Character c = Character.CreateCharacter (targetRace, targetAlignment, targetControllerType);
		c.mapID = targetMapID;
		c.position = targetSpawnPosition;
		c.id = Game.current.RequestID ();
		targetID = c.id;
		Game.current.AddCharacter (c);
	}

	public override bool AssignToPlayer () {
		//only assign the quest if the player does not have an active quest with a target in the same map
		if (GetObjectiveMapID () != Game.current.map.id) { //this rule does not apply to the world map1
			for (int i = 0; i < Game.current.playerCharacter.quests.Count; i++) {
				if (Game.current.playerCharacter.quests [i].GetObjectiveMapID () == GetObjectiveMapID ())
					return false;
			}
		}
		questGiver.Say (dialogue);
		questGiver.Prompt (prompt, new string[] { "yes", "no" }, new System.Action[] {
			() => {
				questGiver.Say (acceptDialogue);
				base.AssignToPlayer ();
			},
			() => questGiver.Say (declineDialogue)
		});
		return true;
	}

	public override bool IsComplete () {
		return target.IsDead ();
	}

	public override ushort GetObjectiveMapID () {
		return targetMapID;
	}

	public override Vector2 GetObjectiveLocation () {
		if (Game.current.map.id == targetMapID) {
			return Game.current.GetCharacterByID (targetID).position;
		} else {
			return ((CaveMap)Game.current.map.GetMapByID (targetMapID)).caveEntrance;
		}
	}
}