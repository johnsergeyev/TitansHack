using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleShotGunController : Weapon {
	public float shotDelay = 0.5f;
	private float currentShotDelay = -1f;

	public override void CheckShoot ()
	{
		if (isFingerOnTheTrigger) {
			if (currentShotDelay >= shotDelay || currentShotDelay < 0f) {
				currentShotDelay = 0.0f;
				Shoot ();
			} else {
				currentShotDelay += Time.deltaTime;
			}
		}
	}
}
