using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crawler : Baddie {
	override protected void ResolveStairs(Collision col) {
		bool fullStairs = CheckStairTypeByName (col.transform.parent.name);	

		if (!fullStairs) {
			Physics.IgnoreCollision (myCollider, col.collider, true);
			IgnoredCollisions.Add (col.collider);
		} else {
			isOnStairs = true;
		}
	}
}
