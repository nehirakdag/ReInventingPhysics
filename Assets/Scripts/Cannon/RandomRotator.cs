using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// Class used to rotate cannon randomly between two angles at each shot.
// Minimum and maximum angles are editable
public class RandomRotator : MonoBehaviour {

	public int minAngle = 25;
	public int maxAngle = 65;
	private Cannon cannon;

	void Start() {
		cannon = GetComponentInChildren<Cannon> ();
	}

	// Update is called once per frame
	void Update () {
		// If space key is pressed and this cannon is currently active,
		// rotate the cannon to a random angle on the z axis
		if (Input.GetKeyDown (KeyCode.Space) && cannon.isActive) {
			int randomNum = Random.Range (minAngle, maxAngle);

			this.transform.eulerAngles = new Vector3 (0.0f, transform.localRotation.eulerAngles.y, (float)(randomNum * -1));
		}
	}
}
