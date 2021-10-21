using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sword : Weapon {
	/*
	[System.Serializable]
	public class ItemAction : Action {
		public ItemAction () {
			name = "Sword";
			ap = 3;
			maxfp = -1;
			target = Target.single;
			minRange = 0;
			maxRange = 3;
			description = "Attack a single target with a one handed sword. Deals 1-6 damage.";
			//hostile = true;
		}

		public override float Act (Character actor, Character target) {
			//target.Damage (Game.RollDice (6));
			target.Damage (damage);
			return Character.damageFlashTime;
		}
	};
	*/

	public Sword () {
		name = "sword";
		description = "a single handed sword. deals 5 damage.";
		cost = 10;
		damage = 5;
		ap = 3;
		fp = 0;
		damageType = Action.DamageType.slash;
	}
	/*
	public override float Attack (Character actor, Character target) {
		//target.Damage (Game.RollDice (6));
		target.Damage (damage);
		return Character.damageFlashTime;
	}
	*/

	//All subclasses go here. They are all short, it doesn't make sense to make a new file for each
	[System.Serializable]
	public class WoodenSword : Sword {
		public WoodenSword () : base () {
			name = "wooden sword";
			description = "a single handed sword made out of wood. more of a training weapon than anything. you figure it wouldn't do much more than give a splinter to a stronger enemy.";
			cost = 30;
			damage = 2; //2
		}
	}
	[System.Serializable]
	public class RustySword : Sword {
		public RustySword () : base () {
			name = "rusty sword";
			description = "a single handed sword. it's pretty rusty, but it should be fine for fighting weaker enemies.";
			cost = 100;
			damage = 8; //10
		}
	}
	[System.Serializable]
	public class BronzeSword : Sword {
		public BronzeSword () : base () {
			name = "bronze sword";
			description = "a single handed sword. it gives off a faint glow and feels light in your hand.";
			cost = 1850;
			damage = 23; //30
		}
	}
	[System.Serializable]
	public class IronSword : Sword {
		public IronSword () : base () {
			name = "iron sword";
			description = "a single handed sword. it appears to be sturdy and well crafted.";
			cost = 3350;
			damage = 39; //50
		}
	}
	[System.Serializable]
	public class SteelSword : Sword {
		public SteelSword () : base () {
			name = "steel sword";
			description = "a single handed steel sword. the surface of the blade is nice to the touch";
			cost = 590000;
			damage = 57; //70
		}
	}
	[System.Serializable]
	public class MythrilSword : Sword {
		public MythrilSword () : base () {
			name = "mythril sword";
			description = "a single handed mythril sword. it is polished beautifully, to the point where one can see their reflection in the blade.";
			cost = 10500000;
			damage = 76; //90
		}
	}

	//THE ADVENTURE ZONE EASTER EGG!!!!!
	/*
	 * Dear Future Self,
	 * The Flaming Poisoning Raging Sword of Doom is one super sweet easter egg, but it is definitely
	 * too overpowered. Please nerf.
	 * -Nick Greene (December 7, 2017)
	 */
	[System.Serializable]
	public class FlamingPoisoningRagingSwordOfDoom : RustySword {
		public FlamingPoisoningRagingSwordOfDoom () : base () {
			name = "flaming poisoning raging sword of doom";
			description = "it sounds pretty damn cool. who wouldn't want it?";
			cost = 60000;
			damage = 200;
		}
	}
}