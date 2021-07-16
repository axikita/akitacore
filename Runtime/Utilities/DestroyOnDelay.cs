using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
public class DestroyOnDelay : MonoBehaviour {

	public float delay;
	float awakeTime;

	// Use this for initialization
	void Awake () {
		awakeTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > awakeTime + delay) {
			GameObject.Destroy (gameObject);
		}
	}
}
}
