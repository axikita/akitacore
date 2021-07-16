using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Akitacore{
public static class ExtensionMethods {

	public static Vector2 V2XY(this Vector3 vec){
		return new Vector2 (vec.x, vec.y);
	}

	public static Vector2 V2XZ(this Vector3 vec){
		return new Vector2 (vec.x, vec.z);
	}

	public static Vector2 V2YZ(this Vector3 vec){
		return new Vector2 (vec.y, vec.z);
	}
}
}
