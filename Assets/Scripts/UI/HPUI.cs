using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPUI : MonoBehaviour {
	public Text text;
	Slider slider;

	float width;

	// Use this for initialization
	void Start () {
		slider = gameObject.GetComponent<Slider> ();
	}
	
	// Update is called once per frame
	void Update () {
		//update bar
		slider.value = (float)Game.current.playerCharacter.HP () / (float)Game.current.playerCharacter.MaxHP ();

		//update text
		text.text = Game.current.playerCharacter.HP ().ToString () + "/" + Game.current.playerCharacter.MaxHP ().ToString();
	}
}
