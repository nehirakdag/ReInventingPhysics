using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomRotator : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			int randomNum = Random.Range (20, 65);

			this.transform.eulerAngles = new Vector3 (0.0f, 180.0f, (float)(randomNum * -1));
		}
	}
}
