using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {

	public Camera mainCam;
	float zPos;
	float shakeAmount = 0;

	void Awake()
	{
		if (mainCam == null)
			mainCam = Camera.main;
		zPos = Camera.main.transform.position.z;
	}

	public void Shake(float amt, float length)
	{
		shakeAmount = amt;
		InvokeRepeating("DoShake", 0, 0.01f);
		Invoke("StopShake", length);
	}

	void DoShake()
	{
		if (shakeAmount > 0)
		{
			float offsetX = Random.value * shakeAmount * 2 - shakeAmount;
			float offsetY = Random.value * shakeAmount * 2 - shakeAmount;

			mainCam.transform.position = new Vector3(offsetX, offsetY, -10);
		}
	}

	void StopShake()
	{
		CancelInvoke("DoShake");
		mainCam.transform.localPosition = new Vector3(0,0,-10);
	}

}
