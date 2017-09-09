using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public Text coinCount;
	public GameObject winText;
	public Player myPlayer;

	public bool celebrate;

	void Start () {
		OnStart();
	}
	
	void Update () {
		OnUpdate();
	}

	void OnStart() {
		myPlayer = GameObject.Find("Player").GetComponent<Player>();
		Initialize();
	}

	void OnUpdate() {
		coinCount.text = "Coins: " + myPlayer.coinsCollected;
		if(celebrate) {
			winText.SetActive(true);
		}
	}

	void Initialize() {
		celebrate = false;
	}
}
