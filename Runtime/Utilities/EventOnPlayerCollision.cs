//Calls a UnityEvent when something with a PlayerController collides.
//Works with triggers or non-trigger colliders.
//works with 2D or 3D colliders.
//works with the PlayerController component existing on parents of the actual detected collider.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Akitacore{
public class EventOnPlayerCollision : MonoBehaviour
{
	CollisionCheck2D collCheck2D;
	CollisionCheck3D collCheck3D;
	
	public bool onCollision = true;
	public bool onTrigger = false;
	
	[Tooltip("Called when collission or trigger event is detected with a PlayerController")]
	public UnityEvent myEvent;
	
    // Start is called before the first frame update
    void Start()
    {
		bool hasCollider = false;
		if(GetComponent<Collider2D>() != null){
			collCheck2D = gameObject.AddComponent<CollisionCheck2D>();
			hasCollider = true;
		}
		if(GetComponent<Collider>() !=null){
			collCheck3D = gameObject.AddComponent<CollisionCheck3D>();
			hasCollider = true;
		}
		
		if(!hasCollider){
			Debug.LogWarning("2D or 3D collider expected by EventOnPlayerCollission.cs on "+gameObject.name+". Disabling component.");
			this.enabled = false;
		}
    }


    void Update()
    {
		if(collCheck2D!=null){
			//this is, for some reason, a problem to check for on the first frame after a scene reset event.
			if(collCheck2D.colliders ==null){
				return;
			}
			if(onCollision){
				foreach (var other in collCheck2D.colliders){
					if (ComponentInSelfOrParents<PlayerController>(other)){
						myEvent.Invoke();
						return;
					}
				}
			}
			if(onTrigger){
				foreach (var other in collCheck2D.triggers){
					if (ComponentInSelfOrParents<PlayerController>(other)){
						myEvent.Invoke();
						return;
					}
				}
			}
		}
		if(collCheck3D!=null){
			if(collCheck3D.colliders ==null){
				return;
			}
			if(onCollision){
				foreach (var other in collCheck3D.colliders){
					if (ComponentInSelfOrParents<PlayerController>(other)){
						myEvent.Invoke();
						return;
					}
				}
			}
			if(onTrigger){
				foreach (var other in collCheck3D.triggers){
					if (ComponentInSelfOrParents<PlayerController>(other)){
						myEvent.Invoke();
						return;
					}
				}
			}
		}
		
	}
	
	
	//used for recursive search of parent objects for a playercontroller.
	bool ComponentInSelfOrParents<T>(GameObject obj){
		if(obj.GetComponent<T>() != null){
			return true;
		} else if(obj.transform.parent!=null){
			return ComponentInSelfOrParents<T>(obj.transform.parent.gameObject);
		} else {
			return false;
		}
	}
}
}
