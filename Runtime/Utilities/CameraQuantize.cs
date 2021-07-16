using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
[ExecuteInEditMode]
public class CameraQuantize : MonoBehaviour {


	Camera cam;
	Transform par; //parent object
	[Tooltip("pixels per unity unit")]
	public int PPU =32;

	[Tooltip("screen pixels per world pixel")]
	public int pixelSize = 1;

	// Use this for initialization
	void Start () {
		par = transform.parent;
		cam = GetComponent<Camera> ();
	}
	
	// Update is called once per frame
	float quantizedPosX;
	float quantizedPosY;

	void Update () {
		//setortho
		cam.orthographicSize = ((0.5f*Screen.height)*(1.0f/pixelSize)*(1.0f/PPU));


		//quantize camera location
		quantizedPosX = ((1.0f/(pixelSize*PPU))*Mathf.Round(par.position.x*pixelSize*PPU));
		quantizedPosY = ((1.0f/(pixelSize*PPU))*Mathf.Round(par.position.y*pixelSize*PPU));

		transform.localPosition = new Vector3(par.position.x - quantizedPosX, par.position.y - quantizedPosY, transform.localPosition.z);
	}

	float quantizedX;
	float quantizedY;
	void Quantize(){
		//quantizedX = ((1.0f/(
	}
}
}
