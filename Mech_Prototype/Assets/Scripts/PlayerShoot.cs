using UnityEngine;
using System.Collections;

public class PlayerShoot : MonoBehaviour {
	
	public GameObject bullet;
	public float timeBetweenShots = 0.3333f;  // Allow 3 shots per second

	private float timestamp;

	void Update () 
	{
		if (Time.time >= timestamp && (Input.GetKeyDown(KeyCode.Space)) )
		{
			Instantiate(bullet, transform.position, transform.rotation);
			timestamp = Time.time + timeBetweenShots;
		}
	}
}
