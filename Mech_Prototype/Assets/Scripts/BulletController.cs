using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
	[HideInInspector]
	public float distance;
	[HideInInspector]
	public float speed;
	[HideInInspector]
	public Transform initial;
	[HideInInspector]
	public GameObject decal;

	[HideInInspector]
	public GameObject vfx;
	[HideInInspector]
	public float vfx_ttl = 0.1f;

	private GameObject vfxGO;
	private DecalController dc;
	private Vector3 moveVector = Vector3.forward;

	void FixedUpdate () {
		dc = GameObject.FindObjectOfType<DecalController> ();

		transform.Translate (moveVector * speed * Time.fixedDeltaTime);

		if (Mathf.Abs (Vector3.Distance (initial.position, transform.position)) > distance) {
			Destroy (gameObject);
		} else {
			RaycastHit hit;
			if (Physics.Linecast (initial.position, transform.position, out hit)) {
				if (hit.collider.gameObject.tag == "Hittable") {
					Destroy(gameObject);

					GameObject g = dc.getDecal (decal.name);
					g.transform.position = hit.point + hit.normal * 0.01f;
					g.transform.rotation = Quaternion.FromToRotation (Vector3.back, hit.normal);
					g.transform.parent = hit.collider.gameObject.transform;

					if (vfx != null) {
						vfxGO = (GameObject)Instantiate (vfx);
						vfxGO.transform.position = g.transform.position;
						vfxGO.transform.rotation = g.transform.rotation;

						dc.SetVFXToDestroy (vfxGO, vfx_ttl);
					}
				}
			}
		}			
	}
}
