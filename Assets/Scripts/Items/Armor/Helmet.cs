using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Helmet : Armor {
	public Helmet () {
		name = "helmet";
		description = "protects the wearer's head.";
		cost = 0;
		armor = 0;
	}

	[System.Serializable]
	public class RustyHelmet : Helmet {
		public RustyHelmet () : base () {
			name = "rusty helmet";
			cost = 0;
			armor = 0;
		}
	}
	[System.Serializable]
	public class SteelHelmet : Helmet {
		public SteelHelmet () : base () {
			name = "steel helmet";
			cost = 0;
			armor = 0;
		}
	}
	[System.Serializable]
	public class MythrilHelmet : Helmet {
		public MythrilHelmet () : base () {
			name = "mythril helmet";
			cost = 0;
			armor = 0;
		}
	}
}
