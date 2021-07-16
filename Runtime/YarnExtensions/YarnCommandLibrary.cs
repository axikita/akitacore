using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using Yarn.Unity;

//defines commands that can be called directly from yarn scrips.
//command syntax is <<FunctionName argumentString>>

//Class is static, so no reference is necessary. Yarn commands are called in CoreDialogueUI.cs

namespace Akitacore{
public static class YarnCommandLibrary {

	public static void RunCommand(string command){
		Dictionary<string, Func<string,int>> functiondict = new Dictionary<string, Func<string,int>> ();
		functiondict.Add ("Log", Log); //prints to console via debug.log
		functiondict.Add ("NOTE:", NOTE); //prints to console but also searchable as a note to the programmer
		functiondict.Add ("Destroy", Destroy); //destroy any gameobject by name. Uses Gameobject.Find(name)
		functiondict.Add ("Cinematic", Cinematic); //display a cinematic.
		functiondict.Add ("ClearCinematic", ClearCinematic); //clear a cinematic.
		functiondict.Add ("SwapScript", SwapScript); //doesn't work!! Swaps to a different yarn script.
		functiondict.Add ("SetActive", SetActive); //Activates or deactivates any gameobject by name.
		functiondict.Add ("PlaySong", PlaySong);  //plays a song named in Radio
		functiondict.Add ("PlaySound", PlaySound); //plays foley named in Radio
		functiondict.Add ("StopSound", StopSound); //stops foley playing on Radio.


		var match = Regex.Match (command, "^(\\w+:?)\\s*(.*)");

		try {
			functiondict[match.Groups[1].ToString()](match.Groups[2].ToString());
		} catch {
			Debug.LogWarning ("YarnCommandError: command \"" + match.Groups[1].ToString() + "\" is not a recognized command.");
		}
	}

	public static bool HasCommand(string command){
		List<string> commands;
		commands = new List<string>();
		commands.Add ("if");
		commands.Add ("elseif");
		commands.Add ("else");
		commands.Add ("endif");
		commands.Add ("set");
		commands.Add ("stop");
		commands.Add ("Log");
		commands.Add ("NOTE:");
		commands.Add ("Destroy");
		commands.Add ("Cinematic");
		commands.Add ("ClearCinematic");
		commands.Add ("SwapScript");
		commands.Add ("SetActive");
		commands.Add ("PlaySong");
		commands.Add ("PlaySound");
		commands.Add ("StopSound");

		return commands.Contains(command);
	}

	/// <<Log messageString>>
	static int Log(string str){
		Debug.Log("Yarn command logged:"+str);
		return 0;
	}

	/// <<Destroy gameobjectName>>
	static int Destroy(string str){

		try {
			GameObject target = GameObject.Find(str);
			GameObject.Destroy(target);
		} catch {
			Debug.LogWarning ("YarnCommandError: could not destroy " + "str");
		}
		return 0;
	}

	/// <<Cinematic imageName, front>> or <<Cinematic imageName, back>>
	/// imageName defined in CinematicResponse.cs
	static int Cinematic(string str){
		var match = Regex.Match(str, "(\\w+),\\s*(\\w{4,5})");
		if (match.Groups.Count != 3) {
			Debug.LogWarning ("Invalid arguments passed to Cinematic(" + str + ") by yarn.");
			return 0;
		}
		GameObject.FindObjectOfType<CinematicResponse> ().Cinematic (match.Groups [1].ToString (),string.Equals(match.Groups [2].ToString (),"front")|string.Equals(match.Groups [2].ToString (),"true"));
		return 0;
	}

	/// <<ClearCinematic>>
	static int ClearCinematic(string str){
		GameObject.FindObjectOfType<CinematicResponse> ().ClearCinematic ();

		return 0;
	}

	//full command: <<SetActive object name, true>>
	static int SetActive(string str){
		var match = Regex.Match (str, "(.*),\\s*(true|false)"); //matches "object name, true" or "objectname, false" - requires end with comma-space-bool. Match1 = name, Match2 = bool.
		if (match.Groups.Count != 3) {
			Debug.LogWarning ("Yarn Command <<SetActive objectname, bool>> recieved an improperly formatted argument list.");
			return 0;
		}
		if (match.Groups [2].ToString () == "true") {
			GameObject obj = SceneUtilities.FindInAll(match.Groups [1].ToString ());
			if (obj != null) {
				obj.SetActive (true);
			} else {
				Debug.LogWarning (match.Groups [1].ToString () + " does not match the name of a gameobject in the current scene.");
			}
		} else if (match.Groups [2].ToString () == "false") {
			GameObject obj = SceneUtilities.FindInAll(match.Groups [1].ToString ());
			if (obj != null) {
				obj.SetActive (false);
			} else {
				Debug.LogWarning (match.Groups [1].ToString () + " does not match the name of a gameobject in the current scene.");
			}
		} else {
			Debug.LogWarning ("Yarn Command <<SetActive objectname, bool>> recieved an improperly formatted boolean value.");
		}
		return 0;
	}

	static int PlaySong(string str){
		GameObject.FindObjectOfType<Radio> ().PlaySong (str);
		return 0;
	}

	static int PlaySound(string str){
		GameObject.FindObjectOfType<Radio> ().PlaySound (str);
		return 0;
	}

	static int StopSound(string str){
		GameObject.FindObjectOfType<Radio> ().StopSound (str);
		return 0;
	}


	//this doesn't work yet. See SceneUtilities as well for related code.
	static int SwapScript(string path){
		Debug.Log ("SwapScript called with arguments:"+path);
		DialogueRunner runner = GameObject.FindObjectOfType<DialogueRunner> ();
		SceneUtilities standalone = GameObject.FindObjectOfType<SceneUtilities> ();
		InteractDialogue[] dialogues = GameObject.FindObjectsOfType<InteractDialogue> ();

		TextAsset newScript = Resources.Load<TextAsset> (path);
		Debug.Log (newScript.name);

		if (newScript != null && dialogues.Length > 0 && standalone != null && runner != null) {
			for (int i = 0; i < dialogues.Length; i++) {
				if (dialogues [i].yarnScript == runner.sourceText [0]) {
					if (standalone != null) {
						Debug.Log ("calling coroutine");
						standalone.StartCoroutineRemote (standalone.SwapOnInactive (newScript, dialogues [i]));
					}

				}
			}
		}

		return 0;
	}
		
	static int NOTE(string note){
		Debug.Log ("Note from yarn script: " + note);
		return 0;
	}

}
}
