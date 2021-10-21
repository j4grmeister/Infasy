using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;

public class Quest {
	public static System.Type[] questTypes = Assembly.GetAssembly (typeof(Quest)).GetTypes ().Where (sb => sb.IsSubclassOf (typeof(Quest))).ToArray ();

	//I now realize that I can't use "short" or "long" as enum values... since they are already type names
	public enum Length {
		quick, //1-2 days
		medium, //3-6 days
		lengthy, //1-2 weeks
		Count
	}
	public enum Significance {
		unimportant,
		inconvenient,
		minor_conflict,
		major_conflict,
		Count
	}
	public enum SocialClass {
		peasant,
		middle_class,
		nobility,
		royalty,
		Count
	}
	//basically a time limit
	//might not apply to certain quest types
	public enum Urgency {
		take_your_time, //no time limit
		no_hurry, //1-2 weeks
		sometime_soon, //4-6 days
		need_it_now, //2-3 days
		no_time_to_spare, //1 day
		Count
	}

	public ushort id = 0;
	public ushort questGiverID = 0;
	public Character questGiver {
		get {
			return Game.current.GetCharacterByID (questGiverID);
		}
	}
	public string name = "quest";
	public string description = "";
	public Length length;
	public Significance significance;
	public SocialClass socialClass;
	public Urgency urgency;
	public int expReward = 0;

	public int timeLimit; //in days; -1 indicates no time limit

	//separate screens with a line break ('\n') for all of the following:
	protected string dialogue;
	protected string prompt = "are you up for the task?";
	protected string acceptDialogue;
	protected string declineDialogue = "oh, well that's too bad.";

	public Quest (ushort giver) {
		questGiverID = giver;
		name = Game.current.GetCharacterByID (giver).name;
	}

	public void RandomizeParameters () {
		length = (Length)Random.Range (0, (int)Length.Count);
		significance = (Significance)Random.Range (0, (int)Significance.Count);
		socialClass = (SocialClass)Random.Range (0, (int)SocialClass.Count);
		urgency = (Urgency)Random.Range (0, (int)Urgency.Count);
	}

	public virtual void Generate () {
		id = Game.current.RequestID ();
		//generate time limit
		int min = 0;
		int max = 0;
		switch (urgency) {
		case Urgency.take_your_time:
			min = -1;
			max = -1;
			break;
		case Urgency.no_hurry:
			min = 7;
			max = 14;
			break;
		case Urgency.sometime_soon:
			min = 4;
			max = 6;
			break;
		case Urgency.need_it_now:
			min = 2;
			max = 3;
			break;
		case Urgency.no_time_to_spare:
			min = 1;
			max = 1;
			break;
		}
		timeLimit = Random.Range (min, max + 1);
	}

	public virtual void StartQuest () {

	}

	//assigns this quest to the player (unless the player declines)
	//returns true if the quest was actually assigned (some quests require certain parameters to be met before they are assigned)
	public virtual bool AssignToPlayer () {
		StartQuest ();
		Game.current.playerCharacter.quests.Add (this);
		Game.current.playerCharacter.currentQuestID = id;
		return true;
	}

	public virtual bool IsComplete () {
		return true;
	}

	public void Complete () {
		Game.current.playerCharacter.quests.Remove (this);
		questGiver.quests.Remove (this);
		Game.current.playerCharacter.completedQuests.Add (this);
		Game.current.playerCharacter.GainExp (expReward);
	}

	public virtual ushort GetObjectiveMapID () {
		return 0;
	}

	public virtual Vector2 GetObjectiveLocation () {
		return Vector2.zero;
	}
}