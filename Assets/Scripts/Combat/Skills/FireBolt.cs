using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBolt : Action {
	public FireBolt () {
		name = "Fire Bolt";
		ap = 5;
		fp = 5;
		target = Target.single;
		minRange = 0;
		maxRange = 5;
		description = "Hurls a flaming bolt at a single target, dealing between 1 and 10 damage.";
		//hostile = true;
	}

	public override float Act (Character actor, Character target) {
		int dmgRoll = Game.RollDice (10);
		MovingTimer mt = GameObject.Instantiate (Resources.Load<GameObject> ("Prefabs/Magic Orb")).GetComponent<MovingTimer> ();
		Vector2 v = target.position - actor.position;
		mt.gameObject.transform.position = actor.gameObject.transform.position;
		mt.velocity = v.normalized;
		mt.time = v.magnitude;
		GameTime.Freeze (v.magnitude);
		//create a lambda to later damage the target
		//this is necessary in order for the target to be damaged after the fire bolt reaches it
		//otherwise it will take damage when the actor performs the skill
		//and to be honest, that would be kind of stupid
		System.Action damageTarget = () => { target.Damage (dmgRoll, actor.id); };
		ActionSchedule.ScheduleAction (damageTarget, v.magnitude);
		return v.magnitude + Character.damageFlashTime;
	}
}