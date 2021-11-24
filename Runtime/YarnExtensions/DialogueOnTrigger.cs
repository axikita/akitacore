using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;



namespace Akitacore{
public class DialogueOnTrigger : MonoBehaviour
{
	
	public StringVariable startNode;
	public TextAsset yarnScript;
	
	public TriggerType triggerType;
	public string targetTag;
	
	
	Collider2D coll2d;
	Collider coll3d;
	
	public enum TriggerType{any, tag, playercontroller}
	
	// Start is called before the first frame update
	void Start()
	{
		coll2d = GetComponent<Collider2D>();
		coll3d = GetComponent<Collider>();
		if(coll2d == null && coll3d == null){
			Debug.LogWarning("No collider (2d or 3d) on "+gameObject.name+", cannot run DialogueOnTrigger");
			this.enabled = false;
		}
	}
	
	
	
	void OnTriggerStay(Collider other){
		//check block exits if the appropriate conditions are not met.
		if(triggerType == TriggerType.tag){
			if(!TagInParent(other.gameObject, targetTag)){
				return;
			}
		} else if(triggerType == TriggerType.playercontroller){
			if(!ControllerInParent(other.gameObject)){
				return;
			}
		} //TriggerType.any will ignore checkblock and continue on any collision
		
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
		gameObject.SetActive(false);
	}
	
	void OnTriggerStay2D(Collider2D other){
		//check block exits if the appropriate conditions are not met.
		if(triggerType == TriggerType.tag){
			if(!TagInParent(other.gameObject, targetTag)){
				return;
			}
		} else if(triggerType == TriggerType.playercontroller){
			if(!ControllerInParent(other.gameObject)){
				return;
			}
		}//TriggerType.any will ignore checkblock and continue on any collision
	
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
		gameObject.SetActive(false);
	}
	
	
	
	//recursive tag search
	bool TagInParent(GameObject obj, string searchTag){
		if(obj.tag == searchTag){
			return true;
		} else if(obj.transform.parent!=null){
			return TagInParent(obj.transform.parent.gameObject, searchTag);
		} else {
			return false;
		}
	}
	
	//recursive component sarch
	bool ControllerInParent(GameObject obj){
		if(obj.GetComponent<PlayerController>()!=null){
			return true;
		} else if(obj.transform.parent!=null){
			return ControllerInParent(obj.transform.parent.gameObject);
		} else {
			return false;
		}
	}
}
}
