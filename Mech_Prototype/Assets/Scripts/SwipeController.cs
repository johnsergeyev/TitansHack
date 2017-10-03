using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeController : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler{
	public float sensivity = 100f;
	public float deadZone = 10f;

	private RectTransform rt;
	private float value = 0.0f;
	private float touchX = 0.0f;

	private bool isTouched = false;
	private int touchId;
    private float delta;

	void Start () {
		rt = GetComponent<RectTransform> ();
		Vector2 om = rt.offsetMin;
        om.x = 0;
		rt.offsetMin = om;
	}

	public virtual void OnDrag(PointerEventData ped) {
		if (!isTouched)
        {
            Debug.Log("isn't touched");
			return;
        }

		if (ped.pointerId != touchId)
        {
            Debug.Log("wrong pointer");
			return;
        }

        delta = ped.position.x - touchX;
		if (Mathf.Abs (delta) < deadZone) {
			value = 0f;
			return;
		}

		delta = delta - deadZone;

		if (sensivity == 100f) {
			value = delta;
		} else {
			value = delta / (1 / (sensivity / 100));
		}

		value /= 10;

        if (value < -1) value = -1;
        if (value > 1) value = 1;
    }

	public virtual void OnPointerDown(PointerEventData ped) {
		isTouched = true;
		touchId = ped.pointerId;
		touchX = ped.position.x;
		OnDrag (ped);
	}

	public virtual void OnPointerUp(PointerEventData ped) {
		isTouched = false;
		value = 0f;
	}

	public float Value() {
		return value;
	}
}
