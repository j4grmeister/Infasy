using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Action {
	public enum Target {
		single,
		self
	};

	public enum DamageType {
		blunt,
		slash,
		pierce
	}

	public static Vector2 labelOffset = new Vector2 (.25f, -.5f);

	public string name;
	public int ap;
	//this should be equal to -1 if this action does not require FP
	public int fp;
	public int maxfp; //remove later (I'm a lazy bastard)
	public Target target; //type of valid target
	public string description;
	public DamageType damageType = DamageType.blunt;
	//public bool hostile;
	//the range of this action; both equal to -1 if range does not apply
	//different combinations of ranges and targets may mean different things:
	/*
	 * minRange == -1 and maxRange == -1: range does not apply
	 * minRange == -1 and target == self: does not damage self, but is rather centered on self and damages surrounding entities
	 */
	//remove these later on (again, as stated above, I'm a lazy son of a biscuit)
	public float minRange;
	public float maxRange;

	public Action () {
		name = "Action";
		ap = 0;
		fp = 0;
		target = Target.single;
		//selectTarget = selectSingle;
		minRange = -1;
		maxRange = -1;
		description = "Description";
		//hostile = false;
	}

	//returns true if the two characters are within range for one of them to perform this action
	public bool InRange (Character c0, Character c1) {
		return ((minRange == -1 && maxRange == -1) || (Vector2.Distance (c0.position, c1.position) >= minRange && Vector2.Distance (c0.position, c1.position) <= maxRange));
	}

	//returns true if the character meets all the requirements of performing this action
	public bool CanDo (Character actor) {
		bool apr = (actor.ap >= ap);
		bool fpr = (actor.fp >= fp);
		//bool fpr = (maxfp == -1) ? true : (actor.FP (name) > 0);
		return (apr && fpr);
	}

	//public void DoAction (Character actor, Character target) {
	//returns the amount of time (in seconds) this action will take to perform
	public float DoAction (Character actor, Character target) {
		actor.UseAP (ap);
		/*
		if (maxfp != -1) {
			actor.UseFP (name);
		}
		*/

		//display text by the actor
		Game.codeCanvas.AddFadingTextToWorld (name, 18, actor.position + labelOffset, 2);

		return Act (actor, target);
	}

	//returns the amount of time (in seconds) this action will take to perform
	public virtual float Act (Character actor, Character target) {
		return 0f;
	}
}