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
    public GameObject decal;

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

					//Instantiate<GameObject>(decal, hit.point + hit.normal * 0.01f, Quaternion.FromToRotation (Vector3.back, hit.normal));


					g.transform.parent = hit.collider.gameObject.transform;
				}
			}
		}			
	}

          /*  if (c.gameObject.GetComponent<Rigidbody>() != null)
            {
                c.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(gameObject.transform.position.normalized * 80f, hit.point);
            }*/
}
