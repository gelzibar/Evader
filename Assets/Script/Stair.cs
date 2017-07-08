using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stair : MonoBehaviour {

	public Material defaultMaterial, fadeMaterial;
	private Color defaultColor, fadeColor;


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void Fade() {
		foreach (MeshRenderer child in GetComponentsInChildren<MeshRenderer>()) {
			child.material = fadeMaterial;
		}
	}

	public void Unfade() {
		foreach (MeshRenderer child in GetComponentsInChildren<MeshRenderer>()) {
			child.material = defaultMaterial;
		}
	}
}
