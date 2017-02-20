using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goat : Shootable {

	public Vector3[] vertices;
	public int numVertices = 21;

	public GameObject verletPointPrefab;
	public VerletPoint[] verletPoints;

	public GameObject verletLinkPrefab;
	public List<VerletLink> verletLinks = new List<VerletLink>();

	public Transform points;
	public Transform links;

	public float scaleSize = 0.1f;

	private const int numIterations = 12;
	private float TimeRemainder = 0f;
	private float SubstepTime = 0.02f;

	// Use this for initialization
	void Start () {
		currentVelocity = new Vector2(-1.0f * speed * Mathf.Cos(elevationAngle * Mathf.Deg2Rad), speed * Mathf.Sin(elevationAngle * Mathf.Deg2Rad));
		transform.Rotate (new Vector3(0.0f, 0.0f, -1.0f * elevationAngle));
		transform.Translate (new Vector3 (0.0f, 0.0f, -0.6f));

		vertices = new Vector3[numVertices];
		verletPoints = new VerletPoint[numVertices];

		// Draw the verlet shape and create the connections
		SetGoatShapeVertices ();
		SetGoatVerletPoints ();
		AddVerletLinks();

		// Give initial velocity to 8 vertices (head area)
		for (int i = 0; i < 8; i++) {
			verletPoints [i].ApplyForce (currentVelocity / Time.deltaTime);
		}

		initialized = true;
	}

	// Update is called once per frame
	void Update () {
		//float UseSubstep = Mathf.Max(SubstepTime, 0.005f);

		//TimeRemainder += Time.deltaTime;
		//while (TimeRemainder > UseSubstep) {
		if (!pinned) {
			for (int i = 0; i < verletPoints.Length; i++) {
				Vector2 drag = (verletPoints [i].transform.position - verletPoints [i].oldPosition).normalized * -0.5f * Movement.AIR_DENSITY * Movement.CrossSectionalArea (radius) * (verletPoints [i].transform.position - verletPoints [i].oldPosition).magnitude * (verletPoints [i].transform.position - verletPoints [i].oldPosition).magnitude;
				Vector2 forceApplied = (drag + Movement.GRAVITY_VECTOR + windForce);
				verletPoints [i].UpdatePhysics (forceApplied, Time.deltaTime);
			}
		}

		for (int i = 0; i < numIterations; i++) {
			for (int j = 0; j < verletPoints.Length; j++) {
				verletPoints [j].SolveConstraints ();

				if (verletPoints [j].pinned) {
					pinned = true;
				}
			}
		}
		//TimeRemainder -= UseSubstep;
	}

	// Method to compute the vertex locatins for the Vector3 array that give a goat shape
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

	// Helper Function to add a verlet point at the given Vector3 location, setting the index index at the array
	void AddVerletPoint(Vector3 location, int index) {
		if (verletPoints [index] == null) {
			GameObject vertex = Instantiate (verletPointPrefab, points);

			// Create a verlet point object at the specified location
			VerletPoint verletPoint = vertex.GetComponent<VerletPoint> ();
			verletPoint.transform.position = location * scaleSize + this.transform.position;
			verletPoint.oldPosition = location * scaleSize + this.transform.position;
			verletPoint.gameObject.name = "Verlet Point " + index;

			// Legs are sticky (not used after prof said goats can stick to the mountain at any colliding vertex)
			if (index == 11 || index == 12 || index == 13 || index == 14) {
				verletPoint.isSticky = true;
			}

			verletPoints [index] = verletPoint;
		} else {
			verletPoints [index].transform.position = location * scaleSize + this.transform.position;
		}

	}

	// Method to compute the vertex locatins for the VerletPoint array that give a goat shape
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

	// Method to add a verlet link between two VerletPoint objects.
	// Can be set drawable or not.
	void AddVerletLink(VerletPoint start, VerletPoint end, bool draw) {
		bool newLink = true;

		// Check to make sure no duplicate links
		foreach(VerletLink link in start.links) {
			if ((link.pointA == start && link.pointB == end) || (link.pointA == end && link.pointB == start)) {
				newLink = false;
			}
		}

		if (newLink) {
			GameObject link = Instantiate (verletLinkPrefab, links);

			// Set appropriate properties of the link and add it to both points' list of links
			VerletLink verletLink = link.GetComponent<VerletLink> ();
			verletLink.pointA = start;
			verletLink.pointB = end;
			verletLink.initialDistance = end.transform.position - start.transform.position;
			verletLink.drawThisLink = draw;

			start.links.Add (verletLink);
			end.links.Add (verletLink);

			verletLinks.Add (verletLink);
		}
	}

	// Law of cosines link: If a verlet point is connected to 2 other points, then those 2 points must also
	// preserve their initial distance
	void AddLawOfCosinesLink(VerletPoint point) {
		if (point.links.Count == 2) {
			VerletPoint neighbor1 = point.links [0].GiveNeighbor (point);
			VerletPoint neighbor2 = point.links [1].GiveNeighbor (point);
			AddVerletLink (neighbor1, neighbor2, false);
		}
	}

	// Method to add appropriate Verlet links to ensure goat shape is drawn
	void AddVerletLinks() {
		// Horn
		AddVerletLink (verletPoints [0], verletPoints [1], true);
		AddVerletLink (verletPoints [0], verletPoints [2], true);
		AddVerletLink (verletPoints [1], verletPoints [20], true);
		AddVerletLink (verletPoints [2], verletPoints [4], true);

		// Face
		AddVerletLink (verletPoints [4], verletPoints [5], true);
		AddVerletLink (verletPoints [5], verletPoints [6], true);

		AddVerletLink (verletPoints [6], verletPoints [7], true);
		AddVerletLink (verletPoints [6], verletPoints [8], true);

		AddVerletLink (verletPoints [7], verletPoints [8], true);
		AddVerletLink (verletPoints [7], verletPoints [9], true);

		// Bottom
		AddVerletLink (verletPoints [9], verletPoints [10], true);

		// Left Leg
		AddVerletLink (verletPoints [10], verletPoints [11], true);
		AddVerletLink (verletPoints [10], verletPoints [13], true);
		AddVerletLink (verletPoints [11], verletPoints [12], true);

		// Right Leg
		AddVerletLink (verletPoints [13], verletPoints [14], true);
		AddVerletLink (verletPoints [13], verletPoints [16], true);
		AddVerletLink (verletPoints [14], verletPoints [15], true);

		// Back & tail
		AddVerletLink (verletPoints [16], verletPoints [17], true);
		AddVerletLink (verletPoints [17], verletPoints [18], true);
		AddVerletLink (verletPoints [17], verletPoints [19], true);

		AddVerletLink (verletPoints [18], verletPoints [19], true);
		AddVerletLink (verletPoints [19], verletPoints [20], true);

		// Eye (must be invisible links)
		AddVerletLink (verletPoints [3], verletPoints [2], false);
		AddVerletLink (verletPoints [3], verletPoints [4], false);

		// Extra links drawn to ensure the shape is not grossly distorted
		for (int i = 0; i < verletPoints.Length; i++) {
			for (int j = i+1; j < verletPoints.Length; j++) {
				if (verletPoints [i].links.Count < 7 && verletPoints [j].links.Count < 7) {
					AddVerletLink (verletPoints [i], verletPoints [j], false);
				}
			}
		}
	}

}
