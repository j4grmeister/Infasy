using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Spear : Weapon {
	public Spear () {
		name = "spear";
		description = "a standard spear.";
		cost = 0;
		ap = 3;
		fp = 0;
		damageType = Action.DamageType.pierce;
	}

	[System.Serializable]
	public class RustySpear : Spear {
		public RustySpear () : base () {
			name = "rusty spear";
			description = "a rusty spear. it appears to be fairly worn and is slightly chipped at the tip. it should hold out long enough to get a better one.";
			cost = 20;
		}
	}
}