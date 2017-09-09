using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidObject : MonoBehaviour {
	public Material defaultMaterial, fadeMaterial;

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