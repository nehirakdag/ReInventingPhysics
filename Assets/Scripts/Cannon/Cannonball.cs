using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : Shootable {

	// Set bounciness to 70%
	private float bouncinessOfCannonball = 0.70f;

	public override void Start() {
		bounciness = bouncinessOfCannonball;
		currentVelocity = new Vector2(speed * Mathf.Cos(elevationAngle * Mathf.Deg2Rad), speed * Mathf.Sin(elevationAngle * Mathf.Deg2Rad));
		transform.Rotate (new Vector3(0.0f, 0.0f, -1 * elevationAngle));
	}

	// Update is called once per frame
	public override void Update () {
		Vector2 newVelocity = currentVelocity;

		// At each frame, calculate the drag force depending on the current initial velocity,
		// add the gravity and the wind forces to find the acceleration, 
		// multiply the final force by time to find the change in velocity
		if (movingAtY) {
			Vector2 drag = newVelocity.normalized * -0.5f * Movement.AIR_DENSITY * Movement.CrossSectionalArea (radius) * newVelocity.magnitude * newVelocity.magnitude;
			newVelocity += (drag + Movement.GRAVITY_VECTOR + windForce) * Time.deltaTime;

			// Stop moving if you reach the ground plane
			if (transform.position.y <= -3.5f || ((newVelocity * Time.deltaTime).magnitude <= 0.005f && newVelocity.x <= 0.01f && 
				transform.position.x > -1.8f && transform.position.x < 1.8f && transform.position.y < 1.7f && transform.position.y > 0.9f)) {
				movingAtY = false;
			}
			if (transform.position.y <= -3.5f) {
				notMovingSince = 2.0f;
			}

		} else {
			newVelocity.x = 0;
			newVelocity.y = 0;
			moving = false;
		}

		transform.Translate (new Vector3(newVelocity.x * Time.deltaTime, newVelocity.y * Time.deltaTime, 0.0f));
		currentVelocity = newVelocity;

		if (!moving) {
			notMovingSince += Time.deltaTime;
		}

	}
}
