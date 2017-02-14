using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsHandler : MonoBehaviour {

	public float collisionTolerence = 0.001f;

	private Vector2 GetSupportingVertex(Vector3[] vertices, Vector3 direction) {
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

	private Vector2 SupportFunction(Vector3[] verticesA, Vector2[] verticesB, Vector2 direction) {
		Vector2 point1 = GetSupportingVertex (verticesA, direction);
		Vector2 point2 = GetSupportingVertex (verticesB, -1f * direction);

		Vector2 minkowskiDifference = new Vector2 (point1.x - point2.x, point1.y - point2.y);

		return minkowskiDifference;
	}

	private Vector2 SupportFunction(Vector2[] verticesA, Vector2[] verticesB, Vector2 direction) {
		Vector2 point1 = GetSupportingVertex (verticesA, direction);
		Vector2 point2 = GetSupportingVertex (verticesB, -1f * direction);

		Vector2 minkowskiDifference = new Vector2 (point1.x - point2.x, point1.y - point2.y);

		return minkowskiDifference;
	}

	public Simplex DetectMeshSpriteCollision(Mesh mesh, SpriteRenderer spriteRenderer) {
		Vector3[] verticesA = mesh.vertices;
		Vector2[] verticesB = new Vector2[spriteRenderer.sprite.vertices.Length];

		for (int i = 0; i < verticesB.Length; i++) {
			Vector2 spriteWorldPosition = spriteRenderer.transform.TransformPoint(spriteRenderer.sprite.vertices[i]);
			verticesB [i] = new Vector2 (spriteWorldPosition.x, spriteWorldPosition.y);
		}

		Vector2 checkDirection = new Vector2 (1.0f, -1.0f);

		Simplex simplex = new Simplex ();

		simplex.simplex2D.Add(SupportFunction(verticesA, verticesB, checkDirection));

		checkDirection *= -1.0f;

		while (true) {
			simplex.simplex2D.Add(SupportFunction(verticesA, verticesB, checkDirection));

			if (Vector2.Dot (simplex.simplex2D [simplex.simplex2D.Count - 1], checkDirection) <= 0.0f) {
				//return false;
				return null;
			} else {
				if (simplex.ContainsOrigin2D (ref checkDirection)) {
					return simplex;
				}
			}
		}
	}

	public Simplex HandleCollision(Mesh mesh, SpriteRenderer spriteRenderer, Simplex collisionSimplex) {
		Vector3[] verticesA = mesh.vertices;
		Vector2[] verticesB = new Vector2[spriteRenderer.sprite.vertices.Length];

		for (int i = 0; i < verticesB.Length; i++) {
			Vector2 spriteWorldPosition = spriteRenderer.transform.TransformPoint(spriteRenderer.sprite.vertices[i]);
			verticesB [i] = new Vector2 (spriteWorldPosition.x, spriteWorldPosition.y);
		}

		float crossProduct = (collisionSimplex.simplex2D [1].x - collisionSimplex.simplex2D [0].x) * (collisionSimplex.simplex2D [2].y - collisionSimplex.simplex2D [1].y) -
		                     (collisionSimplex.simplex2D [1].y - collisionSimplex.simplex2D [0].y) * (collisionSimplex.simplex2D [2].x - collisionSimplex.simplex2D [1].x);

		collisionSimplex.winding = (crossProduct > 0) ? 1 : -1;

		while (true) {
			//Vector2 closestEdge = FindClosestEdgeAtSimplex (collisionSimplex);
			PolygonEdge closestEdge = FindClosestEdgeAtSimplex (collisionSimplex);
			Vector2 p = SupportFunction (verticesA, verticesB, closestEdge.normal);

			float distanceOfPAlongEdgeNormal = Vector2.Dot (p, closestEdge.normal);

			//if (Mathf.Abs(distanceOfPAlongEdgeNormal - closestEdge.distance) < collisionTolerence) {
			if (distanceOfPAlongEdgeNormal - closestEdge.distance < collisionTolerence) {
				collisionSimplex.collisionNormal = closestEdge.normal;
				collisionSimplex.penetratingDistance = distanceOfPAlongEdgeNormal;
				return collisionSimplex;
			} else {
				collisionSimplex.simplex2D.Insert (closestEdge.index, p);
			}
		}
	}

	private PolygonEdge FindClosestEdgeAtSimplex(Simplex simplex) {
		PolygonEdge closestEdge = new PolygonEdge ();
		closestEdge.distance = float.MaxValue;

		for (int i = 0; i < simplex.simplex2D.Count; i++) {
			int j = (i + 1 == simplex.simplex2D.Count) ? 0 : i + 1;

			Vector2 A = simplex.simplex2D [i];
			Vector2 B = simplex.simplex2D [j];

			Vector2 edge = B - A;
			Vector2 OA = A - Vector2.zero;

			//Vector2 edgeTowardsOrigin = TripleProduct (edge, OA, edge).normalized;
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

	private Vector2 TripleProduct(Vector2 A, Vector2 B, Vector2 C) {
		return B * (Vector2.Dot (A, C)) - A * (Vector2.Dot (B, C));
	}

	/*public bool DetectMeshSpriteCollisionWithInfo(Mesh mesh, SpriteRenderer spriteRenderer) {
		Vector3[] verticesA = mesh.vertices;
		Vector2[] verticesB = new Vector2[spriteRenderer.sprite.vertices.Length];

		for (int i = 0; i < verticesB.Length; i++) {
			Vector2 spriteWorldPosition = spriteRenderer.transform.TransformPoint(spriteRenderer.sprite.vertices[i]);
			verticesB [i] = new Vector2 (spriteWorldPosition.x, spriteWorldPosition.y);
		}

		Vector2 checkDirection = new Vector2 (1.0f, -1.0f);

		Simplex simplex = new Simplex ();

		simplex.simplex2D.Add(SupportFunction(verticesA, verticesB, checkDirection));

		checkDirection *= -1.0f;
		simplex.simplex2D.Add(SupportFunction(verticesA, verticesB, checkDirection));

		checkDirection = GetClosestPointToOriginOfLine (simplex.simplex2D [0], simplex.simplex2D [1]);
		while (true) {
			checkDirection *= -1.0f;

			if (checkDirection == Vector2.zero) {
				return false;
			}

			Vector2 newMinkowski = SupportFunction(verticesA, verticesB, checkDirection);
			Debug.Log ("Newminkowski = " + newMinkowski);

			float dNew = Vector2.Dot (newMinkowski, checkDirection);
			float dOld = Vector2.Dot (simplex.simplex2D[0], checkDirection);

			if (Mathf.Abs (dNew - dOld) < collisionTolerence) {
				simplex.leastPenetratingDistance = checkDirection.magnitude;
				return true;
			}

			Vector2 p1 = GetClosestPointToOriginOfLine (simplex.simplex2D [0], newMinkowski);
			Vector2 p2 = GetClosestPointToOriginOfLine (newMinkowski, simplex.simplex2D [1]);

			if (p1.magnitude < p2.magnitude) {
				simplex.simplex2D [1] = newMinkowski;
				checkDirection = p1;
			} else {
				simplex.simplex2D [0] = newMinkowski;
				checkDirection = p2;
			}
		}
	}*/

	private Vector2 GetClosestPointToOriginOfLine(Vector2 A, Vector2 B) {
		Vector2 AB = B - A;
		Vector2 A0 = Vector2.zero - A;

		Vector2 closestPoint = AB * ((Vector2.Dot (AB, A0)) / (Vector2.Dot (AB, AB))) + A;
		return closestPoint;
	}

}
