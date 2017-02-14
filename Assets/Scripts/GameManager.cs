using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public Vector2 minBounds;
	public Vector2 maxBounds;

	public Cannon leftCannon;
	public Cannon rightCannon;

	public GameObject mountain;

	public float windSpeedMax;
	public float destroyStationarySeconds = 2.0f;

	public Text windSpeedText;
	public Transform windSpeedArrow;

	private Vector2 currentWind;

	private float timer = 0.5f;
	private float windChangeTime = 0.5f;

	private PhysicsHandler physics;

	// Use this for initialization
	void Start () {
		physics = GetComponent<PhysicsHandler> ();
	}
	
	// Update is called once per frame
	void Update () {
		DestroyOutOfBoundsOrStationaryShots (leftCannon);
		DestroyOutOfBoundsOrStationaryShots (rightCannon);

		timer += Time.deltaTime;

		if (timer > windChangeTime) {
			ApplyWind ();
			timer = 0.0f;
		}

		foreach (Shootable shot in leftCannon.shotsFired) {
			if (mountain.GetComponent<MeshFilter> ().mesh.bounds.Intersects (shot.GetComponent<SpriteRenderer> ().bounds)) {
				Simplex collisionSimplex = physics.DetectMeshSpriteCollision (mountain.GetComponent<MeshFilter> ().mesh, shot.GetComponent<SpriteRenderer> ());

				if (collisionSimplex != null) {
					collisionSimplex = physics.HandleCollision (mountain.GetComponent<MeshFilter> ().mesh, shot.GetComponent<SpriteRenderer> (), collisionSimplex);

					shot.transform.Translate(new Vector3(-1.0f * collisionSimplex.penetratingDistance * shot.currentVelocity.normalized.x, -1.0f * collisionSimplex.penetratingDistance * shot.currentVelocity.normalized.y, 0.0f));
					shot.currentVelocity += (shot.bounciness * shot.currentVelocity.magnitude * collisionSimplex.collisionNormal.normalized);

				}
			}
		}

	}

	void DestroyOutOfBoundsOrStationaryShots(Cannon cannon) {
		for(int i = 0; i < cannon.shotsFired.Count; i++) {
			Shootable shot = cannon.shotsFired [i];
			if (shot.transform.position.x < minBounds.x || shot.transform.position.x > maxBounds.x ||
			    shot.transform.position.y < minBounds.y || shot.transform.position.y > maxBounds.y ||
				shot.notMovingSince >= destroyStationarySeconds) {
				cannon.shotsFired.Remove (shot);
				Destroy (shot.gameObject);
			}
		}
	}

	void SetWindForShootables(Cannon cannon) {
		foreach (Shootable s in cannon.shotsFired) {
			s.windForce = currentWind;
		}
	}

	void ApplyWind() {
		currentWind = new Vector2(Movement.GetCurrentWindSpeed (windSpeedMax), 0.0f);
		//currentWind = new Vector2(windSpeedMax, 0.0f);

		windSpeedArrow.localScale = Vector3.one * currentWind.x / windSpeedMax;
		windSpeedText.text = "Wind: " + currentWind.x;

		SetWindForShootables (leftCannon);
		SetWindForShootables (rightCannon);
	}
}
