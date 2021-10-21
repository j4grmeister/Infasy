using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemEntry : MonoBehaviour {
	public Text name;

	protected Item item;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public virtual void SetItem (Item i) {
		item = i;
		name.text = item.name;
	}

	public Item GetItem () {
		return item;
	}
}
