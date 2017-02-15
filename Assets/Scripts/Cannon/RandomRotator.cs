using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomRotator : MonoBehaviour {

	private Cannon cannon;

	void Start() {
		cannon = GetComponentInChildren<Cannon> ();
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space) && cannon.isActive) {
			int randomNum = Random.Range (20, 65);

			this.transform.eulerAngles = new Vector3 (0.0f, transform.localRotation.eulerAngles.y, (float)(randomNum * -1));
		}
	}
}
