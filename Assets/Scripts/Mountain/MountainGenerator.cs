using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MountainGenerator : MonoBehaviour {

	public int numIterations;
	public int smoothingFactor;
	public int mountainWidth;
	public int mountainHeight;

	public bool leftSide;

	private int numTotalVertices = 0;

	private Vector3[] vertices;
	private Vector3[] normals;
	private Vector2[] UVs;
	private int[] triangles;

	// Use this for initialization
	void Start () {
		Mesh slopeMesh = CreateSlopeMesh (leftSide);
		GetComponent<MeshFilter> ().mesh = slopeMesh;
		GetComponent<MeshFilter> ().mesh.RecalculateBounds ();
		GetComponent<MeshFilter> ().mesh.RecalculateNormals ();
	}

	// We will create a triangular mesh that will be the slope of the mountain,
	// and recursively apply the midpoint displacement algorithm
	Mesh CreateSlopeMesh(bool leftSide) {

		// Determine the number of vertices we will need so that we can set the
		// array to that size
		for (int i = 1; i < numIterations + 1; i++) {
			numTotalVertices += (int)Mathf.Pow (2.0f, (float)i);
		}
		numTotalVertices += 4;

		// Triangles will be three times the size of the total 
		// number of vertices minus two, because we already have the 
		// two main triangles, will recurse for inner ones
		vertices = new Vector3[numTotalVertices];
		triangles = new int[3* (numTotalVertices - 2)];

		// Our vertices orientations will differ depending on which
		// side of the slope we are at, so we must account for this
		if (leftSide) {
			// Vertex 0: bottom center of the mountain
			vertices [0] = new Vector3 (0.0f, -3.5f, 0.0f);
			// Vertex 1: left bottom side of the slope
			vertices [1] = vertices [0] + new Vector3 (-mountainWidth / 2, 0.0f, 0.0f);
			// Last Vertex: top end of the slope, i.e. the point at which the flat peak begins
			vertices [numTotalVertices - 1] = vertices [0] + new Vector3 (0.0f, mountainHeight, 0.0f);
			// Middle vertex: the midpoint of the last two vertices, i.e. the midpoint of the hypothenuse
			vertices [numTotalVertices / 2] = (vertices [1] + vertices [numTotalVertices - 1]) / 2.0f;

			// Triangles need to be clockwise
			// Start at bottom center, go to left side, midpoint of hypotenuse
			triangles [0] = 0;
			triangles [1] = 1;
			triangles [2] = numTotalVertices / 2;

			// Triangles need to be clockwise
			// Start at bottom center, go midpoint of hypotenuse, top
			triangles [triangles.Length / 2] = 0;
			triangles [triangles.Length / 2 + 1] = numTotalVertices / 2;
			triangles [triangles.Length / 2 + 2] = numTotalVertices - 1;
		} else {
			// Vertex 0: bottom center of the mountain
			vertices [0] = new Vector3(0.0f, -3.5f, 0.0f);
			// Vertex 1: right bottom side of the slope
			vertices [1] = vertices [0] + new Vector3 (mountainWidth / 2, 0.0f, 0.0f);
			// Last Vertex: top end of the slope, i.e. the point at which the flat peak begins
			vertices [numTotalVertices - 1] = vertices [0] + new Vector3 (0.0f, mountainHeight, 0.0f);
			// Middle vertex: the midpoint of the last two vertices, i.e. the midpoint of the hypothenuse
			vertices [numTotalVertices / 2] = (vertices [1] + vertices [numTotalVertices - 1]) / 2.0f;

			// Triangles need to be clockwise
			// Start at bottom center, go midpoint of hypotenuse, right bottom
			triangles [0] = 0;
			triangles [1] = numTotalVertices / 2;
			triangles [2] = 1;

			// Triangles need to be clockwise
			// Start at bottom center, go to top, then midpoint of hypotenuse
			triangles [triangles.Length / 2] = 0;
			triangles [triangles.Length / 2 + 1] = numTotalVertices - 1;
			triangles [triangles.Length / 2 + 2] = numTotalVertices / 2;
		}

		// Reduce the subset to deal with to the new value now that the current iteration is done
		int numSubsetVertices = numTotalVertices - (int)Mathf.Pow (2.0f, (float)numIterations);
		MidpointDisplacementIteration (numIterations - 1, numSubsetVertices, triangles.Length / 2, 0, 1);
		MidpointDisplacementIteration (numIterations - 1, numSubsetVertices, triangles.Length / 2, triangles.Length / 2, numTotalVertices / 2);

		Mesh mountainMesh = new Mesh ();
		mountainMesh.vertices = vertices;
		mountainMesh.triangles = triangles;

		return mountainMesh;
	}

	// Recursive function that iterates until the number of iterations are zero,
	// Performing the same operation of dividing the midpoints, adding triangles with random
	// lengths until the desired iteration number is reached
	void MidpointDisplacementIteration(int numIterations, int numSubsetVertices, int halfTriangleCount, int triangleIndex, int indexA) {
		// The two sides of the triangle have as many as the last iteration's index number of vertices between them.
		int indexB = indexA + (int)Mathf.Pow (2.0f, numIterations + 1);

		// Our vertices orientations will differ depending on which
		// side of the slope we are at, so we must account for this
		if (leftSide) {
			// Triangles need to be clockwise
			// Start at bottom center
			triangles [triangleIndex] = 0;
			// Go to new left side which will be indexA
			triangles [triangleIndex + 1] = indexA;
			// Then go to the next neighbor of the new left side
			triangles [triangleIndex + 2] = indexA + 1;

			// Set the second triangle similarly
			triangles [triangleIndex + 3] = 0;
			triangles [triangleIndex + 4] = indexA + 1;
			triangles [triangleIndex + 5] = indexA + 2;
			// We have divided the triangle into two smaller ones
		} else {
			// Triangles need to be clockwise
			// Start at bottom center
			triangles [triangleIndex] = 0;
			// Go to new left side which will be the neighbor of indexA
			triangles [triangleIndex + 1] = indexA + 1;
			// Then go to the next neighbor of the new left side, which will be below
			// an index compared to the left side orientation
			triangles [triangleIndex + 2] = indexA;

			// Set the second triangle similarly
			triangles [triangleIndex + 3] = 0;
			triangles [triangleIndex + 4] = indexA + 2;
			triangles [triangleIndex + 5] = indexA + 1;
			// We have divided the triangle into two smaller ones
		}

		// Find the midpoint of the two vertices
		Vector3 midpoint = Vector3.Lerp (vertices [indexA], vertices [indexB], 0.5f);
		// Decide randomly to either go higher or lower than the current vertex positions
		Vector3 normal = new Vector3 (-(vertices [indexB].y - vertices [indexA].y), 
			vertices [indexB].x - vertices [indexA].x);
		if (Random.Range (0.0f, 1.0f) < 0.5f) {
			normal *= -1.0f;
		}

		Vector3 deltaMidpoint = normal / (Vector3.Distance (vertices [indexA], vertices [indexB]) * smoothingFactor);
		midpoint += deltaMidpoint;
		vertices [indexA + (int)Mathf.Pow (2.0f, numIterations)] = midpoint;

		// repeat this opeation until the desired number of iterations is reached
		if (numIterations != 0) {
			// Reduce the subset to deal with to the new value now that the current iteration is done
			int newSubsetVerticesCount = numSubsetVertices - (int)Mathf.Pow (2.0f, (float)numIterations);

			MidpointDisplacementIteration (numIterations - 1, newSubsetVerticesCount, halfTriangleCount / 2, triangleIndex, indexA);
			MidpointDisplacementIteration (numIterations - 1, newSubsetVerticesCount, halfTriangleCount / 2, triangleIndex + halfTriangleCount / 2, indexA + numSubsetVertices /2 - 1);
		}
	}
}
