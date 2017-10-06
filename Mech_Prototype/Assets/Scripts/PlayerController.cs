using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	public JoystickController joystick;
	public SwipeController swipe;
	public Transform body;
	private Animator animator;

	[Range(-1f, 1f)]
	public float emulateFront = 0f;
	[Range(-1f, 1f)]
	public float emulateSide = 0f;
	[Range(-1f, 1f)]
	public float emulateRotation = 0f;

	public bool animationBasedOnJoystick = false;
	public bool useInertion = true;

	public float movementInertionKf = 0.07f;
	public float rotationInertionKf= 0.07f;

	public float movementSpeed = 2f;
	public float rotationSpeed = 20f;
	public float bodyRotationSpeed = 20f;

	public float bodyAngleDelta = 20.0f;

	public bool bodyHardLock = true;
	public bool easeRotation = false;

	private Vector3 movementVector;
	private Vector3 iMovementVector;
	private Vector3 rotationVector;
	private Vector3 iRotationVector;

	private Vector3 tmp;
	private float delta;
	private float legsAngle;
	private float bodyAngle;

	private Vector3 lastPos;
	private Vector3 actualSpeed = Vector3.zero; 

	private void Start() {
		animator = GetComponent<Animator> ();

		movementVector = Vector3.zero;
		iMovementVector = Vector3.zero;
		rotationVector = Vector3.zero;
		iRotationVector = Vector3.zero;

		lastPos = transform.position;

		StartCoroutine ("CalcSpeed");
		StartCoroutine ("CalcInertion");
	}

	private Vector3 calculateInertion(Vector3 oldVector, Vector3 newVector, float inertion, 
		bool x, bool y, bool z) {
		if (x) oldVector.x = calculateDelta (oldVector.x, newVector.x, inertion);
		if (y) oldVector.y = calculateDelta (oldVector.y, newVector.y, inertion);
		if (z) oldVector.z = calculateDelta (oldVector.z, newVector.z, inertion);

		return oldVector;
	}

	private float calculateDelta(float oldF, float newF, float delta) {
		if (Mathf.Abs (newF - oldF) > delta) {
			if (newF < oldF) {
				oldF -= delta;
			} else {
				oldF += delta;
			}
		} else {
			oldF = newF;
		}

		return oldF;
	}

	private void calculateAngles()
	{
		tmp = transform.rotation.eulerAngles;
		legsAngle = tmp.y;

		tmp = body.transform.rotation.eulerAngles;
		bodyAngle = tmp.y;
	}

	private void Update() {
		if (!joystick)
			return;

		if (!swipe)
			return;

		movementVector.x = joystick.Horizontal();
		movementVector.z = joystick.Vertical();
		rotationVector.y = swipe.Value();

		if (!emulateFront.Equals(0f))
			movementVector.z = emulateFront;

		if (!emulateSide.Equals(0f))
			movementVector.x = emulateSide;

		if (!emulateRotation.Equals(0f))
			rotationVector.y = emulateRotation;

		if (animationBasedOnJoystick) {
			animator.SetFloat ("speed", movementVector.z);
			animator.SetFloat ("direction", movementVector.x);
		} else {
			animator.SetFloat ("speed", actualSpeed.z / movementSpeed);
			animator.SetFloat ("direction", actualSpeed.x / movementSpeed);
		}

		lastPos = transform.position;
	}

	private float rSpeed;

	private bool isMoving() {
		return movementVector != Vector3.zero;
	}

	private bool hardLockMode;

	private void FixedUpdate() {
		calculateAngles();

		rSpeed = 0;
		tmp = Vector3.zero;
		hardLockMode = false;

		if (bodyAngle != legsAngle && 
			((!isMoving() && bodyAngleDelta != -1f) 
				|| (isMoving() && bodyAngleDelta != -1f)
				|| bodyAngleDelta == 0.0f 
				|| (isMoving() && bodyAngleDelta == -1f))
		) {
			delta = Mathf.DeltaAngle(legsAngle, bodyAngle);

			if (bodyAngleDelta == -1f || (Mathf.Abs(delta) > bodyAngleDelta) || (bodyAngleDelta != -1f && isMoving())) { 
				if (bodyAngleDelta != -1f && (Mathf.Abs(delta) > bodyAngleDelta)) {
					if (bodyHardLock) hardLockMode = true;

					if (delta < 0)
						delta += bodyAngleDelta;
					else
						delta -= bodyAngleDelta; 
				}

				rSpeed = bodyRotationSpeed * Time.fixedDeltaTime;
				if (easeRotation)
					rSpeed = rSpeed * movementVector.magnitude;

				if (Mathf.Abs (delta) < rSpeed) {
					rSpeed = Mathf.Abs (delta) / Time.fixedDeltaTime;
				} else {
					rSpeed = easeRotation ? (rotationSpeed * movementVector.magnitude) : rotationSpeed;
				}

				if (delta < 0) {
					tmp.y = -rSpeed;
				} else {
					tmp.y = rSpeed;
				}
			}
		}
			
		body.Rotate (((useInertion?iRotationVector:rotationVector) * (hardLockMode ? rotationSpeed : bodyRotationSpeed) - tmp) * Time.fixedDeltaTime);
		transform.Rotate (tmp * Time.fixedDeltaTime);
		transform.Translate ((useInertion?iMovementVector:movementVector) * movementSpeed * Time.fixedDeltaTime);
	}

	IEnumerator CalcInertion() {
		while (Application.isPlaying) {
			yield return new WaitForEndOfFrame ();

			if (useInertion) {
				iMovementVector = calculateInertion (iMovementVector, movementVector, movementInertionKf, true, false, true);
				iRotationVector = calculateInertion (iRotationVector, rotationVector, rotationInertionKf, false, true, true);

				//iMovementVector = Vector3.Lerp (iMovementVector, movementVector, movementInertionKf);
				//iRotationVector = Vector3.Lerp (iRotationVector, rotationVector, movementInertionKf);
			}
		}
	}

	IEnumerator CalcSpeed() {
		while (Application.isPlaying) {
			lastPos = transform.position;

			yield return new WaitForFixedUpdate ();

			actualSpeed = (transform.position - lastPos) / Time.fixedDeltaTime;
			actualSpeed = transform.InverseTransformDirection (actualSpeed);
		}
	}
}
