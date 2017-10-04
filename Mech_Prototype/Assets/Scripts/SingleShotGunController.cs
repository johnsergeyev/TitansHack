using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShotGunController : Weapon {
	public override void CheckShoot ()
	{
		base.CheckShoot ();

		if (isFingerOnTheTrigger) {
			isFingerOnTheTrigger = false;

			if (ReadyToShoot()) {
				Shoot ();
			} 
		}
	}
}
