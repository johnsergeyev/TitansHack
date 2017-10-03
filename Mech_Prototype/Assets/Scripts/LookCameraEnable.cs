using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookCameraEnable : MonoBehaviour {
	public GameObject CameraView;

	 void Start () {
		CameraView.GetComponent<MouseLook>().enabled = false;
	}
	public void OnOff () {
		CameraView.GetComponent<MouseLook>().enabled = true;
	}
}