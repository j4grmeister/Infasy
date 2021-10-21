using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Gauntlets : Armor {
	public Gauntlets () {
		name = "gauntlets";
		description = "protects the wearer's hands.";
		cost = 0;
		armor = 0;
	}

	[System.Serializable]
	public class RustyGauntlets : Gauntlets {
		public RustyGauntlets () : base () {
			name = "rusty gauntlets";
			cost = 0;
			armor = 0;
		}
	}
	[System.Serializable]
	public class SteelGauntlets : Gauntlets {
		public SteelGauntlets () : base () {
			name = "steel gauntlets";
			cost = 0;
			armor = 0;
		}
	}
	[System.Serializable]
	public class MythrilGauntlets : Gauntlets {
		public MythrilGauntlets () : base () {
			name = "mythril gauntlets";
			cost = 0;
			armor = 0;
		}
	}
}