using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FireButtonController : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler {
	public float padding = 5f;
	public float heightPercentage = 20f;
	public bool continiousShooting = true;
	public bool isMainWeapon = true;

	private float size;
	private RectTransform rt;
	private bool isDown = false;
	private bool isOver = false;

	void Start () {
		float originalSize;

		size = Screen.height * heightPercentage / 100;
		Vector3 scale = transform.localScale;

		rt = GetComponent<RectTransform> ();

		originalSize = rt.sizeDelta.x;
		scale = scale * size / rt.sizeDelta.x;

		Vector2 v = rt.offsetMin;

		if (v.y != padding) {
			for (int i = 1; i < 100; i++) {
				if (v.y == (i * originalSize + (i + 1) * padding)) {
					v.y = (i * size + (i + 1) * padding) * scale.magnitude;
					rt.offsetMin = v;
					v = rt.sizeDelta;
					v.y = originalSize;
					rt.sizeDelta = v;
					break;
				}
			}
		}

		transform.localScale = scale;
	}

	public virtual void OnPointerEnter(PointerEventData ped) {
		if (!continiousShooting)
			return;
		if (isOver)
			return;
		isOver = true;
		OnPointerDown (ped);
	}

	public virtual void OnPointerExit(PointerEventData ped) {
		if (!continiousShooting)
			return;
		if (!isOver)
			return;
		isOver = false;
		OnPointerUp (ped);
	}

	IEnumerator upAfterDelay() {
		yield return new WaitForSeconds (0.1f);
		isDown = false;
		isOver = false;
		EventManager.TriggerEvent ("onFireButtonUp" + (isMainWeapon?"Main":"Secondary"));
	}

	public virtual void OnPointerDown(PointerEventData ped) {
		if (isDown)
			return;
			isDown = true;
			isOver = true;

		EventManager.TriggerEvent ("onFireButtonDown" + (isMainWeapon?"Main":"Secondary"));

		if (!continiousShooting) {
			StartCoroutine ("upAfterDelay");
		}
	}

	public virtual void OnPointerUp(PointerEventData ped) {
		if (!isDown)
			return;

			isDown = false;
			isOver = false;
		EventManager.TriggerEvent ("onFireButtonUp" + (isMainWeapon?"Main":"Secondary"));
	}
}
