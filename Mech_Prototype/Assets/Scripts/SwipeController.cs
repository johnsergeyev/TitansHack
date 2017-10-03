using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeController : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler{
	public float swipeKf = 100f;

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
			return;
        }

		if (ped.pointerId != touchId)
        {
			return;
        }

        delta = ped.position.x - touchX;

		value = delta;

		if ((Mathf.Abs (value) / swipeKf) > 1f) {
			float oldDelta = value;
			float mpl = 1f;

			if (value < 0f) {
				value = -swipeKf;
				mpl = -1f;
			} else {
				value = swipeKf;
			}

			oldDelta = Mathf.Abs (oldDelta) - Mathf.Abs (value);
			touchX += mpl * oldDelta;
		}

		value /= swipeKf;
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
