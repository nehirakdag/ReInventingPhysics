using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

	public static Vector2 GRAVITY_VECTOR = new Vector3(0.0f, -9.81f);
	public static float GRAVITY = -9.81f;

	public static float AIR_DENSITY = 1.293f;
	public static float DRAG_COEFFICIENT_SPHERE = 0.47f;

	public static float CrossSectionalArea(float radius) {
		return Mathf.Pow (radius, 2.0f) * Mathf.PI;
	}

	public static float GetCurrentWindSpeed(float windSpeedMax) {
		return Random.Range (-windSpeedMax, windSpeedMax);
	}

}
