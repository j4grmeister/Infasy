using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class APUI : MonoBehaviour {
	public Text text;
	Slider slider;
	float width;

	List<Action> actions = new List<Action> ();

	public int usedAP {
		get {
			int ap = 0;
			for (int i = 0; i < actions.Count; i++) {
				ap += actions [i].ap;
			}
			return ap;
		}
	}
	public int remainingAP {
		get {
			return Game.current.playerCharacter.ap - usedAP;
		}
	}

	// Use this for initialization
	void Start () {
		slider = gameObject.GetComponent<Slider> ();
	}

	// Update is called once per frame
	void Update () {
		//update bar
		slider.value = (float)remainingAP / (float)Game.current.playerCharacter.ap;

		//update text
		text.text = remainingAP.ToString () + "/" + Game.current.playerCharacter.ap.ToString();
	}

	public void AddAction (Action a) {
		if (a.ap <= remainingAP)
			actions.Add (a);
	}
	//removes the last action
	public void RemoveAction () {
		actions.RemoveAt (actions.Count - 1);
	}

	public void Reset () {
		actions.Clear ();
	}
}
