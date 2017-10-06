using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapon : MonoBehaviour, IWeapon {
	public GameObject bullet;
	public float shootDistance = 100f;
 	public float shootSpeed = 5f;
	public GameObject decal;
	public GameObject hitVfx;
	public GameObject shootVfx;
	public float hitVfxTtl = 2f;
	public float shootVfxTtl = 2f;

	public float cooldown = 1f;
	protected float tick = -1f;

	[HideInInspector]
	public Animator animator;
	[HideInInspector]
	public Transform shootingPoint;

	protected bool isFingerOnTheTrigger = false;
	private DecalController dc;

	protected void Shoot() {
		dc = GameObject.FindObjectOfType<DecalController> ();

		tick = 0.0f;
		GameObject _bullet = (GameObject)Instantiate (bullet, shootingPoint.position, shootingPoint.rotation);

		if (shootVfx != null) {
			GameObject _vfx = (GameObject)Instantiate (shootVfx, shootingPoint.position, shootVfx.transform.rotation);
			ParticleSystem _vfxPS = _vfx.GetComponent<ParticleSystem> ();
			_vfxPS.transform.parent = shootingPoint.transform;
			_vfxPS.Play ();
		}

		(_bullet.GetComponent<BulletController> ()).distance = shootDistance;
		(_bullet.GetComponent<BulletController> ()).speed = shootSpeed;
		(_bullet.GetComponent<BulletController> ()).initial = shootingPoint;
		(_bullet.GetComponent<BulletController> ()).decal = decal;
		(_bullet.GetComponent<BulletController> ()).vfx = hitVfx;
		(_bullet.GetComponent<BulletController> ()).vfx_ttl = hitVfxTtl;
	}

	public virtual void OnTriggerDown() {
		isFingerOnTheTrigger = true;
	}

	public virtual void OnTriggerUp() {
		isFingerOnTheTrigger = false;
	}

	public virtual void CheckShoot() {
		if (tick == -1f)
			tick = cooldown;
		tick += Time.deltaTime;
	}

	public virtual bool ReadyToShoot() {
		return tick >= cooldown;
	}
}
