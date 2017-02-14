using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : Shootable {

	private float bouncinessOfCannonball = 0.90f;

	public override void Start() {
		bounciness = bouncinessOfCannonball;
		currentVelocity = new Vector2(speed * Mathf.Cos(elevationAngle * Mathf.Deg2Rad), speed * Mathf.Sin(elevationAngle * Mathf.Deg2Rad));
		transform.Rotate (new Vector3(0.0f, 0.0f, -1 * elevationAngle));
	}
}
