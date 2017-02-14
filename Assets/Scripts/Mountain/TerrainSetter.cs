using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainSetter : MonoBehaviour {

	public int length = 20;
	public float grain = 8.0f;
	public int seed = 42;

	private float[,] heightmap;

	// Use this for initialization
	void Start () {
		Terrain terrain = GetComponent<Terrain> ();
		heightmap = MidPointDisplacementGenerator.GenerateHeightMap(length, grain, seed);
		//heightmap = new float[20,20];

		for (int i = 0; i < heightmap.GetLength (0); i++) {
			for (int j = 0; j < heightmap.GetLength (1); j++) {
				heightmap [i, j] /= 2.0f;
			}
		}

		if (terrain != null) {
			terrain.terrainData.SetHeights (0, 0, heightmap);
		}
		for (int i = 0; i < heightmap.GetLength (0); i++) {
			for (int j = 0; j < heightmap.GetLength (1); j++) {
				Debug.Log ("Heightmap[" + i + ", " + j + "] = " + heightmap [i, j]);
			}
		}

		Debug.Log ("Xres = " + terrain.terrainData.heightmapWidth + " , Yres = " + terrain.terrainData.heightmapHeight);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
