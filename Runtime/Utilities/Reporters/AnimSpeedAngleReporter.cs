//reports Angle(int)[0-3], Angle(float)[0-360], and Speed(Float) to the attached Animator.
//intended for basic test usage.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
[RequireComponent(typeof(Animator))]
public class AnimSpeedAngleReporter : MonoBehaviour {

	public GameObject physicsRoot;

	Animator animr;

	Vector3 lastPos;
	float speed;

	// Use this for initialization
	void Start () {
		animr = GetComponent<Animator> ();
		lastPos = physicsRoot.transform.position;
	}
	
	void FixedUpdate(){
		speed = ((lastPos - physicsRoot.transform.position).magnitude / Time.fixedDeltaTime);
		lastPos = physicsRoot.transform.position;
	}

	void Update () {
		float angle = Vector3.SignedAngle (Vector3.right, physicsRoot.transform.right, Vector3.forward);

		foreach (var parameter in animr.parameters) {
			//Debug.Log (parameter.type.ToString ());
			if (string.Equals (parameter.name, "Speed") && string.Equals(parameter.type.ToString(),"Float")) {
				animr.SetFloat ("Speed", speed);
			}
			if (string.Equals (parameter.name, "Angle") && string.Equals(parameter.type.ToString(),"Float")) {
				animr.SetFloat ("Angle", Clamp360(angle));
			}
			if (string.Equals (parameter.name, "Direction") && string.Equals(parameter.type.ToString(),"Int")) {
				animr.SetInteger ("Direction", Mathf.FloorToInt (Clamp360(angle + 45.0f) / 90.0f));
			}
		}
	}

	float Clamp360(float angle){
		if (angle < 0) {
			angle += 360;
		}
		if (angle > 360) {
			angle -= 360;
		}
		return angle;
	}
}
}
