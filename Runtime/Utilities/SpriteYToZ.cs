//automatic sprite sorting
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
public class SpriteYToZ : MonoBehaviour {

	[Tooltip("Apparent Offset from true origin")]
	public float offset;

	void Update () {
		transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.y + offset);
	}
}
}
