//Responsible for writing ScriptableData to a text file and reading it back.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Yarn.Unity;
using System.Text.RegularExpressions;
using System;
using System.Linq; //used to find interfaces
using System.Text; //used for stringbuilder in Save()

namespace Akitacore{
public class ScriptableSaveLoad : MonoBehaviour {

	public ScriptableDataLibrary database;
	[Tooltip("Propogate database from resource folder. Unnecessary if also using ScriptableYarnStorage.")]
	public bool PropogateOnPlay =true;

	[Tooltip("Container for the current save string. Assignable from event system, used by Save() and Load()")]
	public string saveFile;
	public string[] saveFiles;

	// Use this for initialization
	void Start () {
		#if UNITY_EDITOR
			if(PropogateOnPlay){
				database.Propogate(); 
				//automatically updates the database within the editor. Within a build, no new data should be defined, or if it is, it should be added manually from the script that makes it.
				//the Propogate method is slow, so we don't want it unnecessarily bogging down load times in a build.
			}
		#endif

		saveFiles = Directory.GetFiles (Application.persistentDataPath, "*.saveFile");
		for (int i = 0; i < saveFiles.Length; i++) {
			saveFiles [i] = FileNameFromPath (saveFiles [i]);
		}
	}

	//----File Read/Write Operations----------------------------------------------------------------------------------------------------------------
	public void Save(){
		if(string.Equals(saveFile, "")){
			saveFile = "default";
		}
		string path = Path.Combine (Application.persistentDataPath, saveFile+".SaveFile");
		OverWrite (path, ""); //clear file, create if null.

		Debug.Log ("Saving data to "+path);

		//create the save string from the database
		StringBuilder WIPcontents = new StringBuilder ();
		foreach (var datum in database.boolVars) {
			WIPcontents.Append ("{SCRIPTABLEDATA_BOOLVAR _NAME:");
			WIPcontents.Append (datum.name);
			WIPcontents.Append (", _INIT:");
			WIPcontents.Append (datum.init.ToString());
			WIPcontents.Append (", _VAL:");
			WIPcontents.Append (datum.val.ToString ());
			WIPcontents.Append ("}\n");
		}
		foreach (var datum in database.floatVars) {
			WIPcontents.Append ("{SCRIPTABLEDATA_FLOATVAR _NAME:");
			WIPcontents.Append (datum.name);
			WIPcontents.Append (", _INIT:");
			WIPcontents.Append (datum.init.ToString());
			WIPcontents.Append (", _VAL:");
			WIPcontents.Append (datum.val.ToString ());
			WIPcontents.Append ("}\n");
		}
		foreach (var datum in database.intVars) {
			WIPcontents.Append ("{SCRIPTABLEDATA_INTVAR _NAME:");
			WIPcontents.Append (datum.name);
			WIPcontents.Append (", _INIT:");
			WIPcontents.Append (datum.init.ToString());
			WIPcontents.Append (", _VAL:");
			WIPcontents.Append (datum.val.ToString ());
			WIPcontents.Append ("}\n");
		}
		foreach (var datum in database.stringVars) {
			if (datum.init == "") {
				datum.init = "_NULL";
			}
			if (datum.val == "") {
				datum.val = "_NULL";
			}
			WIPcontents.Append ("{SCRIPTABLEDATA_STRINGVAR _NAME:");
			WIPcontents.Append (datum.name);
			WIPcontents.Append (", _INIT:");
			WIPcontents.Append (datum.init.ToString());
			WIPcontents.Append (", _VAL:");
			WIPcontents.Append (datum.val.ToString ());
			WIPcontents.Append ("}\n");
		}
		foreach (var datum in database.vector2Vars) {
			WIPcontents.Append ("{SCRIPTABLEDATA_VECTOR2VAR _NAME:");
			WIPcontents.Append (datum.name);
			WIPcontents.Append (", _INIT:");
			WIPcontents.Append (datum.init.ToString());
			WIPcontents.Append (", _VAL:");
			WIPcontents.Append (datum.val.ToString ());
			WIPcontents.Append ("}\n");
		}
		foreach (var datum in database.vector3Vars) {
			WIPcontents.Append ("{SCRIPTABLEDATA_VECTOR3VAR _NAME:");
			WIPcontents.Append (datum.name);
			WIPcontents.Append (", _INIT:");
			WIPcontents.Append (datum.init.ToString());
			WIPcontents.Append (", _VAL:");
			WIPcontents.Append (datum.val.ToString ());
			WIPcontents.Append ("}\n");
		}
		foreach (var datum in database.vector4Vars) {
			WIPcontents.Append ("{SCRIPTABLEDATA_VECTOR4VAR _NAME:");
			WIPcontents.Append (datum.name);
			WIPcontents.Append (", _INIT:");
			WIPcontents.Append (datum.init.ToString());
			WIPcontents.Append (", _VAL:");
			WIPcontents.Append (datum.val.ToString ());
			WIPcontents.Append ("}\n");
		}

		Append (path, WIPcontents.ToString ()); //write save string to file

		Debug.Log ("Save complete.");
	}

	public void Load(){
		if(string.Equals(saveFile, "")){
			saveFile = "default";
		}
		//read file contents into local variable
		string path = Path.Combine (Application.persistentDataPath, saveFile+".SaveFile");

		if (!File.Exists (path)) {
			Debug.LogWarning ("no save file found, using scene default values.");
			return;
		}

		Debug.Log ("Loading data from " + path);

		string content;
		StreamReader reader = new StreamReader (path);
		content = reader.ReadToEnd ();
		reader.Close ();


		//parse contents and write to the ScriptableData.

		//BoolVars
		var matches = Regex.Matches (content, "{SCRIPTABLEDATA_BOOLVAR\\s_NAME\\:(.+)\\,\\s*_INIT\\s*:(.+)\\,\\s*_VAL\\s*:(.+)}");
		for (int i = 0; i < matches.Count; i++) {
			foreach (var datum in database.boolVars) {
				if (datum.name == matches [i].Groups [1].ToString ()) {
					datum.init = (matches [i].Groups [2].ToString () == "True");
					datum.val = (matches [i].Groups [3].ToString () == "True");
					break;
				}
			}
		}
		//FloatVars
		matches = Regex.Matches (content, "{SCRIPTABLEDATA_FLOATVAR\\s_NAME\\:(.+)\\,\\s*_INIT\\s*:(.+)\\,\\s*_VAL\\s*:(.+)}");
		for (int i = 0; i < matches.Count; i++) {
			foreach (var datum in database.floatVars) {
				if (datum.name == matches [i].Groups [1].ToString ()) {
					datum.init = float.Parse(matches [i].Groups [2].ToString ()); 
					datum.val = float.Parse(matches [i].Groups [3].ToString ()); 
					break;
				}
			}
		}
		//IntVars
		matches = Regex.Matches (content, "{SCRIPTABLEDATA_INTVAR\\s_NAME\\:(.+)\\,\\s*_INIT\\s*:(.+)\\,\\s*_VAL\\s*:(.+)}");
		for (int i = 0; i < matches.Count; i++) {
			foreach (var datum in database.intVars) {
				if (datum.name == matches [i].Groups [1].ToString ()) {
					datum.init =  int.Parse(matches [i].Groups [2].ToString ()); 
					datum.val = int.Parse(matches [i].Groups [3].ToString ()); 
					break;
				}
			}
		}
		//StringVars
		matches = Regex.Matches (content, "{SCRIPTABLEDATA_STRINGVAR\\s_NAME\\:(.+)\\,\\s*_INIT\\s*:(.+)\\,\\s*_VAL\\s*:(.+)}");
		for (int i = 0; i < matches.Count; i++) {
			foreach (var datum in database.stringVars) {
				if (datum.name == matches [i].Groups [1].ToString ()) {
					datum.init =  matches [i].Groups [2].ToString (); 
					datum.val = matches [i].Groups [3].ToString (); 
					if (datum.init == "_NULL") {
						datum.init = "";
					}
					if (datum.val == "_NULL") {
						datum.val = "";
					}
					break;
				}
			}
		}
		//Vector2Vars
		matches = Regex.Matches (content, "{SCRIPTABLEDATA_VECTOR2VAR\\s_NAME\\:(.+)\\,\\s*_INIT\\:\\((.+)\\,\\s*(.+)\\)\\,\\s*_VAL\\:\\((.+)\\,\\s*(.+)\\)}");
		for (int i = 0; i < matches.Count; i++) {
			foreach (var datum in database.vector2Vars) {
				if (datum.name == matches [i].Groups [1].ToString ()) {
					datum.init.Set (float.Parse (matches [i].Groups [2].ToString ()), float.Parse (matches [i].Groups [3].ToString ()));
					datum.val.Set (float.Parse (matches [i].Groups [4].ToString ()), float.Parse (matches [i].Groups [5].ToString ()));
					break;
				}
			}
		}
		//Vector3Vars
		matches = Regex.Matches (content, "{SCRIPTABLEDATA_VECTOR3VAR\\s_NAME\\:(.+)\\,\\s*_INIT\\:\\((.+)\\,\\s*(.+)\\,\\s*(.+)\\)\\,\\s*_VAL\\:\\((.+)\\,\\s*(.+)\\,\\s*(.+)\\)}");
		for (int i = 0; i < matches.Count; i++) {
			foreach (var datum in database.vector3Vars) {
				if (datum.name == matches [i].Groups [1].ToString ()) {
					datum.init.Set (float.Parse (matches [i].Groups [2].ToString ()), float.Parse (matches [i].Groups [3].ToString ()), float.Parse (matches [i].Groups [4].ToString ()));
					datum.val.Set (float.Parse (matches [i].Groups [5].ToString ()), float.Parse (matches [i].Groups [6].ToString ()), float.Parse (matches [i].Groups [7].ToString ()));
					break;
				}
			}
		}
		//Vector4Vars
		matches = Regex.Matches (content, "{SCRIPTABLEDATA_VECTOR4VAR\\s_NAME\\:(.+)\\,\\s*_INIT\\:\\((.+)\\,\\s*(.+)\\,\\s*(.+)\\,\\s*(.+)\\)\\,\\s*_VAL\\:\\((.+)\\,\\s*(.+)\\,\\s*(.+)\\,\\s*(.+)\\)}");
		for (int i = 0; i < matches.Count; i++) {
			foreach (var datum in database.vector4Vars) {
				if (datum.name == matches [i].Groups [1].ToString ()) {
					datum.init.Set (float.Parse (matches [i].Groups [2].ToString ()), float.Parse (matches [i].Groups [3].ToString ()), float.Parse (matches [i].Groups [4].ToString ()), float.Parse (matches [i].Groups [5].ToString ()));
					datum.val.Set (float.Parse (matches [i].Groups [6].ToString ()), float.Parse (matches [i].Groups [7].ToString ()), float.Parse (matches [i].Groups [8].ToString ()), float.Parse (matches [i].Groups [9].ToString ()));
					break;
				}
			}
		}
	}

	//---Utilities--------------------------------------------------------------------------------------------------------------------------------

	/// Write the content to the save file, without deleting old data. Does not create a file if none exists.
	void Append(string path, string content){
		StreamWriter writer = new StreamWriter (path, true);
		writer.WriteLine (content);
		writer.Close ();
	}

	/// Replace the file's data with the specified content. Will create a file if none exists.
	void OverWrite(string path, string content){
		File.WriteAllText (path, content);
	}

	public void SetFileName(UnityEngine.UI.Text fileName){
		saveFile = fileName.text;
	}


	//--Regex Utilities-------------------------------------------------------------------------------------------------------------------------

	string FileNameFromPath(string path){
		var match = Regex.Match (path, "(\\w*)\\.SaveFile");
		if (match.Groups.Count > 0) {
			return match.Groups [1].ToString ();
		} 
		return path;
	}
}
}