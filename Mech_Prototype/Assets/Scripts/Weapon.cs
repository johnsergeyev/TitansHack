using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapon : MonoBehaviour, IWeapon {
	public GameObject bullet;
	public float shootDistance = 100f;
 	public float shootSpeed = 5f;

	[HideInInspector]
	public Animator animator;
	[HideInInspector]
	public Transform shootingPoint;

	protected bool isFingerOnTheTrigger = false;

	protected void Shoot() {
		GameObject _bullet = (GameObject)Instantiate (bullet, shootingPoint.position, shootingPoint.rotation);
		(_bullet.GetComponent<BulletController> ()).distance = shootDistance;
		(_bullet.GetComponent<BulletController> ()).speed = shootSpeed;
		(_bullet.GetComponent<BulletController> ()).initial = shootingPoint;
	}

	public virtual void OnTriggerDown() {
		isFingerOnTheTrigger = true;
	}

	public virtual void OnTriggerUp() {
		isFingerOnTheTrigger = false;
	}

	public virtual void CheckShoot() {
		
	}
}
