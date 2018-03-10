using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleRotation : MonoBehaviour {

	// Use this for initialization
	public float WaitSecond;
	bool IsStartAnimation;
	public  Vector3 RotationAngle;
	void Start () {
		Invoke ("OnStartAnimation",WaitSecond);
	}
	void OnStartAnimation()
	{
		IsStartAnimation = true;
	}
	// Update is called once per frame
	void Update () {
		if (IsStartAnimation) {
			transform.Rotate (RotationAngle);
		}
	}
}
