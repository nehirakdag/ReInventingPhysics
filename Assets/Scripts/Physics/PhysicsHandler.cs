using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that handles physics collision within the scene.
// Collision detections and appropriate computations are performed within this class
// I use the GJK algorithm to detect collisions.
// For this algorithm to work we need
public class PhysicsHandler : MonoBehaviour {

	public float collisionTolerence = 0.001f;

	// To build a simplex, we need to find the minkowski difference. To find the appropriate result,
	// we must find the maximum distance point that the shape has in a desired direction. Using these
	// "furthest points" aka supporting vertices, we will end up with the maximum areas for our computations,
	// thereby providing us with desirable results
	private Vector2 GetSupportingVertex(Vector3[] vertices, Vector3 direction) {
		int supportingVertexIndex = 0;
		float maxDot;

		// The furtest point among the vertices in the desired direction will be the point that
		// gives the largest dot product with the direction vector.
		// Therefore we check for each vector and return the largest dot product yielding one
		if (vertices.Length > 0) {
			maxDot = Vector2.Dot(vertices [supportingVertexIndex], direction);

			for (int i = 0; i < vertices.Length; i++) {
				float dot = Vector2.Dot(vertices [i], direction);

				if (dot > maxDot) {
					maxDot = dot;
					supportingVertexIndex = i;
				}
			}

		}
		return vertices[supportingVertexIndex];
	}

	// Same function but for an argument of vector2 array
	private Vector2 GetSupportingVertex(Vector2[] vertices, Vector3 direction) {
		int supportingVertexIndex = 0;
		float maxDot;

		if (vertices.Length > 0) {
			maxDot = Vector2.Dot(vertices [supportingVertexIndex], direction);

			for (int i = 0; i < vertices.Length; i++) {
				float dot = Vector2.Dot(vertices [i], direction);

				if (dot > maxDot) {
					maxDot = dot;
					supportingVertexIndex = i;
				}
			}

		}
		return vertices[supportingVertexIndex];
	}

	// We need a support function to build a simplex. Since we can find the Minkowski Difference by 
	// subtracting a point from each shape, we have to ensure these points are different each time.
	// Thus we make this function dependent on the input direction argument which we can edit to ensure useful reasults each call.
	private Vector2 SupportFunction(Vector3[] verticesA, Vector2[] verticesB, Vector2 direction) {
		Vector2 point1 = GetSupportingVertex (verticesA, direction);
		Vector2 point2 = GetSupportingVertex (verticesB, -1f * direction);

		Vector2 minkowskiDifference = new Vector2 (point1.x - point2.x, point1.y - point2.y);

		return minkowskiDifference;
	}

	// Same function adjusted to override with different arguments
	private Vector2 SupportFunction(Vector2[] verticesA, Vector2[] verticesB, Vector2 direction) {
		Vector2 point1 = GetSupportingVertex (verticesA, direction);
		Vector2 point2 = GetSupportingVertex (verticesB, -1f * direction);

		Vector2 minkowskiDifference = new Vector2 (point1.x - point2.x, point1.y - point2.y);

		return minkowskiDifference;
	}

	// Function based on GJK algorithm to detect collisions. Used by creating a simplex and checking if it encloses the origin
	// As we will use the simplex to create a collision response if there is a collision, this function
	// simply returns the simplex if a collision was detected, instead of a boolean return type
	public Simplex DetectCollision(Vector3[] verticesA, Vector2[] verticesB) {
		// Start with an arbitrary direction vector
		Vector2 checkDirection = new Vector2 (1.0f, -1.0f);

		Simplex simplex = new Simplex ();

		// Add the support function's result to the simplex
		simplex.simplex2D.Add(SupportFunction(verticesA, verticesB, checkDirection));

		checkDirection *= -1.0f;

		// We will need multiple iterations and "reshaping" of the simplex
		// until we find a result that interests us. Therefore we have a while(true) loop.
		while (true) {
			// Add the support function's result to the simplex for the new direction vector
			simplex.simplex2D.Add(SupportFunction(verticesA, verticesB, checkDirection));

			// If the point added in the previous line was not past the origin in the check direction,
			// then the minkowski difference can not contain the origin, so we can terminate with no collisions
			// detected
			if (Vector2.Dot (simplex.simplex2D [simplex.simplex2D.Count - 1], checkDirection) <= 0.0f) {
				return null;
			} else {
				// If the simplex encloses the origin, then return it. This will be used to mean there 
				// is a collision detected. If not, we will keep checking with the new checkDirection value
				if (simplex.ContainsOrigin2D (ref checkDirection)) {
					return simplex;
				}
			}
		}
	}

	// Function to create appropriate collision responses based on the EPA algorithm.
	// Uses the simplex returned by the GJK algorithm to compute the collision normal and
	// penetration distance so that proper handling can be performed
	public Simplex HandleCollision(Vector3[] verticesA, Vector2[] verticesB, Simplex collisionSimplex) {
		// Get the winding of the simplex. If the cross product is greater than 1, then it is clockwise, if not it is counterclockwise
		// This will affect the direction of the edge normals that we will use 
		float crossProduct = (collisionSimplex.simplex2D [1].x - collisionSimplex.simplex2D [0].x) * (collisionSimplex.simplex2D [2].y - collisionSimplex.simplex2D [1].y) -
			(collisionSimplex.simplex2D [1].y - collisionSimplex.simplex2D [0].y) * (collisionSimplex.simplex2D [2].x - collisionSimplex.simplex2D [1].x);

		collisionSimplex.winding = (crossProduct > 0) ? 1 : -1;

		while (true) {
			// Get the simplex edge that is closest to the origin.
			// To do this, we must create an edge with the given simplex points
			PolygonEdge closestEdge = FindClosestEdgeAtSimplex (collisionSimplex);

			// Get a new support point
			Vector2 p = SupportFunction (verticesA, verticesB, closestEdge.normal);

			// Check if it is close enough to the origin. If it is, then we have found our result
			float distanceOfPAlongEdgeNormal = Vector2.Dot (p, closestEdge.normal);

			if (distanceOfPAlongEdgeNormal - closestEdge.distance < collisionTolerence) {
				collisionSimplex.collisionNormal = closestEdge.normal;
				collisionSimplex.penetratingDistance = distanceOfPAlongEdgeNormal;
				return collisionSimplex;
			} else {
				// we can get closer to the origin, add a new point to the simplex
				// in between the two points that gave the closest edge and check again
				collisionSimplex.simplex2D.Insert (closestEdge.index, p);
			}
		}
	}

	private PolygonEdge FindClosestEdgeAtSimplex(Simplex simplex) {
		PolygonEdge closestEdge = new PolygonEdge ();
		closestEdge.distance = float.MaxValue;

		// for each point in the simplex, compute the edge they form and get the 
		// distance that that edge's normal makes with the origin.
		// The closest edge will be the one with the smallest value, so return it
		for (int i = 0; i < simplex.simplex2D.Count; i++) {
			int j = (i + 1 == simplex.simplex2D.Count) ? 0 : i + 1;

			Vector2 A = simplex.simplex2D [i];
			Vector2 B = simplex.simplex2D [j];

			Vector2 edge = B - A;
			Vector2 OA = A - Vector2.zero;

			Vector2 edgeTowardsOrigin = (simplex.winding == -1) ? new Vector2(edge.y, -edge.x).normalized : new Vector2(-edge.y, edge.x).normalized;

			float distanceToEdge = Vector2.Dot (edgeTowardsOrigin, A);

			if (distanceToEdge < closestEdge.distance) {
				closestEdge.distance = distanceToEdge;
				closestEdge.normal = edgeTowardsOrigin;
				closestEdge.index = j;
			}
		}

		return closestEdge;
	}

	// Helper function to get the closest point to the origin of the line created by the two vectors
	private Vector2 GetClosestPointToOriginOfLine(Vector2 A, Vector2 B) {
		Vector2 AB = B - A;
		Vector2 A0 = Vector2.zero - A;

		Vector2 closestPoint = AB * ((Vector2.Dot (AB, A0)) / (Vector2.Dot (AB, AB))) + A;
		return closestPoint;
	}
		
}
