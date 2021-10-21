using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueMenu : MonoBehaviour {
	public struct Prompt {
		public System.Action action;
		public DialoguePrompt prompt;
	}

	public struct QueueEntry {
		public string characterName;
		public string text;
		public string[] response;
		public System.Action[] action;
	}

	public bool inProgress = false;
	public float timeBetweenTypedCharacters;
	public Text dialogueText;
	public Text character;
	public GameObject marker;
	public GameObject promptContainer;
	public DialoguePrompt promptObject;
	public float promptSpacing;
	public Vector2 markerOffset;
	List<Prompt> prompts = new List<Prompt> ();
	List<QueueEntry> queue = new List<QueueEntry> ();
	string text {
		get {
			if (queue.Count == 0)
				return "";
			return queue [0].text;
		}
	}
	float timeSinceLastTypedCharacter = 0f;
	int index = 0;
	bool prompting = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		AnimateTyping ();
		if (GameInput.GetKeyDown (GameInput.Bind.back)) {
			Close ();
		}
		if (prompting && dialogueText.text == text) {
			marker.GetComponent<RectTransform> ().localPosition = prompts [index].prompt.gameObject.GetComponent<RectTransform> ().localPosition + (Vector3)markerOffset;
			if (GameInput.GetKeyDown (GameInput.Bind.down)) {
				index++;
				if (index >= prompts.Count)
					index = 0;
			}
			if (GameInput.GetKeyDown (GameInput.Bind.up)) {
				index--;
				if (index < 0)
					index = prompts.Count - 1;
			}
			if (GameInput.GetKeyDown (GameInput.Bind.interact)) {
				prompts [index].action ();
				Next ();
			}
		} else {
			if (GameInput.GetKeyDown (GameInput.Bind.interact)) {
				if (dialogueText.text == text) {
					Next ();
				} else {
					Skip ();
				}
			}
		}
	}

	void AnimateTyping () {
		if (timeSinceLastTypedCharacter < timeBetweenTypedCharacters) {
			//Debug.Log ("wait to type");
			timeSinceLastTypedCharacter += Time.deltaTime;
		}
		if (!text.StartsWith (dialogueText.text)) {
			//Debug.Log ("dialogue changed");
			dialogueText.text = "";
		}
		if (dialogueText.text.Length < text.Length) {
			if (timeSinceLastTypedCharacter >= timeBetweenTypedCharacters) {
				//Debug.Log ("typed character: " + text [dialogueText.text.Length]);
				dialogueText.text += text [dialogueText.text.Length];
				timeSinceLastTypedCharacter = 0f;
			}
			if (dialogueText.text.Length == text.Length && prompts.Count > 0) {
				ShowPrompts ();
			}
		}
	}

	void Skip () {
		dialogueText.text = text;
		if (prompts.Count > 0)
			ShowPrompts ();
	}

	void Next () {
		ClearPrompts ();
		character.text = "";
		dialogueText.text = "";
		queue.RemoveAt (0);
		if (queue.Count > 0) {
			LoadThis ();
		} else {
			Close ();
		}
	}

	void LoadThis () {
		character.text = queue [0].characterName.ToLower () + ":";
		if (queue [0].response != null) {
			for (int i = 0; i < queue [0].response.Length; i++) {
				prompting = true;
				Prompt p;
				DialoguePrompt dp = GameObject.Instantiate (promptObject, promptContainer.transform);
				dp.SetText (queue [0].response [i]);
				dp.gameObject.GetComponent<RectTransform> ().localPosition = new Vector2 (0, (promptObject.gameObject.GetComponent<RectTransform> ().rect.height + promptSpacing) * (queue [0].response.Length - 1) - (promptObject.gameObject.GetComponent<RectTransform> ().rect.height + promptSpacing) * i);
				p.prompt = dp;
				p.action = queue [0].action [i];
				prompts.Add (p);
			}
		}
	}
		
	public void Open () {
		inProgress = true;
		//ClearText ();
		gameObject.SetActive (true);
		GameTime.paused = true;
	}

	public void Close () {
		Reset ();
		inProgress = false;
		gameObject.SetActive (false);
		GameTime.paused = false;
	}

	void Reset () {
		character.text = "";
		dialogueText.text = "";
		queue.Clear ();
		ClearPrompts ();
	}

	/*
	public void ClearText () {
		text = "";
		dialogueText.text = "";
	}
	*/

	public void ClearPrompts () {
		prompting = false;
		for (;prompts.Count > 0;) {
			GameObject.Destroy (prompts [0].prompt.gameObject);
			prompts.RemoveAt (0);
		}
		promptContainer.SetActive (false);
		marker.SetActive (false);
	}

	//SHOULD NEVER BE USED BY THE PLAYER!!!
	/*
	public void Say (Character c, string dialogue) {
		text = c.name.ToLower () + ": " + dialogue.ToLower ();
		dialogueText.text = c.name.ToLower () + ": ";
		marker.SetActive (false);
	}
	public void PromptPlayer (Character c, string prompt, string[] response, System.Action[] onSelect) {
		ClearPrompts ();
		for (int i = 0; i < response.Length; i++) {
			Prompt p;
			DialoguePrompt dp = GameObject.Instantiate (promptObject, promptContainer.transform);
			dp.SetText (response [i]);
			dp.gameObject.GetComponent<RectTransform> ().localPosition = new Vector2 (0, (promptObject.gameObject.GetComponent<RectTransform> ().rect.height + promptSpacing) * i);
			p.prompt = dp;
			p.action = onSelect [i];
			prompts.Add (p);
		}
		promptContainer.SetActive (false);
		prompting = false;
		Say (c, prompt);
	}
	*/
	public void AddDialogue (Character c, string dialogue) {
		if (!gameObject.activeSelf)
			Open ();
		QueueEntry qe = new QueueEntry ();
		if (c != null) {
			qe.characterName = c.name;
		} else {
			qe.characterName = "";
		}
		qe.text = dialogue;
		queue.Add (qe);
		if (queue.Count == 1) {
			LoadThis ();
		}
	}
	public void AddPrompt (Character c, string prompt, string [] response, System.Action[] onSelect) {
		if (!gameObject.activeSelf)
			Open ();
		QueueEntry qe;
		if (c != null) {
			qe.characterName = c.name;
		} else {
			qe.characterName = "";
		}
		qe.text = prompt;
		qe.response = response;
		qe.action = onSelect;
		queue.Add (qe);
		if (queue.Count == 1) {
			LoadThis ();
		}
	}

	void ShowPrompts () {
		promptContainer.SetActive (true);
		index = 0;
		prompting = true;
		marker.SetActive (true);
	}
}