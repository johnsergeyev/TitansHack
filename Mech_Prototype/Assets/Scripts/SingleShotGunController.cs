using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShotGunController : Weapon {
	public override void CheckShoot ()
	{
		if (isFingerOnTheTrigger) {
			isFingerOnTheTrigger = false;
			Shoot ();
		}
	}
}
