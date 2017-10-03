using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	public float movementSpeed = 1;
	public float turningSpeed = 10;


	void Update() {
		float Horizontal = Input.GetAxis("Horizontal") * turningSpeed * Time.deltaTime;
		transform.Rotate(0, Horizontal, 0);

		float Vertical = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
		transform.Translate(0, 0, Vertical);

	}
}