//launch a dialogue at scene start, or when gameobject is enabled.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

namespace Akitacore{
public class DialogueOnEnable : MonoBehaviour {

	public StringVar startNode;
	public TextAsset yarnScript;

	bool pending = false;


	public void OnEnable(){
		pending = true;

	}

	public void Update(){
		if(pending){
			pending = false;
			DialogueRunner runner = FindObjectOfType<DialogueRunner> ();
			if (runner.isDialogueRunning || yarnScript == null) {
				return;
			}
			runner.Clear ();
			runner.AddScript (yarnScript);
			if(startNode !=null){
				runner.StartDialogue (startNode.val);
			} else {
				runner.StartDialogue ("Start");
			}
		}
	}


}
}
