using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that serves as an abstraction of the polygon
// that we build around the Minkowski Diference.
// Maintains a list of points that are the vertices of the
// polygon. We are mainly interested to check if the
// polygon enclosed by these points contain the origin or not.
public class Simplex {

	public List<Vector2> simplex2D;

	public float penetratingDistance;
	public Vector2 collisionNormal;

	public int winding;

	public Simplex() {
		simplex2D= new List<Vector2> ();
	}

	// Method for 2D points to check if the simplex contains the origin.
	// If it does, we know that we have a collision
	public bool ContainsOrigin2D(ref Vector2 distance) {
		// Get the last point in the simplex list
		Vector2 A = simplex2D [simplex2D.Count - 1];
		Vector2 A0 = -1.0f * A;

		// If we have a triangle, get the other 2 points B and C
		if (simplex2D.Count == 3) {
			Vector2 B = simplex2D [0];
			Vector2 C = simplex2D [1];

			Vector2 AB = B - A;
			Vector2 AC = C - A;

			// set the checked distance vector to be perpendicular to AB
			distance = new Vector3 (-AB.y, AB.x, -0.1f);

			// Verify if we are going at the right direction by comparing it with the third.
			// If not, reverse the direction of the vector
			if (Vector2.Dot (distance, C) > 0.0f) {
				distance *= -1.0f;
			}

			// If we are in the same direction with the origin then C is the furthest vector
			// among the given three that we have. So we can remove it
			if (Vector2.Dot (distance, A0) > 0.0f) {
				simplex2D.Remove (C);
				return false;
			}

			// Repeat the same process for AC with B being the third vector
			distance = new Vector3 (-AC.y, AC.x, -0.1f);

			if (Vector2.Dot (distance, B) > 0.0f) {
				distance *= -1.0f;
			}

			if (Vector2.Dot (distance, A0) > 0.0f) {
				simplex2D.Remove (B);
				return false;
			}

			// Then origin must be inside the area covered by the triangle simplex
			return true;
		} else {
			// If we have a line, simplex can not be containing the origin. We must 
			// edit the reference vector if necessary
			Vector2 B = simplex2D [0];
			Vector2 AB = B - A;

			distance = new Vector3 (-AB.y, AB.x, -0.1f);

			if(Vector2.Dot (distance, A0) < 0.0f) {
				distance *= -1.0f;
			}
		}

		return false;
	}
}
