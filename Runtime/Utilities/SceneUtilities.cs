using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

namespace Akitacore{
public class SceneUtilities : MonoBehaviour {
	
	float transitionStart;
	float transitionLength = 0.5f;
	string transitionTarget;

	public GameObject[] enableOnStart;
	public GameObject[] disableOnStart;



	public void Start(){
		foreach (GameObject obj in enableOnStart) {
			obj.SetActive (true);
		}
		foreach (GameObject obj in disableOnStart) {
			obj.SetActive (false);
		}


	}


	//--Scene management functions------------------------------------------------------------

	///Load the scene with BuildIndex 0
	public void Restart(){
		SceneManager.LoadScene (SceneManager.GetSceneByBuildIndex (0).name);
	}

	///Load the current scene again
	public void RestartScene (){
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

	///Load a specfic scene by name. A number will be parsed into a build index.
	public void LoadScene(string nameOrIndex){
		var match = Regex.Match (nameOrIndex, "^(-?\\d+)$"); //matches to an int with no other characters in the string.
		if (match.Groups.Count > 1) {
			SceneManager.LoadScene (SceneManager.GetSceneByBuildIndex (int.Parse (match.Groups [1].ToString ())).name);
		} else {
			SceneManager.LoadScene (nameOrIndex);
		}
	}

	///Load a specific scene with a transition. A number will be parsed into a build index.
	/// TargetLoader can be set to 0 for default transition behavior.
	public void SceneTransition(string nameOrIndex, int targetLoader){
		transitionStart = Time.time;
		transitionTarget = nameOrIndex;
		StartCoroutine ("Transition");
	}

	///Coroutine for performing transition effects before scene load.
	IEnumerator Transition(){
		while (Time.time < transitionStart + transitionLength) {
			//do transition effects

			//end
			yield return null;
		}
		LoadScene (transitionTarget);
	}


	public void StartCoroutineRemote (IEnumerator coroutine){
		StartCoroutine (coroutine);

	}
		
	///Find a gameobject in the scene by name, whether active or inactive.
	public static GameObject FindInAll(string nameQuery){
		var roots = SceneManager.GetActiveScene ().GetRootGameObjects ();
		foreach (var root in roots) {
			var childList = root.GetComponentsInChildren<Transform> (true);
			foreach (Transform child in childList) {
				if (child.name == nameQuery) {
					return child.gameObject;
				}
			}
		}
		return null;
	}


	///Returns a list of every object in the scene, including inactives.
	public static GameObject[] AllObjects(){
		List<GameObject> objects = new List<GameObject> ();
		List<GameObject> childList;
		foreach (var rootObj in SceneManager.GetActiveScene ().GetRootGameObjects ()) {
			childList = GameObjectsInChildren (rootObj.transform);
			foreach (var obj in childList) {
				objects.Add (obj);
			}
		}
		return objects.ToArray ();
	}
	//recursion utility for AllObjects()
	static List<GameObject> GameObjectsInChildren(Transform trans){
		List<GameObject> objects = new List<GameObject> ();
		objects.Add (trans.gameObject);
		for (int i = 0; i < trans.childCount; i++) {
			foreach (var obj in GameObjectsInChildren(trans.GetChild(i))) {
				objects.Add (obj);

			}
		}
		return objects;
	}



	public void Precompile(){
		//get literally every gameobject, active or not
		List<GameObject> allObjects = new List<GameObject> ();
		var roots = SceneManager.GetActiveScene ().GetRootGameObjects ();
		foreach (var root in roots) {
			allObjects.Add (root);
			var childList = root.GetComponentsInChildren<Transform> (true);
			foreach (Transform child in childList) {
				if (!allObjects.Contains (child.gameObject)) {
					allObjects.Add (child.gameObject);
				}
			}
		}

		//precompile AOT code in  statemachinecontrollers
		foreach (GameObject obj in allObjects) {
			if (obj.GetComponent<StateMachineController> () != null) {
				//obj.GetComponent<StateMachineController> ().AOTInitialize ();
			}
		}

	}

	//--Coroutines-----------------
	public IEnumerator SwapOnInactive(TextAsset yarnfile, InteractDialogue currentSpeakerDialogue){
		if (GameObject.Find ("Dialogue").activeInHierarchy) {
			yield return null;
		} else {
			currentSpeakerDialogue.yarnScript = yarnfile;
		}
	}

}
}
