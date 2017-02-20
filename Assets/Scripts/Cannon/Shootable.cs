using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Abstract parent class that CannonBall and Goat descend from
// implements common trajectory physics variables
public abstract class Shootable : MonoBehaviour {

	public float radius;

	public float speed;
	public float elevationAngle;

	public float notMovingSince;

	public Vector2 windForce;

	public float bounciness;

	public Vector2 currentVelocity;
	public bool moving = true;
	protected bool movingAtY = true;
	public bool pinned = false;
	public bool initialized = false;

	// Use this for initialization
	public virtual void Start () {
		
	}
	
	// Update is called once per frame
	public virtual void Update () {

	}
}
