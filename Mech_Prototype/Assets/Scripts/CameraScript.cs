using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
	public Transform lookAt;
	public Transform camTransform;

	public float distance = 10f;
	public float flyAngle = 35f;
	public float flyHeight = -0.7f;

    private Vector3 angles;
    private Vector3 dir;
	private Vector3 hDir;

	void Start () {
        camTransform = transform;
        dir = Vector3.zero;
		hDir = Vector3.zero;
	}

	void LateUpdate () {
        dir.z = -distance;
		hDir.y = flyHeight;

        angles = lookAt.rotation.eulerAngles;

		Quaternion rotation = Quaternion.Euler (flyAngle, angles.y, 0);
		camTransform.position = lookAt.position + rotation * dir;

		camTransform.LookAt (lookAt.position - hDir);
	}
}
