//	^([A-Za-z]+)_(?:([LRlr])_)?([A-Za-z]+)(?:_(\d+))?\. will identify CharacterName_R_Emotion_1.png (L/R and # are optional)


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System.IO;
using System;
//using UnityEditor.AssetDatabase;


namespace Akitacore{
public class TalkSpriteImport : EditorWindow {
	
	public string characterName;
	
	[MenuItem("Window/TalkSprite Import")]
	private static void OpenWindow()
	{
		TalkSpriteImport window = GetWindow<TalkSpriteImport>();
		window.titleContent = new GUIContent("TalkSprite Import");
	}
	

	//use for variable initialization
	void OnEnable(){
		
	}
	
	void OnGUI(){
			GUIAbout();
			GUIInput();
			GUIConfirm();
			GUIFormatAbout();
			
			
	}
	//-End Monobehavior-
	//--OnGUI subsets------------------
	
	void GUIAbout(){
		GUIStyle textWrapStyle = new GUIStyle();
		textWrapStyle.wordWrap=true;
		
		EditorGUILayout.LabelField ("This is an advanced import option for converting a folder of character expression sprites into a properly formatted SpeakerData asset.",textWrapStyle);
		EditorGUILayout.LabelField ("----------");
	}
	
	void GUIInput(){
		using (var horiz = new EditorGUILayout.HorizontalScope ()) {
			EditorGUILayout.LabelField ("Character Name:",GUILayout.Width(120.0f));
			characterName = EditorGUILayout.TextField(characterName);
		}
	}
	void GUIConfirm(){
		if(GUILayout.Button("Create SpeakerData",GUILayout.Width(200.0f))){
			CreateData();
		}
	}
	
	void GUIFormatAbout(){
		EditorGUILayout.LabelField (" ");
		
		EditorGUILayout.LabelField ("Acceptable Image Name Format Examples:");
		EditorGUILayout.LabelField ("CharacterName_L_Emote_1");
		EditorGUILayout.LabelField ("CharacterName_R_Emote_2");
		EditorGUILayout.LabelField ("L/R and number are optional");
		EditorGUILayout.LabelField ("CharacterName_Emote is thus also valid");

	}
	
	void CreateData(){
		//verify character name is legal
		if(!Regex.IsMatch(characterName, "^[A-Za-z]+$")){
			Debug.LogError("TalkSpriteImport cannot create SpeakerData, Character Name must be only alphabetical characters and cannot be null.");
			return;
		}
		Debug.Log("Attempting to create SpeakerData for "+characterName+".");
		
		//search for pre-existing speakerdata
		
		string speakerPath; 
		speakerPath = Application.dataPath;//+"/Resources/YarnSpeakers";
		
		speakerPath = speakerPath+"/Resources";
		if(!Directory.Exists(speakerPath)){
			UnityEditor.AssetDatabase.CreateFolder("Assets","Resources");
			Debug.Log("No Resources folder found. Assets/Resources/ created.");
		}
		speakerPath = speakerPath+"/YarnSpeakers";
		if(!Directory.Exists(speakerPath)){
			UnityEditor.AssetDatabase.CreateFolder("Assets/Resources","YarnSpeakers");
			Debug.Log("No YarnSpeakers folder found. Assets/Resources/YarnSpeakers created.");
		}
		//--at this point, the path to Assets/Resources/YarnSpeakers is verified or created
	
		string portraitPath = Application.dataPath+"/Resources/PortraitSprites";
		if(!Directory.Exists(portraitPath)){
			UnityEditor.AssetDatabase.CreateFolder("Assets/Resources","PortraitSprites");
			Debug.Log("No PortraitSprites folder found. Assets/Resources/PortraitSprites created.");
		}
		//--at this point, the path to Assets/Resources/PortraitSprites is verified or created
	
		//get all the current SpeakerData assets
		UnityEngine.Object[] speakerResources = Resources.LoadAll("YarnSpeakers");
		List<SpeakerData> speakers = new List<SpeakerData>();
		
		foreach(var entry in speakerResources){
			if(entry is SpeakerData){
				speakers.Add(entry as SpeakerData);
			}
		}
		
		//get all the current portrait sprite assets
		UnityEngine.Object[] portraitCandidates = Resources.LoadAll("PortraitSprites");
		List<Sprite> portraitSprites = new List<Sprite>();
		
		foreach(var entry in portraitCandidates){
			if(entry is Sprite){
				portraitSprites.Add(entry as Sprite);
			}
		}
		
		if(portraitSprites.Count == 0){
			Debug.LogError("No sprites found in Assets/Resources/PortraitSprites. Make sure your portrait files are in the right place.");
			return;
		}
		
		//--At this point, all our relevant resources are loaded.
		
		//If the speakerdata does not exist, create it. If it does exist, prompt deletion before continuing.
		
		//Prevent overwriting an existing speaker. This could be replaced by a cool popup window later.
		for (int i=0; i<speakers.Count; i++){
			if(speakers[i].speaker == characterName){
				Debug.LogError("SpeakerData already exists for "+characterName+". Please manually delete this asset before re-instantiating it with this script.");
				return;
			}
		}
		
		SpeakerData newSpeaker = ScriptableObject.CreateInstance("SpeakerData") as SpeakerData;
		UnityEditor.AssetDatabase.CreateAsset(newSpeaker, "Assets/Resources/YarnSpeakers/"+characterName+".asset");
		newSpeaker.speaker = characterName;
		
		//--At this point, our new speaker is now created and ready to be filled with sprites.
		
		//find the actual matching portraits- remove anything where the name does not match the speaker, and anything not formatted like a portrait sprite
		List<Sprite> removableSprite = new List<Sprite>();
		foreach(var entry in portraitSprites){
			if(!Regex.IsMatch(entry.name, "^("+characterName+")_(?:([LRlr])_)?([A-Za-z]+)(?:_(\\d+))?$")){
				removableSprite.Add(entry);
			}
		}
		foreach(var entry in removableSprite){ //this is two loops so that portraitSprite does not get modified during iteration
			portraitSprites.Remove(entry);	
		} 
		
		//send a warning and quit if there are no matching portraits
		if(portraitSprites.Count == 0){
			Debug.LogError("No matching portraits found for "+characterName+". Make sure that your sprite names are formatted \"CharacterName_R_Emotion_1.png\" the L/R indicator and the number are optional. Character name is case sensitive.");
			return;
		}
		
		//Now we're trimmed down to just the portraits we care about, so everything should be prepped for conversion.
		List<RichSprite> richSprites = new List<RichSprite>();
		
		//build a library of richSprites- sprites with metadata
		foreach(var sprite in portraitSprites){
			var spriteMatch = Regex.Match(sprite.name, "^("+characterName+")_(?:([LRlr])_)?([A-Za-z]+)(?:_(\\d+))?$");
			
			RichSprite newRich = new RichSprite();
			
			newRich.sprite = sprite;
			
			newRich.name = spriteMatch.Groups[1].ToString();
			
			if(spriteMatch.Groups[2].ToString() != ""){
				newRich.handed = spriteMatch.Groups[2].ToString().ToUpper();
			} else {
				newRich.handed = "L";
			}
			
			newRich.emote = spriteMatch.Groups[3].ToString().ToLower();
			
			if(spriteMatch.Groups[4].ToString() != ""){
				try{
					newRich.index = int.Parse(spriteMatch.Groups[4].ToString());
				}catch{
					newRich.index = -1;
					Debug.LogWarning("cannot convert \""+spriteMatch.Groups[4].ToString()+"\" to an int.");
				}
			} else {
				newRich.index = -1;
			}
			richSprites.Add(newRich);
		}
		
		//start making the speakerdata. Breadth first fill.
		
		List<String> leftEmotes = new List<string>();
		List<String> rightEmotes = new List<string>();
		foreach(var spr in richSprites){
			if(spr.handed == "L"){
				if(spr.emote!="idle"){
					if(!leftEmotes.Contains(spr.emote)){
						leftEmotes.Add(spr.emote);
					}
				}
			} else {
				if(spr.emote!="idle"){
					if(!rightEmotes.Contains(spr.emote)){
						rightEmotes.Add(spr.emote);
					}
				}
			}
		}
		
		newSpeaker.emoteLibrary = new SpeakerData.EmotePortrait[leftEmotes.Count];
		newSpeaker.emoteLibraryRight = new SpeakerData.EmotePortrait[rightEmotes.Count];
		
		for (int i=0; i<newSpeaker.emoteLibrary.Length; i++){
			newSpeaker.emoteLibrary[i] = new SpeakerData.EmotePortrait();
		}
		for (int i=0; i<newSpeaker.emoteLibraryRight.Length; i++){
			newSpeaker.emoteLibraryRight[i] = new SpeakerData.EmotePortrait();
		}
		
		
		if(rightEmotes.Count>0){
			newSpeaker.isAsymmetrical = true;
		}
		
		//emotePortrait
		//--string emote
		//--Sprite[] portrait  //I need a count of these
		
		//assign left emotes
		for (int i=0; i<leftEmotes.Count; i++){
			newSpeaker.emoteLibrary[i].emote = leftEmotes[i];
			
			//count matching portraits to make the Sprite[] Array
			int frameCount = 0;
			foreach(var rSpr in richSprites){
				if(String.Equals(rSpr.handed, "L")){
					if(String.Equals(rSpr.emote,leftEmotes[i])){
						frameCount++;
					}
				}
			}
			newSpeaker.emoteLibrary[i].portrait = new Sprite[frameCount];
			
			int j = 0;
			foreach(var rSpr in richSprites){
				if(String.Equals(rSpr.handed, "L")){
					if(String.Equals(rSpr.emote,leftEmotes[i])){
						newSpeaker.emoteLibrary[i].portrait[j] = rSpr.sprite;
						j++;
					}
				}
			}
		}
		
		//assign right emotes
		for (int i=0; i<rightEmotes.Count; i++){
		//	Debug.Log(newSpeaker.emoteLibraryRight[i].emote);
			newSpeaker.emoteLibraryRight[i].emote = rightEmotes[i];
			
			//count matching portraits to make the Sprite[] Array
			int frameCount = 0;
			foreach(var rSpr in richSprites){
				if(String.Equals(rSpr.handed, "R")){
					if(String.Equals(rSpr.emote,rightEmotes[i])){
						frameCount++;
					}
				}
			}
			newSpeaker.emoteLibraryRight[i].portrait = new Sprite[frameCount];
			
			int j = 0;
			foreach(var rSpr in richSprites){
				if(String.Equals(rSpr.handed, "R")){
					if(String.Equals(rSpr.emote,rightEmotes[i])){
						newSpeaker.emoteLibraryRight[i].portrait[j] = rSpr.sprite;
						j++;
					}
				}
			}
		}
		
		//Count idle frames
		int leftIdleCount = 0;
		int rightIdleCount = 0;
		foreach(var rSpr in richSprites){
			if(String.Equals(rSpr.handed, "L")){
				if(String.Equals(rSpr.emote,"idle")){
					leftIdleCount++;
				}
			} else {
				if(String.Equals(rSpr.emote,"idle")){
					rightIdleCount++;
				}
			}
		}
		newSpeaker.idleLeft.portrait = new Sprite[leftIdleCount];
		newSpeaker.idleRight.portrait = new Sprite[rightIdleCount];
		//propogate idle 
		int leftIdleIndex=0;
		int rightIdleIndex=0;
		foreach(var rSpr in richSprites){
			if(String.Equals(rSpr.handed, "L")){
				if(String.Equals(rSpr.emote,"idle")){
					newSpeaker.idleLeft.portrait[leftIdleIndex] = rSpr.sprite;
					leftIdleIndex++;
				}
			} else {
				if(String.Equals(rSpr.emote,"idle")){
					newSpeaker.idleRight.portrait[rightIdleIndex] = rSpr.sprite;
					rightIdleIndex++;
				}
			}
		}
		
		
		
	}
	
	class RichSprite{
		public string name;
		public string handed;
		public string emote;
		public int index;
		public Sprite sprite;
	}
}
}
