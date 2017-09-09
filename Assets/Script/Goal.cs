using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {

	public bool isLocked { get; set; }
	int toll { get; set;}

	Player myPlayer;

	// Use this for initialization
	void Start () {
		OnStart();
	}
	
	// Update is called once per frame
	void Update () {
		OnUpdate();
	}

	void OnStart() {
		myPlayer = GameObject.Find("Player").GetComponent<Player>();

		Initialize();
	}

	void OnUpdate() {
		if(myPlayer.coinsCollected >= toll) {
			isLocked = false;
		}

		if(!isLocked) {
			transform.Find("Bot Door").gameObject.SetActive(false);
			transform.Find("Top Door").gameObject.SetActive(false);
		}else {
			transform.Find("Bot Door").gameObject.SetActive(true);
			transform.Find("Top Door").gameObject.SetActive(true);
		}
		
	}

	void Initialize() {
		isLocked = true;
		toll = 3;
	}
}
