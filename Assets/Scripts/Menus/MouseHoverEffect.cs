using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MouseHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	public Color hoverColor; 

	Color defaultColor;
	Text target;

	// Use this for initialization
	void Start () {
		target = gameObject.GetComponent<Text> ();
		defaultColor = target.color;
	}

	// Update is called once per frame
	void Update () {

	}

	public void OnPointerEnter(PointerEventData eventData) {
		target.color = hoverColor;
	}

	public void OnPointerExit(PointerEventData eventData) {
		target.color = defaultColor;
	}
}
