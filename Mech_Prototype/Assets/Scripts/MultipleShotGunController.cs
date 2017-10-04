using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleShotGunController : Weapon {
	public override void CheckShoot ()
	{
		base.CheckShoot ();

		if (isFingerOnTheTrigger) {
			if (ReadyToShoot()) {
				Shoot ();
			}
		}
	}
}
