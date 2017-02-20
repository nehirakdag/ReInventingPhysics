using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that is an abstraction of a polygon edge.
// Used to compute collisions following EPA algorithm
public class PolygonEdge {

	public Vector2 edge;
	public Vector2 normal;

	public int index;
	public float distance;

	public PolygonEdge() {

	}
}
