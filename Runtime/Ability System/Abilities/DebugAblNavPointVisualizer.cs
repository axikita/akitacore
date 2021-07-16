//draws the navigation path for an AblAINavPointFollow.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
[RequireComponent(typeof(AblAINavPointFollow))]
[ExecuteInEditMode]
public class DebugAblNavPointVisualizer : MonoBehaviour {

	AblAINavPointFollow navFollow;
	public Vector3 cubeSize = new Vector3 (0.1f,0.1f,0.1f);

	// Use this for initialization
	void Start () {
		navFollow = GetComponent<AblAINavPointFollow> ();
	}
	
	// Update is called once per frame
	void OnDrawGizmos () {
		if (navFollow) {
			if (navFollow.navPoints.Count > 1) {
				for (int i = 0; i < navFollow.navPoints.Count - 1; i++) {
					Debug.DrawLine (navFollow.navPoints [i], navFollow.navPoints [i + 1], Color.blue);
				}
			}
			if (navFollow.navPoints.Count > 0) {
				for (int i = 0; i < navFollow.navPoints.Count; i++) {
					Gizmos.DrawCube (navFollow.navPoints [i], cubeSize);
				}
			}
		}


	}
}
}
