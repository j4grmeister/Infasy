using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunderwave : Action {
	public Thunderwave () {
		name = "Thunderwave";
		ap = 5;
		maxfp = 5;
		target = Target.self;
		minRange = -1;
		maxRange = 5;
		description = "A wave of thunderous force sweeps out from you. Each creature in a 5 tile circle originating from you takes 2-16 damage and is pushed back 3 tiles.";
		//hostile = true;
	}
}
