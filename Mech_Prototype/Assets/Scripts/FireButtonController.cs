using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FireButtonController : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler {
	public float padding = 5f;
	public float heightPercentage = 20f;
	public bool continiousShooting = true;
	public bool isMainWeapon = true;

	private bool isDown = false;
	private bool isOver = false;


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
