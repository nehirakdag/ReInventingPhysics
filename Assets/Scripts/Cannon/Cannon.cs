using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour {

	public GameObject ammoPrefab;
	public float ammoLaunchSpeed;

	public Transform ammosLaunched;

	public bool isActive = false;

	public List<Shootable> shotsFired = new List<Shootable>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// Shoot cannonball if the cannon is active and the space key is pressed
		if (Input.GetKeyDown (KeyCode.Space) && isActive) {
			if (ammoPrefab != null) {
				ShootCannonBall ();
			}
		}
	}

	// Instantiate the ammo (cannonball for left cannon, goat for right cannon) at the end of the cannon
	// and give it the appropriate launch speed
	void ShootCannonBall() {
		GameObject ammo = Instantiate (ammoPrefab, this.transform.position + 0.1f * Vector3.back, this.transform.rotation, ammosLaunched.transform) as GameObject;
		ammo.GetComponent<Shootable> ().speed = ammoLaunchSpeed;
		ammo.GetComponent<Shootable> ().elevationAngle = Mathf.Abs(this.transform.rotation.eulerAngles.z);

		shotsFired.Add (ammo.GetComponent<Shootable> ());
	}
}
