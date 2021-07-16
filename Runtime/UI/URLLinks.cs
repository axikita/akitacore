//collection of urls to open. 
//Since OpenURL is vulnerable and I don't know best practices, I'm taking the nuclear option of hard coding these.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class URLLinks : MonoBehaviour {

	public void LoadURLGlip(){
		Application.OpenURL ("https://glitchedpuppet.com");
	}
	public void LoadURLAxi(){
		Application.OpenURL ("https://axiakita.itch.io");
	}
	public void LoadURLFloraverse(){
		Application.OpenURL ("https://floraverse.com");
	}
	public void LoadURLBandcamp(){
		Application.OpenURL ("https://floraverse.bandcamp.com");
	}
}
