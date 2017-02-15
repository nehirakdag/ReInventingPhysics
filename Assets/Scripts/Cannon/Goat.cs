using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goat : Shootable {

	public Vector3[] vertices;
	public int numVertices = 21;

	public GameObject verletPointPrefab;
	public VerletPoint[] verletPoints;

	public Bounds boundingBox;

	public float scaleSize = 0.1f;

	// Use this for initialization
	void Start () {
		currentVelocity = new Vector2(speed * Mathf.Cos(elevationAngle * Mathf.Deg2Rad), speed * Mathf.Sin(elevationAngle * Mathf.Deg2Rad));
		Debug.Log ("Starting Velocity = " + currentVelocity.ToString ());
		transform.Rotate (new Vector3(0.0f, 0.0f, -1 * elevationAngle));
		boundingBox = new Bounds ();
		boundingBox.center = new Vector3 (-0.01f, 0.18f);
		boundingBox.extents = new Vector3 (0.4f, 0.3f);
		transform.Translate (new Vector3 (0.0f, 0.0f, -0.6f));

		vertices = new Vector3[numVertices];
		verletPoints = new VerletPoint[numVertices];
	}

	// Update is called once per frame
	void Update () {
		SetGoatShapeVertices ();
		SetGoatVerletPoints ();
		AddConnectionLines ();

		Vector2 newVelocity = currentVelocity;

		if (movingAtY) {
			Vector2 drag = newVelocity.normalized * -0.5f * Movement.AIR_DENSITY * Movement.CrossSectionalArea (radius) * newVelocity.magnitude * newVelocity.magnitude;
			newVelocity += (drag + Movement.GRAVITY_VECTOR + windForce) * Time.deltaTime;

			if (transform.position.y <= -3.5f) {
				movingAtY = false;
			}
		} else {
			newVelocity.x = 0;
			newVelocity.y = 0;
			moving = false;
		}

		transform.Translate (new Vector3(newVelocity.x * Time.deltaTime, newVelocity.y * Time.deltaTime, 0.0f));
		currentVelocity = newVelocity;

		if (!moving) {
			notMovingSince += Time.deltaTime;
		}

		ShowBoundingBox ();
	}

	void SetGoatShapeVertices() {
		// Horn
		vertices [0] = new Vector3 (-2.0f, 8.0f) * scaleSize + this.transform.position; // connect to [1], [2]
		vertices [1] = new Vector3 (-3.0f, 7.0f) * scaleSize + this.transform.position; // connect to [20], [2]
		vertices [2] = new Vector3 (-4.0f, 7.0f) * scaleSize + this.transform.position; // connect to [0], [1], [4]

		// Eye
		vertices [3] = new Vector3 (-5.0f, 5.5f) * scaleSize + this.transform.position;

		// Face
		vertices [4] = new Vector3 (-7.0f, 5.0f) * scaleSize + this.transform.position; // connect to [2], [5]
		vertices [5] = new Vector3 (-7.0f, 3.0f) * scaleSize + this.transform.position; // connect to [4], [6]
		vertices [6] = new Vector3 (-6.0f, 3.5f) * scaleSize + this.transform.position; // connect to [5], [7], [8]
		vertices [7] = new Vector3 (-5.0f, 4.0f) * scaleSize + this.transform.position; // connect to [6], [8], [9]

		// Horn
		vertices [8] = new Vector3 (-5.5f, 2.0f) * scaleSize + this.transform.position; // connect to [6], [7]

		// Bottom
		vertices [9] = new Vector3 (-4.0f, 4.0f) * scaleSize + this.transform.position; // connect to [7], [10]

		// Left leg
		vertices [10] = new Vector3(2.0f, 2.0f) * scaleSize + this.transform.position; // connect to [9], [11], [13]
		vertices [11] = new Vector3(2.0f, -1.0f) * scaleSize + this.transform.position; // connect to [10], [12]
		vertices [12] = new Vector3(2.0f, -2.5f) * scaleSize + this.transform.position; // connect to [11]

		// Right leg
		vertices [13] = new Vector3(5.5f, 1.0f) * scaleSize + this.transform.position; // connect to [11], [14], [16]
		vertices [14] = new Vector3(5.5f, -2.0f) * scaleSize + this.transform.position; // connect to [13], [15]
		vertices [15] = new Vector3(5.5f, -3.5f) * scaleSize + this.transform.position; // connect to [14]

		// Back & tail
		vertices [16] = new Vector3(7.0f, 1.5f) * scaleSize + this.transform.position; // connect to [13], [17]
		vertices [17] = new Vector3(7.0f, 5.0f) * scaleSize + this.transform.position; // connect to [16], [18], [19]
		vertices [18] = new Vector3(8.0f, 7.0f) * scaleSize + this.transform.position; // connect to [17], [19]
		vertices [19] = new Vector3(6.0f, 6.0f) * scaleSize + this.transform.position; // connect to [17], [18], [20]

		vertices [20] = new Vector3(-1.0f, 6.0f) * scaleSize + this.transform.position; // connect to [1], [19]
	}
		
	void AddConnectionLines() {
		if (vertices.Length > 20) {
			// Horn
			Debug.DrawLine (vertices [0], vertices [1], Color.black);
			Debug.DrawLine (vertices [0], vertices [2], Color.black);
			Debug.DrawLine (vertices [1], vertices [20], Color.black);
			Debug.DrawLine (vertices [2], vertices [4], Color.black);

			// Face
			Debug.DrawLine (vertices [4], vertices [5], Color.black);
			Debug.DrawLine (vertices [5], vertices [6], Color.black);

			Debug.DrawLine (vertices [6], vertices [7], Color.black);
			Debug.DrawLine (vertices [6], vertices [8], Color.black);

			Debug.DrawLine (vertices [7], vertices [8], Color.black);
			Debug.DrawLine (vertices [7], vertices [9], Color.black);

			// Bottom
			Debug.DrawLine (vertices [9], vertices [10], Color.black);

			// Left leg
			Debug.DrawLine (vertices [10], vertices [11], Color.black);
			Debug.DrawLine (vertices [10], vertices [13], Color.black);
			Debug.DrawLine (vertices [11], vertices [12], Color.black);

			// Right leg
			Debug.DrawLine (vertices [13], vertices [14], Color.black);
			Debug.DrawLine (vertices [13], vertices [16], Color.black);
			Debug.DrawLine (vertices [14], vertices [15], Color.black);

			// Back & tail
			Debug.DrawLine (vertices [16], vertices [17], Color.black);
			Debug.DrawLine (vertices [17], vertices [18], Color.black);
			Debug.DrawLine (vertices [17], vertices [19], Color.black);

			Debug.DrawLine (vertices [18], vertices [19], Color.black);
			Debug.DrawLine (vertices [19], vertices [20], Color.black);
		}
	}

	/*void OnDrawGizmos() {
		Gizmos.color = Color.black;
		for (int i = 0; i < vertices.Length; i++) {
			Gizmos.DrawSphere (vertices [i], 0.035f);
		}
		//AddConnectionLines ();
	}*/

	void AddVerletPoint(Vector3 location, int index) {
		if (verletPoints [index] == null) {
			GameObject vertex = Instantiate (verletPointPrefab, this.transform);

			VerletPoint verletPoint = vertex.GetComponent<VerletPoint> ();
			verletPoint.location = location * scaleSize + this.transform.position;
			verletPoint.gameObject.name = "Verlet Point " + index;

			verletPoints [index] = verletPoint;
		} else {
			verletPoints [index].location = location * scaleSize + this.transform.position;
		}

	}

	void SetGoatVerletPoints() {
		// Horn
		AddVerletPoint(new Vector3 (-2.0f, 8.0f), 0);
		AddVerletPoint(new Vector3 (-3.0f, 7.0f), 1);
		AddVerletPoint(new Vector3 (-4.0f, 7.0f), 2);

		// Eye
		AddVerletPoint(new Vector3 (-5.0f, 5.5f), 3);

		// Face
		AddVerletPoint(new Vector3 (-7.0f, 5.0f), 4);
		AddVerletPoint(new Vector3 (-7.0f, 3.0f), 5);
		AddVerletPoint(new Vector3 (-6.0f, 3.5f), 6);
		AddVerletPoint(new Vector3 (-5.0f, 4.0f), 7);

		// Horn
		AddVerletPoint(new Vector3 (-5.5f, 2.0f), 8);

		// Bottom
		AddVerletPoint(new Vector3 (-4.0f, 4.0f), 9);

		// Left Leg
		AddVerletPoint(new Vector3 (2.0f, 2.0f), 10);
		AddVerletPoint(new Vector3 (2.0f, -1.0f), 11);
		AddVerletPoint(new Vector3 (2.0f, -2.5f), 12);

		// Right Leg
		AddVerletPoint(new Vector3 (5.5f, 1.0f), 13);
		AddVerletPoint(new Vector3 (5.5f, -2.0f), 14);
		AddVerletPoint(new Vector3 (5.5f, -3.5f), 15);

		// Back & tail
		AddVerletPoint(new Vector3 (7.0f, 1.5f), 16);
		AddVerletPoint(new Vector3 (7.0f, 5.0f), 17);
		AddVerletPoint(new Vector3 (8.9f, 7.0f), 18);
		AddVerletPoint(new Vector3 (6.0f, 6.0f), 19);

		AddVerletPoint (new Vector3 (-1.0f, 6.0f), 20);
	}


	void ShowBoundingBox(){
		Vector3 v3Center = boundingBox.center;
		Vector3 v3Extents = boundingBox.extents;
		Color color = Color.black;

		Vector3 v3FrontTopLeft     = new Vector3(v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z);  // Front top left corner
		Vector3 v3FrontTopRight    = new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z);  // Front top right corner
		Vector3 v3FrontBottomLeft  = new Vector3(v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z);  // Front bottom left corner
		Vector3 v3FrontBottomRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z);  // Front bottom right corner
		Vector3 v3BackTopLeft      = new Vector3(v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z + v3Extents.z);  // Back top left corner
		Vector3 v3BackTopRight        = new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z + v3Extents.z);  // Back top right corner
		Vector3 v3BackBottomLeft   = new Vector3(v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z + v3Extents.z);  // Back bottom left corner
		Vector3 v3BackBottomRight  = new Vector3(v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z + v3Extents.z);  // Back bottom right corner

		v3FrontTopLeft     = transform.TransformPoint(v3FrontTopLeft);
		v3FrontTopRight    = transform.TransformPoint(v3FrontTopRight);
		v3FrontBottomLeft  = transform.TransformPoint(v3FrontBottomLeft);
		v3FrontBottomRight = transform.TransformPoint(v3FrontBottomRight);
		v3BackTopLeft      = transform.TransformPoint(v3BackTopLeft);
		v3BackTopRight     = transform.TransformPoint(v3BackTopRight);
		v3BackBottomLeft   = transform.TransformPoint(v3BackBottomLeft);
		v3BackBottomRight  = transform.TransformPoint(v3BackBottomRight);    

		Debug.DrawLine (v3FrontTopLeft, v3FrontTopRight, color);
		Debug.DrawLine (v3FrontTopRight, v3FrontBottomRight, color);
		Debug.DrawLine (v3FrontBottomRight, v3FrontBottomLeft, color);
		Debug.DrawLine (v3FrontBottomLeft, v3FrontTopLeft, color);

		Debug.DrawLine (v3BackTopLeft, v3BackTopRight, color);
		Debug.DrawLine (v3BackTopRight, v3BackBottomRight, color);
		Debug.DrawLine (v3BackBottomRight, v3BackBottomLeft, color);
		Debug.DrawLine (v3BackBottomLeft, v3BackTopLeft, color);

		Debug.DrawLine (v3FrontTopLeft, v3BackTopLeft, color);
		Debug.DrawLine (v3FrontTopRight, v3BackTopRight, color);
		Debug.DrawLine (v3FrontBottomRight, v3BackBottomRight, color);
		Debug.DrawLine (v3FrontBottomLeft, v3BackBottomLeft, color);
	}

	/*
	public GameObject verletLinePrefab;
	public List<VerletLine> verletEdges = new List<VerletLine>();

	public Transform points;
	public Transform lines;

	public int numVertices = 21;
	public float scaleSize = 1.0f;

	void Start () {
		currentVelocity = new Vector2(speed * Mathf.Cos(elevationAngle * Mathf.Deg2Rad), speed * Mathf.Sin(elevationAngle * Mathf.Deg2Rad));
		transform.Rotate (new Vector3(0.0f, 0.0f, -1 * elevationAngle));

		transform.Translate (new Vector3 (0.0f, 0.0f, -0.6f));

		SetGoatVerletPoints ();
		AddVerletLines ();
		initialized = true;
	}

	void AddVerletPoint(Vector3 location, int index) {
		GameObject vertex = Instantiate(verletPointPrefab, points);

		VerletPoint verletPoint = vertex.GetComponent<VerletPoint> ();
		verletPoint.location = location * scaleSize + this.transform.position;
		verletPoint.gameObject.name = "Verlet Point " + index;

		vertices [index] = verletPoint;
	}

	void SetGoatVerletPoints() {
		vertices = new VerletPoint[numVertices];

		// Horn
		AddVerletPoint(new Vector3 (-2.0f, 8.0f), 0);
		AddVerletPoint(new Vector3 (-3.0f, 7.0f), 1);
		AddVerletPoint(new Vector3 (-4.0f, 7.0f), 2);

		// Eye
		AddVerletPoint(new Vector3 (-5.0f, 5.5f), 3);

		// Face
		AddVerletPoint(new Vector3 (-7.0f, 5.0f), 4);
		AddVerletPoint(new Vector3 (-7.0f, 3.0f), 5);
		AddVerletPoint(new Vector3 (-6.0f, 3.5f), 6);
		AddVerletPoint(new Vector3 (-5.0f, 4.0f), 7);

		// Horn
		AddVerletPoint(new Vector3 (-5.5f, 2.0f), 8);

		// Bottom
		AddVerletPoint(new Vector3 (-4.0f, 4.0f), 9);

		// Left Leg
		AddVerletPoint(new Vector3 (2.0f, 2.0f), 10);
		AddVerletPoint(new Vector3 (2.0f, -1.0f), 11);
		AddVerletPoint(new Vector3 (2.0f, -2.5f), 12);

		// Right Leg
		AddVerletPoint(new Vector3 (5.5f, 1.0f), 13);
		AddVerletPoint(new Vector3 (5.5f, -2.0f), 14);
		AddVerletPoint(new Vector3 (5.5f, -3.5f), 15);

		// Back & tail
		AddVerletPoint(new Vector3 (7.0f, 1.5f), 16);
		AddVerletPoint(new Vector3 (7.0f, 5.0f), 17);
		AddVerletPoint(new Vector3 (8.9f, 7.0f), 18);
		AddVerletPoint(new Vector3 (6.0f, 6.0f), 19);

		AddVerletPoint (new Vector3 (-1.0f, 6.0f), 20);
	}

	void AddVerletLine(VerletPoint start, VerletPoint end) {
		GameObject line = Instantiate (verletLinePrefab, lines);

		VerletLine verletLine = line.GetComponent<VerletLine> ();
		verletLine.startPoint = start;
		verletLine.endPoint = end;
		verletLine.totalLength = (end.location - start.location).magnitude;
		verletLine.numSegments = ((int)verletLine.totalLength * 10 > 0) ? (int)verletLine.totalLength * 10 : 2;
		verletLine.gameObject.name = "Verlet Line " + verletEdges.Count;

		verletEdges.Add (verletLine);
	}

	void AddVerletLines() {
		// Horn
		AddVerletLine (vertices [0], vertices [1]);
		AddVerletLine (vertices [0], vertices [2]);
		AddVerletLine (vertices [1], vertices [20]);
		AddVerletLine (vertices [2], vertices [4]);

		// Face
		AddVerletLine (vertices [4], vertices [5]);
		AddVerletLine (vertices [5], vertices [6]);

		AddVerletLine (vertices [6], vertices [7]);
		AddVerletLine (vertices [6], vertices [8]);

		AddVerletLine (vertices [7], vertices [8]);
		AddVerletLine (vertices [7], vertices [9]);

		// Bottom
		AddVerletLine (vertices [9], vertices [10]);

		// Left Leg
		AddVerletLine (vertices [10], vertices [11]);
		AddVerletLine (vertices [10], vertices [13]);
		AddVerletLine (vertices [11], vertices [12]);

		// Right Leg
		AddVerletLine (vertices [13], vertices [14]);
		AddVerletLine (vertices [13], vertices [16]);
		AddVerletLine (vertices [14], vertices [15]);

		// Back & tail
		AddVerletLine (vertices [16], vertices [17]);
		AddVerletLine (vertices [17], vertices [18]);
		AddVerletLine (vertices [17], vertices [19]);

		AddVerletLine (vertices [18], vertices [19]);
		AddVerletLine (vertices [19], vertices [20]);
	}*/
}
