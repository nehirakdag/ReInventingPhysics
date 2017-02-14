using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Shootable : MonoBehaviour {

	public float radius;

	public float speed;
	public float elevationAngle;

	public float notMovingSince;

	public Vector2 windForce;

	public float bounciness;

	public Vector2 currentVelocity;
	public bool moving = true;
	private bool movingAtY = true;

	// Use this for initialization
	public virtual void Start () {
		currentVelocity = new Vector2(speed * Mathf.Cos(elevationAngle * Mathf.Deg2Rad), speed * Mathf.Sin(elevationAngle * Mathf.Deg2Rad));
		transform.Rotate (new Vector3(0.0f, 0.0f, -1 * elevationAngle));
	}
	
	// Update is called once per frame
	public virtual void Update () {
		Vector2 newVelocity = currentVelocity;

		if (movingAtY) {
			Vector2 drag = newVelocity.normalized * -0.5f * Movement.AIR_DENSITY * Movement.CrossSectionalArea (radius) * newVelocity.magnitude * newVelocity.magnitude;
			newVelocity += (drag + Movement.GRAVITY_VECTOR + windForce) * Time.deltaTime;

			if (transform.position.y <= -3.5f) {
				movingAtY = false;
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
