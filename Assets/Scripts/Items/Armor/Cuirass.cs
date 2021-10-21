using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cuirass : Armor {
	public Cuirass () {
		name = "cuirass";
		description = "protects the wearer's torso.";
		cost = 0;
		armor = 0;
	}

	[System.Serializable]
	public class RustyCuirass : Cuirass {
		public RustyCuirass () : base () {
			name = "rusty cuirass";
			cost = 0;
			armor = 0;
		}
	}
	[System.Serializable]
	public class SteelCuirass : Cuirass {
		public SteelCuirass () : base () {
			name = "steel cuirass";
			cost = 0;
			armor = 0;
		}
	}
	[System.Serializable]
	public class MythrilCuirass : Cuirass {
		public MythrilCuirass () : base () {
			name = "mythril cuirass";
			cost = 0;
			armor = 0;
		}
	}
}