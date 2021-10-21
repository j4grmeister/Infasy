using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodeCanvas : MonoBehaviour {
	List<Text> fadingText = new List<Text> ();
	List<Vector2> fadingTextTar = new List<Vector2> ();
	List<float> fadingTextTime = new List<float> ();
	List<float> fadingTextMaxTime = new List<float> ();

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		//change the opacity of the fading text and change their positions
		for (int i = 0; i < fadingText.Count; i++) {
			float alpha = fadingTextTime [i] / fadingTextMaxTime [i];
			fadingText [i].color = new Color (1, 1, 1, alpha);
			Vector3 pos = GameObject.FindObjectOfType<Camera> ().WorldToScreenPoint (Game.GameToUnity (fadingTextTar [i]));
			fadingText [i].rectTransform.position = pos;
		}
		//reduce the fading text times and remove any
		for (int i = 0; i < fadingTextTime.Count; i++) {
			float t = fadingTextTime [i];
			fadingTextTime.RemoveAt (i);
			t -= GameTime.deltaWorldTime;
			if (t > 0) {
				fadingTextTime.Insert (i, t);
			} else {
				GameObject.Destroy (fadingText [i].gameObject);
				fadingText.RemoveAt (i);
				fadingTextTar.RemoveAt (i);
				fadingTextMaxTime.RemoveAt (i);
			}
		}
	}
		
	public void AddFadingTextToWorld (string str, Vector2 position, float time) {
		Text txt = Game.CreateObject<Text> (gameObject);
		txt.text = str;
		txt.font = GameResources.defaultFont;
		txt.fontSize = 30;
		fadingText.Add (txt);
		fadingTextTar.Add (position);
		fadingTextTime.Add (time);
		fadingTextMaxTime.Add (time);
	}

	public void AddFadingTextToWorld (string str, int size, Vector2 position, float time) {
		Text txt = Game.CreateObject<Text> (gameObject);
		txt.text = str;
		txt.font = GameResources.defaultFont;
		txt.fontSize = size;
		fadingText.Add (txt);
		fadingTextTar.Add (position);
		fadingTextTime.Add (time);
		fadingTextMaxTime.Add (time);
	}
}
