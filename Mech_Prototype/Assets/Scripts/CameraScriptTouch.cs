using UnityEngine;
using System.Collections;

public class CameraScriptTouch: MonoBehaviour {
	
	public float zoomEase = 0.01f;
	public float rotateEase = 2f;
	public float minDistance = 3f;
	public float maxDistance = 15f;
	public GameObject lookAtTarget;
	
	void Start()
	{
		transform.LookAt(lookAtTarget.transform);
	}
	
	void Update()
	{
		if (Input.touchCount == 1)
		{
			Touch touchZero = Input.GetTouch(0);
			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			
			float distance = Vector2.Distance(touchZero.position, touchZeroPrevPos);
			bool right = touchZero.position.x > touchZeroPrevPos.x;
			
			transform.RotateAround(lookAtTarget.transform.position, right?Vector3.up:-Vector3.up, distance * rotateEase);
		}
		else if (Input.touchCount == 2)
		{
			Touch touchZero = Input.GetTouch(0);
			Touch touchOne = Input.GetTouch(1);
			
			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
			
			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
			
			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
			float distance = Vector3.Distance(transform.position, lookAtTarget.transform.position);
			
			if (deltaMagnitudeDiff < 0)
			{
				//zoomIN
				if (distance <= minDistance) return;
			} else if (deltaMagnitudeDiff > 0)
			{
				//zoomOUT
				if (distance >= maxDistance) return;
			}
			
			transform.Translate(0, 0, -deltaMagnitudeDiff * zoomEase);
		}
	}
}