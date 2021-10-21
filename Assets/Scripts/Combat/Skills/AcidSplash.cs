using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidSplash : Action {
	public AcidSplash () {
		name = "Acid Splash";
		ap = 5;
		maxfp = 5;
		target = Target.single;
		minRange = 0;
		maxRange = 5;
		description = "You hurl a bubble of acid. One creature or two creatures within 3 tiles of each other take 1-6 damage.";
		//hostile = true;
	}
}