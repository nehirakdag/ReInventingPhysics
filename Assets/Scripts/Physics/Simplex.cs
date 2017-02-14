using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simplex {

	public List<Vector3> simplex;
	public List<Vector2> simplex2D;

	public float penetratingDistance;
	public Vector2 collisionNormal;

	public int winding;

	public Simplex() {
		simplex = new List<Vector3> ();
		simplex2D= new List<Vector2> ();
	}

	public bool ContainsOrigin() {
		return false;
	}

	public bool ContainsOrigin2D(ref Vector2 distance) {
		Vector2 A = simplex2D [simplex2D.Count - 1];
		Vector2 A0 = -1.0f * A;

		if (simplex2D.Count == 3) {
			Vector2 B = simplex2D [0];
			Vector2 C = simplex2D [1];

			Vector2 AB = B - A;
			Vector2 AC = C - A;

			distance = new Vector3 (-AB.y, AB.x, -0.1f);

			if (Vector2.Dot (distance, C) > 0.0f) {
				distance *= -1.0f;
			}

			if (Vector2.Dot (distance, A0) > 0.0f) {
				simplex2D.Remove (C);
				return false;
			}

			distance = new Vector3 (-AC.y, AC.x, -0.1f);

			if (Vector2.Dot (distance, B) > 0.0f) {
				distance *= -1.0f;
			}

			if (Vector2.Dot (distance, A0) > 0.0f) {
				simplex2D.Remove (B);
				return false;
			}

			return true;
		} else {
			Vector2 B = simplex2D [0];
			Vector2 AB = B - A;

			distance = new Vector3 (-AB.y, AB.x, -0.1f);

			if(Vector2.Dot (distance, A0) < 0.0f) {
				distance *= -1.0f;
			}
		}

		return false;
	}

	public bool containsOrigin(ref Vector2  d)
	{
		// get the last point added to the simplex
		Vector2 a = simplex2D[simplex2D.Count -1];
		// compute AO (same thing as -A)
		Vector2 ao = new Vector2(-a.x, -a.y);

		//triangle  ABC
		if (simplex2D.Count == 3)
		{

			// get b and c
			Vector2 b = simplex2D[1];
			Vector2 c = simplex2D[0];

			Vector2 ab = b - a;
			Vector2 ac = c - a;

			//direction perpendicular to AB
			d=new Vector2(-ab.y,ab.x);
			//away from C
			if(Vector2.Dot(d,c)>0)// if same direction, make d opposite
			{
				d = d * -1.0f;
			}

			//If the new vector (d) perpenicular on AB is in the same direction with the origin (A0)
			//it means that C is the furthest from origin and remove to create a new simplex
			if(Vector2.Dot(d,ao)>0)//same direction
			{
				simplex2D.Remove(c);
				return false;
			}

			//direction to be perpendicular to AC
			d = new Vector2(-ac.y, ac.x);

			//away form B
			if(Vector2.Dot(d, b) >0)
			{
				d = d * -1.0f;
			}

			//If the new vector (d) perpenicular on AC edge, is in the same direction with the origin (A0)
			//it means that B is the furthest from origin and remove to create a new simplex

			if(Vector2.Dot(d, ao) > 0)
			{
				simplex2D.Remove(b);
				return false;
			}

			//origin must be inside the triangle, so this is the simplex
			return true;
		}

		//line
		else
		{
			// then its the line segment case
			Vector2 b = simplex2D[0];
			// compute AB
			Vector2 ab = b - a;

			//direction perpendicular to ab, to orgin: ABXAOXAB
			d = new Vector2(-ab.y, ab.x);
			if(Vector2.Dot(d, ao)<0)
			{
				d = d * -1.0f;
			}


		}
		return false;
	}
}
