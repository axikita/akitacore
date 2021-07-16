//add this to a player controller or a state machine controller to suspend during the specified conditions
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

namespace Akitacore{
public class SuspendDetector : Interactable {

	StateMachineController stateMachine;
	PlayerController player;

	public bool suspendIfDialogue;
	public bool suspendIfInteractedDialogue;
	bool dialogue;

	bool interacted;

	void Start () {
		stateMachine = GetComponent<StateMachineController> ();
		player = GetComponent <PlayerController> ();
	}

	// Use this for initialization
	public override void Interact(){
		interacted = true;
	}



	void Update () {
		//observe game state
		if (FindObjectOfType<DialogueRunner>()!=null) {
			dialogue = FindObjectOfType<DialogueRunner> ().isDialogueRunning;
		}


		//assign suspension
		if (dialogue) {
			if (stateMachine != null) {
				if (suspendIfDialogue || (suspendIfInteractedDialogue && interacted)) {
					stateMachine.suspended = true;
				}
			}
			if (player != null) {
				if (suspendIfDialogue || (suspendIfInteractedDialogue && interacted)) {
					player.suspended = true;
				}
			}

		} else {
			if (stateMachine != null) {
				stateMachine.suspended = false;	
			}
			if (player != null) {
				player.suspended = false;
			}
		}

		interacted = false;
	}
}
}
