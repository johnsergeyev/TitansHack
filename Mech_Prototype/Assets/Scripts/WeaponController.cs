using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour {
	public Transform shootingPoint;
	public Weapon mainWeapon;
	public Weapon secondaryWeapon;
	private Animator animator;

	void OnEnable () {
		EventManager.StartListening ("onFireButtonDownMain", onFireDownMain);
		EventManager.StartListening ("onFireButtonDownSecondary", onFireDownSecondary);
		EventManager.StartListening ("onFireButtonUpMain", onFireUpMain);
		EventManager.StartListening ("onFireButtonUpSecondary", onFireUpSecondary);
	}

	void OnDisable() {
		EventManager.StopListening ("onFireButtonDownMain", onFireDownMain);
		EventManager.StopListening ("onFireButtonDownSecondary", onFireDownSecondary);
		EventManager.StopListening ("onFireButtonUpMain", onFireUpMain);
		EventManager.StopListening ("onFireButtonUpSecondary", onFireUpSecondary);
	}
		
	private void onFireDownMain() {
		onFireDown (mainWeapon);
	}
		
	private void onFireDownSecondary() {
		onFireDown (secondaryWeapon);
	}

	private void onFireUpMain() {
		onFireUp (mainWeapon);
	}

	private void onFireUpSecondary() {
		onFireUp (secondaryWeapon);
	}

	private void onFireDown(Weapon weapon) {
		if (!animator) {
			animator = GetComponent<Animator> ();
		}

		if (!weapon)
			return;

		weapon.shootingPoint = shootingPoint;

		weapon.OnTriggerDown ();
		animator.SetBool ("shooting", true);
	}

	private void onFireUp(Weapon weapon) {
		if (!weapon)
			return;

		weapon.OnTriggerUp ();
		animator.SetBool ("shooting", false);
	}

	void Update() {
		if (mainWeapon)
			mainWeapon.CheckShoot ();

		if (secondaryWeapon)
			secondaryWeapon.CheckShoot ();

		Debug.DrawRay (Camera.main.transform.position, Camera.main.transform.forward * 100f);

		RaycastHit hit;

		if (Physics.Raycast (Camera.main.transform.position, Camera.main.transform.forward, out hit)) {
			if (hit.collider.gameObject.tag == "Hittable") {
				shootingPoint.LookAt(hit.point);
			}
		}

		Debug.DrawRay (shootingPoint.position, shootingPoint.forward * 100f);
	}
}
