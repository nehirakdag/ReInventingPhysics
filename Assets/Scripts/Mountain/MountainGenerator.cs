using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MountainGenerator : MonoBehaviour {

	public int width;
	public int height;
	public int recursionLevelNumber;
	public int smoothness;

	public bool leftSide;

	private int vertexCount = 0;

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

		GetComponent<MeshRenderer> ().material.shader = Shader.Find ("Unlit/Color");
		GetComponent<MeshRenderer> ().material.color = Color.white;

	}

	// Update is called once per frame
	void Update () {
		
	}

	Mesh CreateSlopeMesh(bool leftSide) {
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

		MakeRecursiveMesh (recursiveVertexCount, triangles.Length / 2, 0, 1, recursionLevelNumber - 1);
		MakeRecursiveMesh (recursiveVertexCount, triangles.Length / 2, triangles.Length / 2, vertexCount / 2, recursionLevelNumber - 1);

		Mesh mountainMesh = new Mesh ();
		mountainMesh.vertices = vertices;
		mountainMesh.triangles = triangles;
		mountainMesh.normals = normals;

		return mountainMesh;
	}

	void MakeRecursiveMesh(int recursiveVertexCount, int halfTriangleCount, int triangleIndex, int leftVertexIndex, int recursionLevel) {
		int rightVertexIndex = leftVertexIndex + (int)Mathf.Pow (2.0f, recursionLevel + 1);

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

			MakeRecursiveMesh (newRecursiveVertexCount, halfTriangleCount / 2, triangleIndex, leftVertexIndex, recursionLevel - 1);
			MakeRecursiveMesh (newRecursiveVertexCount, halfTriangleCount / 2, triangleIndex + halfTriangleCount / 2, leftVertexIndex + recursiveVertexCount /2 - 1, recursionLevel - 1);
		}
	}
}
