using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombiner : MonoBehaviour {

	private bool found = false;
	private bool handled = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (found) {
			if (!handled) {
				
				MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter> ();
				CombineInstance[] combine = new CombineInstance[meshFilters.Length];
				int i = 0;
				while (i < meshFilters.Length) {
					combine [i].mesh = meshFilters [i].sharedMesh;
					combine [i].transform = meshFilters [i].transform.localToWorldMatrix;
					meshFilters [i].gameObject.active = false;
					i++;
				}

				transform.GetComponent<MeshFilter> ().mesh = new Mesh ();
				transform.GetComponent<MeshFilter> ().mesh.CombineMeshes (combine);
				transform.GetComponent<MeshRenderer>().enabled = true;
				transform.GetComponent<MeshRenderer> ().material.color = Color.white;
				GetComponent<MeshRenderer> ().material.shader = Shader.Find ("Unlit/Color");

				transform.Translate(new Vector3(0.0f, 0.5f, 0.0f));

				transform.gameObject.SetActive (true);

				handled = true;
			}
		} else {
			MountainGenerator[] mountainGenerators = this.gameObject.GetComponentsInChildren<MountainGenerator> ();

			if (mountainGenerators != null && mountainGenerators.Length > 1) {
				found = true;
			}
		}
	}
}
