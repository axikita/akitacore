using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
public class TransformReporter : MonoBehaviour
{
	[Tooltip("Optional. Automatically assigned if animr is on the same gameobject.")]
	public Animator animr;
	[Tooltip("If true, send PosX, PosY, PosZ.")]
	public bool animatePosition;
	[Tooltip("If true, send ScaleX, ScaleY, ScaleZ.")]
	public bool animateScale;
	
	void Start(){
		if(animr == null){
			animr = GetComponent<Animator>();
		}
	}
	void Update(){
		if(animr !=null){
			if(animatePosition){
				animr.SetFloat("PosX", transform.position.x);
				animr.SetFloat("PosY", transform.position.y);
				animr.SetFloat("PosZ", transform.position.z);
			}
			if(animateScale){
				animr.SetFloat("ScaleX", transform.localScale.x);
				animr.SetFloat("ScaleY", transform.localScale.y);
				animr.SetFloat("ScaleZ", transform.localScale.z);
			}
		}
	}
	public float GetX(){
		return transform.position.x;
	}
	public float GetY(){
		return transform.position.y;
	}
	public float GetZ(){
		return transform.position.z;
	}
	
	public float GetScaleX(){
		return transform.localScale.x;
	}
	public float GetScaleY(){
		return transform.localScale.y;
	}
	public float GetScaleZ(){
		return transform.localScale.z;
	}
	
	
}
}
