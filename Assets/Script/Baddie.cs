using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baddie : Actor {

	protected bool isOnStairs, isOnGround;

	protected const float height = 1.0f;

	protected Vector3 prevPosition;

	protected float speed, maxSpeed;


	// Use this for initialization
	void Start () {
		base.OnStart();
		isOnStairs = false;
		isOnGround = false;
		
		speed = 0.0f;
		maxSpeed = 0.05f;
		speed = maxSpeed;
	}

	void FixedUpdate() {
		Vector3 curPos = myRigidbody.position;

		prevPosition = curPos;
		myRigidbody.MovePosition (new Vector3 (curPos.x + speed, curPos.y, curPos.z));
	}
	// Update is called once per frame
	void Update () {
		isOnGround = CheckIfOnGround ();
		CheckPositionChange ();
		CleanIgnoredCollisions ();
	}

	virtual protected void ResolveStairs(Collision col) {
		string facing = CheckStairDirectionInName(col.transform.parent.name);
		bool fullStairs = CheckStairTypeByName (col.transform.parent.name);	

		if (!isOnStairs && isOnGround && !fullStairs) {
			if (facing == "right" && transform.position.x > col.transform.position.x ||
				facing == "left" && transform.position.x < col.transform.position.x) {
				Physics.IgnoreCollision (myCollider, col.collider, true);
				IgnoredCollisions.Add (col.collider);
			} else {
				isOnStairs = true;
			}
		} else {
			isOnStairs = true;
		}
	}

	void CleanIgnoredCollisions ()
	{
		foreach (Collider col in IgnoredCollisions.ToArray()) {
			if (Vector3.Distance (transform.position, col.transform.position) > 1.2f) {
				Physics.IgnoreCollision (myCollider, col.GetComponent<Collider>(), false);
				IgnoredCollisions.Remove (col);
			}
		}
	}

	void OnCollisionEnter (Collision col)
	{
		switch (col.gameObject.tag) {
		case "Stairs":
			ResolveStairs (col);
			break;
		case "Player":
			//col.gameObject.GetComponent<Player> ().isDead = true;
			break;
		case "Enemy":
			Physics.IgnoreCollision (myCollider, col.collider, true);
			IgnoredCollisions.Add (col.collider);			
			break;
		default:
			break;
		}
//		if (col.gameObject.tag == "Stairs") {
//			string facing = CheckStairDirectionInName(col.transform.parent.name);
//			bool fullStairs = CheckStairTypeByName (col.transform.parent.name);	
//
//			if (!isOnStairs && isOnGround && !fullStairs) {
//				if (facing == "right" && transform.position.x > col.transform.position.x ||
//					facing == "left" && transform.position.x < col.transform.position.x) {
//					Physics.IgnoreCollision (myCollider, col.collider, true);
//					IgnoredCollisions.Add (col);
//				} else {
//					isOnStairs = true;
//				}
//			} else {
//				isOnStairs = true;
//			}
//		}
	}

	void OnCollisionExit (Collision col)
	{
		if (col.gameObject.tag == "Stairs") {
			isOnStairs = false;
		}
	}

	protected string CheckStairDirectionInName(string name) {
		string facing = "blank";
		if (name.Contains ("Right")) {
			facing = "right";
		} else if (name.Contains ("Left")) {
			facing = "left";
		}

		return facing;
	}

	protected bool CheckStairTypeByName(string name) {
		bool fullStairs = false;
		if (name.Contains ("Full")) {
			fullStairs = true;				
		}
		return fullStairs;
	}

	bool CheckIfOnGround ()
	{
		bool hitReturn = false;
		RaycastHit ray;
		float radius = 0.49f;
		Vector3 position = new Vector3 (transform.position.x, transform.position.y, transform.position.z);

		int layerMask = 1 << 8;
		layerMask = ~layerMask;
		hitReturn = Physics.SphereCast (position, radius, Vector3.down, out ray, height / 3.0f, layerMask);
		return hitReturn;
	}

	void CheckPositionChange() {
		if(prevPosition.Equals(transform.position)) {
			speed = speed * -1;
		}
	}

}
