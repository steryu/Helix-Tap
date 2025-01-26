using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Helix Rotation */

public class HelixRotation : MonoBehaviour
{
	public float rotationSpeedPc = 5f;
	public float rotationSpeedAndroid = 5f;

	public void Update()
	{
#if UNITY_EDITOR || UNITY_STANDALONE
		if (Input.GetMouseButton(0))
		{
			float mouseX = Input.GetAxisRaw("Mouse X");
			transform.Rotate(transform.position.x, transform.position.y - mouseX * rotationSpeedPc, transform.position.z);
		}
#elif UNITY_ANDROID
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) 
		{
			float touchX = Input.GetTouch(0).deltaPosition.x;
			transform.Rotate(transform.position.x, transform.position.y -touchX * rotationSpeedAndroid, transform.position.z);
		}
#endif
	}
}
