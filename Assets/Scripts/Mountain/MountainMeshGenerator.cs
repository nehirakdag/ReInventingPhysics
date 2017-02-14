using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MountainMeshGenerator : MonoBehaviour {

	public int width;
	public int height;
	public int recursionLevelNumber;
	public int smoothness;

	private int vertexCount;

	private Vector3[] vertices;
	private Vector3[] normals;
	private Vector2[] UVs;
	private int[] triangles;

	// Use this for initialization
	void Start () {
		Mesh mountainMesh = GenerateMountain ();
		//Mesh leftSide = CreateSlopeMesh (true);
		//Mesh rightSide = CreateSlopeMesh (false);
		//Mesh mountainMesh = new Mesh ();

		GetComponent<MeshFilter> ().mesh = mountainMesh;
		GetComponent<MeshFilter> ().mesh.RecalculateBounds ();
		GetComponent<MeshFilter> ().mesh.RecalculateNormals ();
	}

	// Update is called once per frame
	Mesh GenerateMountain () {
		Mesh leftSide = CreateSlopeMesh (true);
		Mesh rightSide = CreateSlopeMesh (false);

		vertices = new Vector3[leftSide.vertices.Length + rightSide.vertices.Length];
		triangles = new int[leftSide.triangles.Length + rightSide.triangles.Length + 6];

		UVs = new Vector2[vertices.Length];
		normals = new Vector3[vertices.Length];

		for (int i = 0; i < vertices.Length; i++) {
			UVs [i] = Vector2.zero;
			normals [i] = Vector3.back;
		}

		leftSide.vertices.CopyTo (vertices, 0);
		rightSide.vertices.CopyTo (vertices, leftSide.vertices.Length);

		leftSide.triangles.CopyTo (triangles, 0);
		rightSide.triangles.CopyTo (triangles, leftSide.triangles.Length);

		triangles [triangles.Length - 6] = 0;
		triangles [triangles.Length - 5] = leftSide.vertices.Length - 1;
		triangles [triangles.Length - 4] = leftSide.vertices.Length - 1 + rightSide.vertices.Length - 1;
		triangles [triangles.Length - 3] = 0;
		triangles [triangles.Length - 2] = leftSide.vertices.Length - 1 + rightSide.vertices.Length - 1;
		triangles [triangles.Length - 1] = leftSide.vertices.Length - 1 + 1;

		Mesh mountain = new Mesh ();
		mountain.vertices = vertices;
		mountain.triangles = triangles;
		mountain.normals = normals;
		mountain.uv = UVs;

		return mountain;
	}

	Mesh CreateSlopeMesh(bool leftSide) {
		vertexCount = 0;

		for (int i = 1; i < recursionLevelNumber + 1; i++) {
			vertexCount += (int)Mathf.Pow (2.0f, (float)i);
			//Debug.Log ("VertexCount = " + vertexCount);
		}
		vertexCount += 4;

		vertices = new Vector3[vertexCount];
		normals = new Vector3[vertexCount];
		UVs = new Vector2[vertexCount];
		triangles = new int[3* (vertexCount - 2)];

		for (int i = 0; i < vertexCount; i++) {
			UVs [i] = Vector2.zero;
			normals [i] = Vector3.back;
		}

		if (leftSide) {
			vertices [0] = new Vector3 (0.0f, -3.5f, 0.0f);
			vertices [1] = vertices [0] + new Vector3 (-width / 2, 0.0f, 0.0f);
			vertices [vertexCount - 1] = vertices [0] + new Vector3 (0.0f, height, 0.0f);
			vertices [vertexCount / 2] = (vertices [1] + vertices [vertexCount - 1]) / 2.0f;

			triangles [0] = 0;
			triangles [1] = 1;
			triangles [2] = vertexCount / 2;

			triangles [triangles.Length / 2] = 0;
			triangles [triangles.Length / 2 + 1] = vertexCount / 2;
			triangles [triangles.Length / 2 + 2] = vertexCount - 1;
		} else {
			vertices [0] = new Vector3(0.0f, -3.5f, 0.0f);
			vertices [1] = vertices [0] + new Vector3 (width / 2, 0.0f, 0.0f);
			vertices [vertexCount - 1] = vertices [0] + new Vector3 (0.0f, height, 0.0f);
			vertices [vertexCount / 2] = (vertices [1] + vertices [vertexCount - 1]) / 2.0f;

			triangles [0] = 0;
			triangles [1] = vertexCount / 2;
			triangles [2] = 1;

			triangles [triangles.Length / 2] = 0;
			triangles [triangles.Length / 2 + 1] = vertexCount - 1;
			triangles [triangles.Length / 2 + 2] = vertexCount / 2;
		}


		int recursiveVertexCount = vertexCount - (int)Mathf.Pow (2.0f, (float)recursionLevelNumber);

		MakeRecursiveMesh (recursiveVertexCount, triangles.Length / 2, 0, 1, recursionLevelNumber - 1, leftSide);
		MakeRecursiveMesh (recursiveVertexCount, triangles.Length / 2, triangles.Length / 2, vertexCount / 2, recursionLevelNumber - 1, leftSide);

		Mesh slopeMesh = new Mesh ();
		slopeMesh.vertices = vertices;
		slopeMesh.triangles = triangles;
		slopeMesh.normals = normals;
		slopeMesh.uv = UVs;

		return slopeMesh;
	}

	void MakeRecursiveMesh(int recursiveVertexCount, int halfTriangleCount, int triangleIndex, int leftVertexIndex, int recursionLevel, bool leftSide) {
		int rightVertexIndex = leftVertexIndex + (int)Mathf.Pow (2.0f, recursionLevel + 1);

		Debug.Log ("Rightside = " + rightVertexIndex);
		Vector3 midpoint = Vector3.Lerp (vertices [leftVertexIndex], vertices [rightVertexIndex], 0.5f);
		Vector3 normal = new Vector3 (-(vertices [rightVertexIndex].y - vertices [leftVertexIndex].y), 
			vertices [rightVertexIndex].x - vertices [leftVertexIndex].x);

		if (Random.Range (0.0f, 1.0f) < 0.5f) {
			normal.Scale (-1 * Vector3.one);
		}

		midpoint += normal / (smoothness * Vector3.Distance (vertices [leftVertexIndex], vertices [rightVertexIndex]));
		vertices [leftVertexIndex + (int)Mathf.Pow (2.0f, recursionLevel)] = midpoint;

		if (leftSide) {
			triangles [triangleIndex] = 0;
			triangles [triangleIndex + 1] = leftVertexIndex;
			triangles [triangleIndex + 2] = leftVertexIndex + 1;

			triangles [triangleIndex + 3] = 0;
			triangles [triangleIndex + 4] = leftVertexIndex + 1;
			triangles [triangleIndex + 5] = leftVertexIndex + 2;
		} else {
			triangles [triangleIndex] = 0;
			triangles [triangleIndex + 1] = leftVertexIndex + 1;
			triangles [triangleIndex + 2] = leftVertexIndex;

			triangles [triangleIndex + 3] = 0;
			triangles [triangleIndex + 4] = leftVertexIndex + 2;
			triangles [triangleIndex + 5] = leftVertexIndex + 1;
		}



		if (recursionLevel != 0) {
			int newRecursiveVertexCount = recursiveVertexCount - (int)Mathf.Pow (2.0f, (float)recursionLevel);

			MakeRecursiveMesh (newRecursiveVertexCount, halfTriangleCount / 2, triangleIndex, leftVertexIndex, recursionLevel - 1, leftSide);
			MakeRecursiveMesh (newRecursiveVertexCount, halfTriangleCount / 2, triangleIndex + halfTriangleCount / 2, leftVertexIndex + recursiveVertexCount /2 - 1, recursionLevel - 1, leftSide);
		}
	}
}

