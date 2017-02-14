using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBounds2D : MonoBehaviour {

	[System.Serializable]
	public class Bounds2D {
		public Vector3 topLeft;
		public Vector3 topRight;
		public Vector3 bottomLeft;
		public Vector3 bottomRight;
	}

	public static Bounds2D GetBounds(Mesh mesh) {
		Bounds bounds = mesh.bounds;

		Vector2 center = bounds.center;
		Vector2 extents = bounds.extents;

		Bounds2D bounds2D = new Bounds2D ();;
		bounds2D.topLeft = new Vector3 (center.x - extents.x, center.y + extents.y);
		bounds2D.topRight = new Vector3 (center.x + extents.x, center.y + extents.y);
		bounds2D.bottomLeft = new Vector3 (center.x - extents.x, center.y - extents.y);
		bounds2D.bottomRight = new Vector3 (center.x + extents.x, center.y - extents.y);

		return bounds2D;
	}
}
