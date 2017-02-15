using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerletLine : MonoBehaviour {

	public GameObject particlePrefab;
	public VerletPoint startPoint;
	public VerletPoint endPoint;

	public float totalLength = 10.0f;
	public int numSegments = 5;
	public int numIterations = 3;

	private List<VerletParticle> particles = new List<VerletParticle>();
	private LineRenderer line;

	private float TimeRemainder = 0f;
	private float SubstepTime = 0.02f;

	// Use this for initialization
	void Start () {
		int numParticles = numSegments + 1;

		line = GetComponent<LineRenderer> ();
		if (line == null) {
			line = gameObject.AddComponent<LineRenderer> ();
		}

		line.numPositions = numSegments;
		line.startWidth = 0.1f;
		line.endWidth = 0.1f;
		line.startColor = Color.cyan;
		line.endColor = Color.green;

		Vector3 lineVector = endPoint.location - startPoint.location;

		for (int i = 0; i < numParticles; i++) {
			GameObject newParticle = Instantiate (particlePrefab) as GameObject;

			float displacement = (float)i / (float)numParticles;
			VerletParticle particle = newParticle.GetComponent<VerletParticle> ();

			particle.transform.position = startPoint.location + displacement * lineVector;
			particle.oldPosition = startPoint.location + displacement * lineVector;
			particle.transform.parent = this.transform;
			particle.gameObject.name = "VelvetParticle " + i;
			particles.Add (particle);

			if (i == 0 || i == numParticles - 1) {
				particle.freelyMoveable = false;
			}
		}
	}
	
	// Update is called once per frame
	/*void Update () {
		VerletParticle startParticle = particles [0];
		startParticle.transform.position = startParticle.oldPosition = startPoint.location;

		VerletParticle endParticle = particles [numSegments];
		endParticle.transform.position = endParticle.oldPosition = endPoint.location;

		float UseSubstep = Mathf.Max(SubstepTime, 0.005f);

		TimeRemainder += Time.deltaTime;
		while (TimeRemainder > UseSubstep) {
			VerletIntegrate(UseSubstep);
			ResolveConstraints ();
			TimeRemainder -= UseSubstep;
		}
	}

	private void VerletIntegrate(float substepTime) {
		int numParticles = numSegments + 1;

		for (int i = 0; i < numParticles; i++) {
			VerletParticle particle = particles [i];

			if (particle.freelyMoveable) {
				Vector3 particleVelocity = particle.transform.position - particle.oldPosition;
				Vector3 particleNewPosition = particle.transform.position + particleVelocity + (substepTime * substepTime * Vector3.up * Movement.GRAVITY);

				particle.oldPosition = particleNewPosition;
				particle.transform.position = particleNewPosition;
			}
		}
	}
	
	private void ResolveConstraints() {
		float desiredSegmentDistance = totalLength / (float)numSegments;

		for (int i = 0; i < numIterations; i++) {
			for (int j = 0; j < numSegments; j++) {
				VerletParticle particleA = particles [j];
				VerletParticle particleB = particles [j + 1];

				Vector3 distanceOfParticles = particleB.transform.position - particleA.transform.position;
				float actualSegmentDistance = distanceOfParticles.magnitude;

				float error = (actualSegmentDistance - desiredSegmentDistance) / actualSegmentDistance;

				if (particleA.freelyMoveable && particleB.freelyMoveable) {
					particleA.transform.position += error * 0.5f * distanceOfParticles;
					particleB.transform.position -= error * 0.5f * distanceOfParticles;
				} else if (particleA.freelyMoveable) {
					particleA.transform.position += error * distanceOfParticles;
				} else if (particleB.freelyMoveable) {
					particleA.transform.position -= error * distanceOfParticles;
				}

				line.SetPosition (j, particleA.transform.position);
			}
		}
	}*/

	/*void Start() {
		int NumParticles = numSegments + 1;
		//Particles.Clear();

		// Use linerenderer as visual cable representation
		line = transform.GetComponent<LineRenderer>();
		if (line == null) {
			line = gameObject.AddComponent<LineRenderer>();
		}
		line.SetVertexCount(numSegments);
		line.SetWidth(.2f, .2f);
		line.SetColors(Color.cyan, Color.blue);

		Vector3 Delta = startPoint.location - endPoint.location;

		for (int ParticleIndex = 0; ParticleIndex < NumParticles; ParticleIndex++) {
			Transform newTransform = Instantiate(particlePrefab, Vector3.zero, Quaternion.identity).transform;

			float Alpha = (float)ParticleIndex / (float)NumParticles;
			Vector3 InitializePosition = startPoint.transform.position + (Alpha * Delta);

			VerletParticle particle = newTransform.GetComponent<VerletParticle>();
			particle.transform.position = InitializePosition;
			particle.oldPosition = InitializePosition;
			particle.transform.parent = this.transform;
			particle.gameObject.name = "VelvetParticle " + ParticleIndex;
			particles.Add(particle);

			if (ParticleIndex == 0 || ParticleIndex == (NumParticles - 1)) {
				particle.freelyMoveable = false;
			} else {
				particle.freelyMoveable = true;
			}
		}
	}*/

	void Update() {
		// Update start+end positions first
		VerletParticle StartParticle = particles[0];
		StartParticle.transform.position = StartParticle.oldPosition = startPoint.location;

		VerletParticle EndParticle = particles[numSegments];
		EndParticle.transform.position = EndParticle.oldPosition = endPoint.location;

		Vector3 Gravity = Physics.gravity;
		float UseSubstep = Mathf.Max(SubstepTime, 0.005f);

		TimeRemainder += Time.deltaTime;
		while (TimeRemainder > UseSubstep) {
			PreformSubstep(UseSubstep, Gravity);
			TimeRemainder -= UseSubstep;
		}
	}

	private void PreformSubstep(float InSubstepTime, Vector3 Gravity) {
		VerletIntegrate(InSubstepTime, Gravity);
		SolveConstraints();
	}

	private void VerletIntegrate(float InSubstepTime, Vector3 Gravity) {
		int numParticles = numSegments + 1;
		float SubstepTimeSqr = InSubstepTime * InSubstepTime;

		for (int ParticleIndex = 0; ParticleIndex < numParticles; ParticleIndex++) {
			VerletParticle particle = particles[ParticleIndex];
			if (particle.freelyMoveable) {
				Vector3 Velocity = particle.transform.position - particle.oldPosition;
				Vector3 NewPosition = particle.transform.position + Velocity + (SubstepTimeSqr * Gravity);

				particle.oldPosition = particle.transform.position;
				particle.transform.position = NewPosition;
			}
		}
	}

	private void SolveConstraints() {
		float SegmentLength = totalLength/(float)numSegments;

		// For each iteration
		for (int IterationIndex = 0; IterationIndex < numIterations; IterationIndex++) {
			// For each segment
			for (int SegmentIndex = 0; SegmentIndex < numSegments; SegmentIndex++) {
				VerletParticle ParticleA = particles[SegmentIndex];
				VerletParticle ParticleB = particles[SegmentIndex + 1];
				// Solve for this pair of particles
				SolveDistanceConstraint(ParticleA, ParticleB, SegmentLength);

				// Update render position
				line.SetPosition(SegmentIndex, ParticleA.transform.position);
				//Debug.Log ("Drawed segment " + SegmentIndex);
			}
		}
	}

	void SolveDistanceConstraint(VerletParticle ParticleA, VerletParticle ParticleB, float DesiredDistance) {
		// Find current difference between particles
		Vector3 Delta = ParticleB.transform.position - ParticleA.transform.position;
		float CurrentDistance = Delta.magnitude;
		float ErrorFactor = (CurrentDistance - DesiredDistance) / CurrentDistance;

		// Only move free particles to satisfy constraints
		if (ParticleA.freelyMoveable && ParticleB.freelyMoveable) {
			ParticleA.transform.position += ErrorFactor * 0.5f * Delta;
			ParticleB.transform.position -= ErrorFactor * 0.5f * Delta;
		} else if (ParticleA.freelyMoveable) {
			ParticleA.transform.position += ErrorFactor * Delta;
		} else if (ParticleB.freelyMoveable) {
			ParticleB.transform.position -= ErrorFactor * Delta;
		}
	}
}
