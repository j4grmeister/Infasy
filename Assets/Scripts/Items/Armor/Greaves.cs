using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Greaves : Armor {
	public Greaves () {
		name = "greaves";
		description = "protects the wearer's legs.";
		cost = 0;
		armor = 0;
	}

	[System.Serializable]
	public class RustyGreaves : Greaves {
		public RustyGreaves () : base () {
			name = "rusty greaves";
			cost = 0;
			armor = 0;
		}
	}
	[System.Serializable]
	public class SteelGreaves : Greaves {
		public SteelGreaves () : base () {
			name = "steel greaves";
			cost = 0;
			armor = 0;
		}
	}
	[System.Serializable]
	public class MythrilGreaves : Greaves {
		public MythrilGreaves () : base () {
			name = "mythril greaves";
			cost = 0;
			armor = 0;
		}
	}
}
