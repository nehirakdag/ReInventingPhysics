using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerletPoint : MonoBehaviour {

	public Vector3 location;
	public float radius = 0.05f;

	public bool isSticky = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = location;
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.black;
		Gizmos.DrawSphere (location, radius);
	}
}
