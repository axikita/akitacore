//Interactable. Upon interaction, play assigned dialogue.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;


namespace Akitacore{
public class InteractDialogue : Interactable {

	public bool pending;
	public StringVar startNode;
	public GameObject interactableHint;
	public TextAsset yarnScript;

	public bool running = false;


	public override void Interact(){
		DialogueRunner runner = FindObjectOfType<DialogueRunner> ();
		if (runner.isDialogueRunning || yarnScript == null) {
			return;
		}
		runner.Clear ();
		runner.AddScript (yarnScript);
		runner.StartDialogue (startNode.val);
		pending = false;
		running = true;
		if (interactableHint) {
			interactableHint.SetActive (false);
		}
	}

}
}
