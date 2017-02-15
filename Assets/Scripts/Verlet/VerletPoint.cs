using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerletPoint : MonoBehaviour {

	public Vector3 oldPosition;
	public Vector3 acceleration = Vector3.zero;

	public List<VerletLink> links = new List<VerletLink> ();

	public float mass = 1.0f;

	public float radius = 0.035f;

	public bool isSticky = false;
	public bool pinned = false;

	public Vector3 pin;

	// Use this for initialization
	void Start () {
		oldPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		//transform.position = location;
	}

	void UpdatePhysics(Vector3 forceToApply, float deltaTime) {
		ApplyForce (forceToApply);

		Vector3 velocity = transform.position - oldPosition;
		float squaredTimeStep = deltaTime * deltaTime;

		Vector3 nextPosition = transform.position + velocity + 0.5f * acceleration * squaredTimeStep;

		oldPosition = transform.position;
		transform.position = nextPosition;
		acceleration = Vector3.zero;
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.black;
		Gizmos.DrawSphere (transform.position, radius);

		if (links.Count > 0) {
			for (int i = 0; i < links.Count; i++) {
				links [i].DrawLink ();
			}
		}
	}

	void SolveConstraints() {
		foreach (VerletLink link in links) {
			link.SolveLinkConstraint ();
		}

		if (pinned) {
			transform.position = pin;
			oldPosition = pin;
		}
	}

	public void ApplyForce(Vector3 force) {
		acceleration += force / mass;
	}

	public void PinTo(Vector3 pin) {
		pinned = true;
		this.pin = pin;
	}
}
