using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

	private Rigidbody myRigidbody;
	private bool isUpPressed, isDownPressed, isOnStairs, isOnGround, isPorting;
	private bool isRightPressed, isLeftPressed;
	private GameObject Depth;
	private bool dropState;
	private float dropTime, dropDistCur, dropDistTotal, dropDistStart;
	private const float dropMax = 0.3f;
	private const float portMax = 0.5f;
	private float portTime;
	private Vector3 portTarget;

	private const float height = 1.95f;

	private List<Collision> IgnoredCollisions;


	void Start ()
	{
		myRigidbody = GetComponent<Rigidbody> ();
		isUpPressed = false;
		isOnStairs = false;
		isOnGround = false;
		isDownPressed = false;
		isRightPressed = false;
		isLeftPressed = false;
		isPorting = false;
		Depth = transform.FindChild ("Depth").gameObject;		
		IgnoredCollisions = new List<Collision> ();
		portTime = 0.0f;

	}

	void FixedUpdate ()
	{
		PlayerInput ();
		Vector3 curPos = myRigidbody.position;
//		float zVal = 0;
		bool depthActivity = false;
		if (isUpPressed || isOnStairs || !isOnGround) {
//			zVal = -1;
			depthActivity = true;
		}
		Depth.SetActive (depthActivity);
		float speed = 0.0f;
		float maxSpeed = 0.05f;
		if (isLeftPressed == true) {
			speed = maxSpeed * -1;
		} else if (isRightPressed == true) {
			speed = maxSpeed;
		}

		myRigidbody.MovePosition (new Vector3 (curPos.x + speed, curPos.y, curPos.z));


	}

	void Update ()
	{
		isOnGround = CheckIfOnGround ();
		if (isDownPressed == true && dropState == false ||
		    isOnStairs == true && dropState == false && !isUpPressed) {
			SetDropThrough (true);
			dropTime = 0.0f;
			dropDistCur = transform.position.y;
			dropDistStart = transform.position.y;
		}
//		} else if (isOnStairs == true) {
//			Physics.IgnoreLayerCollision (8, 9, true);
//		}

		if (dropState == true && (dropDistStart - transform.position.y) > 2.0f ||
		    dropState == true && isOnGround == true && dropTime > dropMax) {
//			dropTime += Time.deltaTime;
//			dropDistTotal = dropDistCur - transform.position.y;
			SetDropThrough (false);
		} else {
			dropTime += Time.deltaTime;
		}

		if (isPorting == true && portTime == 0.0f) {
			myRigidbody.isKinematic = true;
			Depth.SetActive (false);
			transform.position = portTarget;
			myRigidbody.isKinematic = false;
		}

		if (isPorting == true && portTime < portMax) {
			portTime += Time.deltaTime;
		} else {
			isPorting = false;
			portTime = 0.0f;
			portTarget = Vector3.zero;
		}


		CleanIgnoredCollisions ();

		float zDepth = 0.0f;
		if (isOnStairs || !isOnGround) {
			Debug.Log ("Stair Anim");
			zDepth = 1.0f;
			Vector3 curTopPos = transform.FindChild ("Top").position;
			Vector3 curBotPos = transform.FindChild ("Bottom").position;
			transform.FindChild ("Top").position = new Vector3 (curTopPos.x, curTopPos.y, zDepth);
			transform.FindChild ("Bottom").position = new Vector3 (curBotPos.x, curBotPos.y, zDepth);
		} else {
			Vector3 curTopPos = transform.FindChild ("Top").transform.position;
			Vector3 curBotPos = transform.FindChild ("Bottom").transform.position;
			transform.FindChild ("Top").transform.position = new Vector3 (curTopPos.x, curTopPos.y, zDepth);
			transform.FindChild ("Bottom").transform.position = new Vector3 (curBotPos.x, curBotPos.y, zDepth);
		}
			
	}

	void CleanIgnoredCollisions() {
		foreach (Collision col in IgnoredCollisions.ToArray()) {
			if (Vector3.Distance (Depth.transform.position, col.transform.position) > 1.1f) {
				Physics.IgnoreCollision (Depth.GetComponent<Collider> (), col.collider, false);
				IgnoredCollisions.Remove (col);
			}
		}
	}

	void SetDropThrough (bool setting)
	{
		if (setting == true) {
			Physics.IgnoreLayerCollision (8, 9, true);
			dropState = true;
		} else if (setting == false) {
			Physics.IgnoreLayerCollision (8, 9, false);
			dropState = false;
		}
	}

	void PlayerInput ()
	{
		float speed = 0.05f;
		Vector3 curPos = transform.position;
		if (Input.GetKey (KeyCode.A)) {
//			myRigidbody.MovePosition(new Vector3 (curPos.x - speed, curPos.y));
			isLeftPressed = true;
		} else {
			isLeftPressed = false;
		}
		if (Input.GetKey (KeyCode.D)) {
//			myRigidbody.MovePosition(new Vector3 (curPos.x + speed, curPos.y));
			isRightPressed = true;
		} else {
			isRightPressed = false;
		}

		if (Input.GetKey (KeyCode.W) && !isOnStairs) {
			isUpPressed = true;
		} else {
			isUpPressed = false;
		}
		if (Input.GetKey (KeyCode.S)) {
			isDownPressed = true;
		} else {
			isDownPressed = false;
		}

		if (Input.GetKey (KeyCode.E)) {
			Vector3 curBotPos = transform.FindChild ("Bottom").localPosition;
			transform.FindChild ("Bottom").localPosition = new Vector3 (curBotPos.x, curBotPos.y, 1.0f);
		}

	}

	//	void OnCollisionStay(Collision col) {
	//		if (col.gameObject.tag == "Stairs") {
	//			isOnStairs = true;
	//		}
	//	}

	void OnCollisionEnter (Collision col)
	{
		if (col.gameObject.tag == "Stairs") {
			string facing = "blank";
			if (col.transform.parent.name.Contains ("Right")) {
				Debug.Log ("Contains Right");
				facing = "right";
			} else if (col.transform.parent.name.Contains ("Left")) {
				Debug.Log ("Contains Left");
				facing = "left";
			}
			if (Depth.activeSelf == true) {
				if (facing == "right" && Depth.transform.position.x > col.transform.position.x && !isOnStairs && isOnGround) {
					Physics.IgnoreCollision (Depth.GetComponent<Collider> (), col.collider, true);
					IgnoredCollisions.Add (col);
				} else if (facing == "left" && Depth.transform.position.x < col.transform.position.x && !isOnStairs && isOnGround) {
					Physics.IgnoreCollision (Depth.GetComponent<Collider> (), col.collider, true);
					IgnoredCollisions.Add (col);
				} else {
					isOnStairs = true;
				}
						
			}
		}
	}

	void OnCollisionStay (Collision col) {
		if (col.gameObject.tag == "Stairs") {
			if (Depth.activeSelf == true) {
					isOnStairs = true;
			}
		}
	}

	void OnCollisionExit (Collision col)
	{
		if (col.gameObject.tag == "Stairs") {
			isOnStairs = false;
		}
	}

	void OnTriggerEnter (Collider col) {
		if (col.transform.name == "Trigger" && col.transform.parent.tag == "Stairwell" && isUpPressed && !isPorting) {
			GameObject target = col.transform.parent.GetComponent<Stairwell> ().exit;
//			myRigidbody.isKinematic = true;
//			Depth.SetActive (false);
//			transform.position = target.transform.FindChild ("Spawn").position;
//			myRigidbody.isKinematic = false;
			isPorting = true;
			portTarget = target.transform.FindChild ("Spawn").position;
		}
	}

	void StairEntryCheck ()
	{

		// Concern: Depth activation doesn't care about what direction the character is coming from. If coming from the  "back" of the stairs
		// then weird collision things may occur. Additionally, when those don't occur, the character "hops" over the first stair-- not game breaking, but distracting

		// Conditions:
		// If depth sensor is on
		// And depth sensor is colliding with a stair
		// Get Depth Sensor Position and Stair Position
		if (Depth.activeSelf == true) {
			
		}
		
	}

	bool CheckIfOnGround ()
	{
		bool hitReturn = false;
		bool distanceToObj = false;
		RaycastHit ray;
		Vector3 position = new Vector3 (transform.position.x, transform.position.y - height / 4.0f, transform.position.z);
//		Vector3 position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		Vector3 boxSize = new Vector3 (transform.localScale.x / 2.0f, transform.localScale.y / 2.0f, transform.localScale.z / 2.0f);

		int layerMask = 1 << 8;
		layerMask = ~layerMask;
		Debug.DrawRay (position, Vector3.down, Color.blue);
//		if (Physics.BoxCast(position, boxSize, Vector3.down, transform.rotation, 0.1f, layerMask))  {
////			SphereCast(position, 1.0f, Vector3.down, out ray, 0.1f, layerMask)) {
//			falling = false;
//		}

//		hitReturn = Physics.BoxCast (position, boxSize, transform.up * -1.0f, transform.rotation, 1.0f, layerMask);
		hitReturn = Physics.SphereCast (position, 0.45f, Vector3.down, out ray, 0.1f, layerMask);
//		hitReturn = Physics.Raycast(position, Vector3.down, 0.25f, layerMask);
		return hitReturn;
	}

}

