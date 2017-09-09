using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {

	public UIManager myUIManager;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (transform.up, 2.0f);
	}

	void OnTriggerEnter(Collider col) {
		if (col.tag == "Player") {
			col.gameObject.GetComponent<Player>().coinsCollected++;
			Destroy (this.gameObject);
		}
	}
}
