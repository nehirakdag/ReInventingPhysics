using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public Vector2 minBounds;
	public Vector2 maxBounds;

	public Vector2 mountainMinBounds;
	public Vector2 mountainMaxBounds;

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
		// Functions check if x-y coordinates of each cannon's shots have gone out of bounds
		DestroyOutOfBoundsOrStationaryShots (leftCannon);
		DestroyOutOfBoundsOrStationaryShots (rightCannon);

		timer += Time.deltaTime;

		// Change wind every windChangeTime seconds (0.5s in our case)
		if (timer > windChangeTime) {
			ApplyWind ();
			timer = 0.0f;
		}

		// Check for cannonballs' collisions with the mountain
		foreach (Shootable shot in leftCannon.shotsFired) {
			// Get respective vertex locations of both the mountain and the cannonball
			Vector3[] verticesA = mountain.GetComponent<MeshFilter> ().mesh.vertices;
			Vector2[] verticesB = new Vector2[shot.GetComponent<SpriteRenderer> ().sprite.vertices.Length];

			// Since cannonballs are 2D sprites, their vertex locations must be updated via the transform's current location.
			// Otherwise we'd always get the same points indicating the orientation and not the actual world position
			for (int i = 0; i < verticesB.Length; i++) {
				Vector2 spriteWorldPosition = shot.GetComponent<SpriteRenderer> ().transform.TransformPoint(shot.GetComponent<SpriteRenderer> ().sprite.vertices[i]);
				verticesB [i] = new Vector2 (spriteWorldPosition.x, spriteWorldPosition.y);
			}

			// Start looking for collisions only if the bounding boxes of two objects intersect
			if (mountain.GetComponent<MeshFilter> ().mesh.bounds.Intersects (shot.GetComponent<SpriteRenderer> ().bounds)) {
				// Check for collisions
				Simplex collisionSimplex = physics.DetectCollision (verticesA, verticesB);

				// If encountered a collision, handle it
				if (collisionSimplex != null) {
					collisionSimplex = physics.HandleCollision (verticesA, verticesB, collisionSimplex);

					// Apply appropriate response
					shot.transform.Translate(new Vector3(-1.0f * collisionSimplex.penetratingDistance * shot.currentVelocity.normalized.x, -1.0f * collisionSimplex.penetratingDistance * shot.currentVelocity.normalized.y, 0.0f));
					shot.currentVelocity += (shot.bounciness * shot.currentVelocity.magnitude * collisionSimplex.collisionNormal.normalized);

				}
			}
		}

		// Check for goats' collisions with the mountain
		foreach (Shootable shot in rightCannon.shotsFired) {
			// Get respective vertex locations of both the mountain and the goat
			Vector3[] verticesA = mountain.GetComponent<MeshFilter> ().mesh.vertices;
			Vector2[] verticesB = new Vector2[shot.GetComponent<Goat>().vertices.Length];

			// Start looking for collisions only if the bounding boxes of two objects intersect
			bool intersection = false;
			for (int i = 0; i < shot.GetComponent<Goat>().verletPoints.Length; i++) {
				verticesB [i] = new Vector2 (shot.GetComponent<Goat>().verletPoints[i].transform.position.x, shot.GetComponent<Goat>().verletPoints[i].transform.position.y);

				if (mountainMinBounds.x < verticesB[i].x && mountainMinBounds.y < verticesB[i].y && 
					mountainMaxBounds.x > verticesB[i].x && mountainMaxBounds.y > verticesB[i].y) {
					intersection = true;
				}
			}

			// Goat can be pinned to the mountain. So no need to detect collisions if they are pinned
			if(intersection&& !shot.pinned) {
				// Check for collisions
				Simplex collisionSimplex = physics.DetectCollision (verticesA, verticesB);

				// If encountered a collision, handle it
				if (collisionSimplex != null) {
					collisionSimplex = physics.HandleCollision (verticesA, verticesB, collisionSimplex);

					// Apply appropriate response
					Vector3 shotVelocity = shot.GetComponent<Goat>().verletPoints[0].transform.position - shot.GetComponent<Goat>().verletPoints[0].oldPosition;
					shot.transform.Translate(new Vector3(collisionSimplex.penetratingDistance * shotVelocity.normalized.x, -1.0f * collisionSimplex.penetratingDistance * shotVelocity.normalized.y, 0.0f));
					shot.pinned = true;

				}
			}
		}

		// Check for cannonballs' collisions with goats
		for(int index = 0; index < leftCannon.shotsFired.Count; index++) {
			Shootable cannonball = leftCannon.shotsFired[index];

			// No need to check for collisions if the cannonball is near stationary
			if (cannonball.currentVelocity.magnitude > 0.1f) {
				foreach (Shootable goat in rightCannon.shotsFired) {
					// Check if goat's shape is initialized to make sure no early checks
					if (goat.initialized) {
						// Get respective vertex locations of both the mountain and the goat
						Vector3[] verticesA = new Vector3[cannonball.GetComponent<SpriteRenderer> ().sprite.vertices.Length];
						Vector2[] verticesB = new Vector2[goat.GetComponent<Goat> ().vertices.Length];

						// Since cannonballs are 2D sprites, their vertex locations must be updated via the transform's current location.
						// Otherwise we'd always get the same points indicating the orientation and not the actual world position
						for (int i = 0; i < verticesA.Length; i++) {
							Vector2 spriteWorldPosition = cannonball.GetComponent<SpriteRenderer> ().transform.TransformPoint (cannonball.GetComponent<SpriteRenderer> ().sprite.vertices [i]);
							verticesA [i] = new Vector3 (spriteWorldPosition.x, spriteWorldPosition.y, -0.1f);
						}

						for (int i = 0; i < goat.GetComponent<Goat> ().verletPoints.Length; i++) {
							verticesB [i] = new Vector2 (goat.GetComponent<Goat> ().verletPoints [i].transform.position.x, goat.GetComponent<Goat> ().verletPoints [i].transform.position.y);
						}

						// Check for collisions
						Simplex collisionSimplex = physics.DetectCollision (verticesA, verticesB);

						// If encountered a collision, handle it
						if (collisionSimplex != null) {
							goat.pinned = false;

							for (int i = 0; i < goat.GetComponent<Goat> ().verletPoints.Length; i++) {
								// * 4.0f just to look more realistic since both have unit mass, otherwise almost no change in trajectory of the goat
								goat.GetComponent<Goat> ().verletPoints [i].ApplyForce (4.0f * cannonball.currentVelocity / Time.deltaTime);
							}

							// Destroy the cannonball as required
							leftCannon.shotsFired.Remove (cannonball);
							Destroy (cannonball.gameObject);
						}
					}


				}
			}
		}

		// Change actively shooting cannonball when tab key is pressed
		if (Input.GetKeyDown (KeyCode.Tab)) {
			if (leftCannon.isActive) {
				leftCannon.isActive = false;
				rightCannon.isActive = true;
			} else {
				leftCannon.isActive = true;
				rightCannon.isActive = false;
			}
		}

	}

	// Destroy any cannonballs or goats that have gone out of bounds.
	// Destroy any cannonballs that have stopped moving since destroyStationarySeconds
	void DestroyOutOfBoundsOrStationaryShots(Cannon cannon) {
		for(int i = 0; i < cannon.shotsFired.Count; i++) {
			Shootable shot = cannon.shotsFired [i];

			/// Check for cannonballs
			if (shot.transform.position.x < minBounds.x || shot.transform.position.x > maxBounds.x ||
			    shot.transform.position.y < minBounds.y || shot.transform.position.y > maxBounds.y ||
				shot.notMovingSince >= destroyStationarySeconds) {
				cannon.shotsFired.Remove (shot);
				Destroy (shot.gameObject);
			}

			// Check for goats, following statement will only be true if the shot is a goat
			Goat goat = shot.gameObject.GetComponent<Goat> ();
			if (goat != null) {
				for(int k = 0; k < goat.vertices.Length; k++) {
					if ((goat.verletPoints [k].pinned && goat.verletPoints [k].oldPosition.y <= -5.1f) ||
						goat.verletPoints [k].transform.position.x < minBounds.x || goat.verletPoints [k].transform.position.x > maxBounds.x ||
						goat.verletPoints [k].transform.position.y < minBounds.y || goat.verletPoints [k].transform.position.y > maxBounds.y) {
						cannon.shotsFired.Remove (shot);
						Destroy (shot.gameObject);
					}
				}
			}
		}
	}

	// Since wind force is take in to account during the class' own update function, signal the new value
	void SetWindForShootables(Cannon cannon) {
		foreach (Shootable s in cannon.shotsFired) {
			s.windForce = currentWind;
		}
	}

	// Calculate and signal the new wind value to the shootables in play, update UI
	void ApplyWind() {
		currentWind = new Vector2(Movement.GetCurrentWindSpeed (windSpeedMax), 0.0f);

		windSpeedArrow.localScale = (windSpeedMax == 0.0f) ? Vector3.zero : Vector3.one * currentWind.x / windSpeedMax;
		windSpeedText.text = "Wind: " + currentWind.x;

		SetWindForShootables (leftCannon);
		SetWindForShootables (rightCannon);
	}
}
