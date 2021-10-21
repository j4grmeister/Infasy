using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

	public float transitionTime;

	//public bool paused = false;
	bool update = true;

	Character target;
	//Character lastTarget;

	// Use this for initialization
	void Start () {
		Game.camera = this;
		target = Game.current.playerCharacter;
		//lastTarget = target;
	}

	// Update is called once per frame
	void Update () {
		if (update) {
			gameObject.transform.position = Game.GameToUnity (target.position);
			gameObject.transform.Translate (new Vector3 (0, 0, -10));
		}
		/*
		if (!paused) {
			if (!Game.current.playerCharacter.InCombat ()) {
				target = Game.current.playerCharacter;
			} else {
				Battle battle = Game.current.playerCharacter.GetBattle ();
				target = battle.CurrentCombatant ();
			}
		}
		if (target == lastTarget) {
			gameObject.transform.position = Game.GameToUnity (target.position);
			gameObject.transform.Translate (new Vector3 (0, 0, -10));
		} else {
			//transition to the new target
			Vector2 dif = target.position - lastTarget.position;
			Vector2 perSec = dif / transitionTime;
			System.Func<bool> transitionCamera = () => {
				if (Vector2.Distance (target.position, Game.UnityToGame (gameObject.transform.position)) <= GameTime.deltaWorldTime * perSec.magnitude) {
					paused = false;
					lastTarget = target;
					return true;
				}
				gameObject.transform.Translate (Game.GameToUnity (perSec) * GameTime.deltaWorldTime);
				return false;
			};
			ActionSchedule.AddRepeatedAction (transitionCamera);
			paused = true;
		}
		*/
	}

	public void SetTarget (Character c) {
		/*
		if (c != target) {
			target = c;
			//transition to the new target
			Vector2 dif = target.position - lastTarget.position;
			Vector2 perSec = dif / transitionTime;
			float time = 0;
			System.Func<bool> transitionCamera = () => {
				time += GameTime.deltaWorldTime;
				///*
				if (Vector2.Distance (target.position, Game.UnityToGame (gameObject.transform.position)) <= GameTime.deltaWorldTime * perSec.magnitude) {
					update = true;
					return true;
				}
				//
				gameObject.transform.Translate (Game.GameToUnity (perSec) * GameTime.deltaWorldTime);
				if (time >= transitionTime) {
					update = true;
					return true;
				}
				update = false;
				return false;
			};
			lastTarget = target;
			ActionSchedule.AddRepeatedAction (transitionCamera);
		}
		*/
		target = c;
	}
}