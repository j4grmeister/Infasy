using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JournalTab : MenuTab {
	public Text description;
	public Text questEntry;
	public GameObject questContainer;
	public GameObject marker;

	Vector2 containerSize;
	Vector2 entrySize;
	List<Text> quests = new List<Text> ();
	int index = 0;

	// Use this for initialization
	void Start () {
		//containerSize = new Vector2 (questContainer.GetComponent<RectTransform> ().rect.width, questContainer.GetComponent<RectTransform> ().rect.height);
		//entrySize = new Vector2 (questEntry.gameObject.GetComponent<RectTransform> ().rect.width, questEntry.gameObject.GetComponent<RectTransform> ().rect.height);
		//containerSize = questContainer.GetComponent<RectTransform> ().sizeDelta;
		//entrySize = questEntry.GetComponent<RectTransform> ().sizeDelta;
	}
	
	// Update is called once per frame
	void Update () {
		if (GameInput.GetKeyDown (GameInput.Bind.up)) {
			index--;
			if (index < 0)
				index = Game.current.playerCharacter.quests.Count - 1;
		}
		if (GameInput.GetKeyDown (GameInput.Bind.down)) {
			index++;
			if (index >= Game.current.playerCharacter.quests.Count)
				index = 0;
		}
		if (GameInput.GetKeyDown (GameInput.Bind.interact)) {

		}
		if (GameInput.GetKeyDown (GameInput.Bind.back)) {
			Game.ui.menu.GoHome ();
		}
		SetInfo ();
	}

	void SetInfo () {
		description.text = Game.current.playerCharacter.quests [index].description;
		marker.GetComponent<RectTransform> ().localPosition = (Vector2)quests [index].GetComponent<RectTransform> ().localPosition + new Vector2 (-10, -entrySize.y / 2);
		/*
		for (int i = 0; i < quests.Count; i++) {
			quests [i].gameObject.GetComponent<RectTransform> ().localPosition = new Vector2 (0, containerSize.y - entrySize.y * i);
		}
		*/
	}

	public override void UpdateTab () {
		containerSize = questContainer.GetComponent<RectTransform> ().sizeDelta;
		entrySize = questEntry.GetComponent<RectTransform> ().sizeDelta;
		for (; quests.Count > 0;) {
			GameObject.Destroy (quests [0].gameObject);
			quests.RemoveAt (0);
		}
		index = 0;
		for (int i = 0; i < Game.current.playerCharacter.quests.Count; i++) {
			Text t = (Text)GameObject.Instantiate (questEntry, questContainer.transform);
			t.gameObject.GetComponent<RectTransform> ().localPosition = new Vector2 (0, containerSize.y / 2 - entrySize.y * i);
			t.text = Game.current.playerCharacter.quests [i].name;
			t.gameObject.SetActive (true);
			quests.Add (t);
		}
	}
}
