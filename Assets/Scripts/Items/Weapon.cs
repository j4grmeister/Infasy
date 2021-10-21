using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Linq;

[System.Serializable]
public class Weapon : Item {
	//array of all exiting weapon types
	//public static System.Type[] weapons = Assembly.GetAssembly (typeof(Weapon)).GetTypes ().Where (sb => sb.IsSubclassOf (typeof(Weapon))).ToArray ();

	[System.Serializable]
	public class WeaponAction : Action {
		Weapon w;

		public WeaponAction (Weapon w) {
			name = w.name;
			ap = w.ap;
			fp = w.fp;
			//maxfp = -1;
			//target = w.target;
			damageType = w.damageType;
			//minRange = 0;
			//maxRange = 3;
			description = w.description;
			//hostile = true;
		}

		public override float Act (Character actor, Character target) {
			return w.Attack (actor, target);
		}
	}

	//public WeaponType weaponType;
	//public Type type;
	//public int damage;
	//public int FPcost;
	public Action action;

	public int damage;
	public int ap;
	public int fp;
	//public Action.Target target;
	public Action.DamageType damageType;

	public Weapon () {
		name = "weapon";
		description = "desription";
	}

	public virtual float Attack (Character actor, Character target) {
		int d = damage * actor.abilities.strength;
		target.Damage (d, actor.id);
		return Character.damageFlashTime;
	}

	public static Weapon CreateNewWeapon (System.Type t) {
		Weapon w = (Weapon)System.Activator.CreateInstance (t);
		w.action = new WeaponAction (w);
		return w;
	}
		
	/*
	public Weapon(Type t, int d, int cost) {
		//type = Item.Type.weapon;
		type = t;
		//damage = d;
		//FPcost = cost;
	}

	public enum Type {
		sword
	}
	*/

	[System.Serializable]
	public class Fists : Weapon {
		public Fists () : base () {
			name = "fists";
			description = "your bare fists. hope you have a high pain tolerance, because you might have some bloody knuckles when you finish.";
			cost = 0;
			damage = 1;
			ap = 1;
			fp = 0;
			damageType = Action.DamageType.blunt;
		}
	}
}