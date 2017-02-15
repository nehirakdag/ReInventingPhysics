using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerletLink : MonoBehaviour {

	public VerletPoint pointA;
	public VerletPoint pointB;

	public Vector3 initialDistance;

	public bool drawThisLink = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		DrawLink ();
	}

	public void DrawLink() {
		if (drawThisLink) {
			Debug.DrawLine (pointA.transform.position, pointB.transform.position, Color.black);
		}
	}

	public void SolveLinkConstraint() {
		Vector3 delta = pointB.transform.position - pointA.transform.position;
		float currentDistance = delta.magnitude;
		float errorFactor = (currentDistance - initialDistance.magnitude) / currentDistance;

		pointA.transform.position += errorFactor * 0.5f * delta;
		pointB.transform.position -= errorFactor * 0.5f * delta;
	}
}
