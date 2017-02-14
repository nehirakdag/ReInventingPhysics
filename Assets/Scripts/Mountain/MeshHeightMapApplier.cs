using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshHeightMapApplier : MonoBehaviour {

	public int length = 20;
	public float grain = 8.0f;
	public int seed = 42;

	private Vector3[] vertices;
	private float[,] heightmap;

	// Use this for initialization
	void Start () {
		vertices = GetComponent<MeshFilter> ().mesh.vertices;
		heightmap = MidPointDisplacementGenerator.GenerateHeightMap(length, grain, seed);

		for (int k = 0; k < vertices.Length; k++) {
			int i = k / heightmap.GetLength (0);
			int j = k / heightmap.GetLength (1);

			vertices [k] *= heightmap [i, j];
		}

		Debug.Log ("Vertices size: " + vertices.Length);


		GetComponent<MeshFilter> ().mesh.vertices = vertices;
		GetComponent<MeshFilter> ().mesh.RecalculateBounds ();
		GetComponent<MeshFilter> ().mesh.RecalculateNormals ();
	}

	// Update is called once per frame
	void Update () {

	}
}
