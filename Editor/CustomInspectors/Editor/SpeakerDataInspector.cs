using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Akitacore{
[CustomEditor(typeof(SpeakerData))]
public class SpeakerDataInspector : Editor{
	float iconDisplaySize = 100;
	
	public override void OnInspectorGUI(){
		
		SpeakerData speakerData = (SpeakerData)target;
		
		//DrawDefaultInspector ();
		
		
		
		//--Display basic info------
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField ("Speaker ", GUILayout.Width(150.0f));
			speakerData.speaker = EditorGUILayout.TextField(speakerData.speaker);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField ("Char Is Asymmetrical ", GUILayout.Width(150.0f));
			speakerData.isAsymmetrical = EditorGUILayout.Toggle(speakerData.isAsymmetrical);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField ("Display side:", GUILayout.Width(150.0f));
			speakerData.spriteSide =  (SpeakerData.SpriteSide)EditorGUILayout.EnumPopup(speakerData.spriteSide);
		EditorGUILayout.EndHorizontal();
		
				//Display Beeble Info
		EditorGUILayout.LabelField ("----VOICE BEEBLE INFO------------------");
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField ("Beeble ", GUILayout.Width(150.0f));
			speakerData.beeble = (AudioClip)EditorGUILayout.ObjectField(speakerData.beeble, typeof(AudioClip),false);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField ("Beeble Pitch Spread ", GUILayout.Width(150.0f));
			speakerData.beeblePitchSpread = EditorGUILayout.FloatField(speakerData.beeblePitchSpread);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField ("Beeble Min Delay ", GUILayout.Width(150.0f));
			speakerData.beebleMinDelay = EditorGUILayout.FloatField(speakerData.beebleMinDelay);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField ("Beeble Max Delay ", GUILayout.Width(150.0f));
			speakerData.beebleMaxDelay = EditorGUILayout.FloatField(speakerData.beebleMaxDelay);
		EditorGUILayout.EndHorizontal();
		
		//--Display Idle Portrait info-----------------------------------------------------
		if(speakerData.isAsymmetrical){
		EditorGUILayout.LabelField ("----IDLE PORTRAIT LEFT------------------");
		} else {
		EditorGUILayout.LabelField ("----IDLE PORTRAIT------------------");
		}
		
		if(speakerData.idleLeft.portrait!=null && speakerData.idleLeft.portrait.Length>0){
			EditorGUILayout.BeginHorizontal();
			for (int i=0; i<speakerData.idleLeft.portrait.Length; i++){
			 speakerData.idleLeft.portrait[i] = (Sprite)EditorGUILayout.ObjectField(speakerData.idleLeft.portrait[i],typeof(Sprite), false, GUILayout.Width(iconDisplaySize), GUILayout.Height(iconDisplaySize));
			}
			EditorGUILayout.BeginVertical(); // Sprite add/remove buttons
			if(GUILayout.Button("Add",GUILayout.Width(50.0f))){
				speakerData.idleLeft.portrait = AddSprite(speakerData.idleLeft.portrait);

			}
			if(GUILayout.Button("Del",GUILayout.Width(50.0f))){
				speakerData.idleLeft.portrait = RemoveSprite(speakerData.idleLeft.portrait);

			}
			if(speakerData.idleLeft.portrait.Length>1){
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("FrameRate:",GUILayout.Width(75.0f));
				speakerData.idleLeft.portraitFramerate = EditorGUILayout.FloatField(speakerData.idleLeft.portraitFramerate,GUILayout.Width(100.0f));
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Always Animate:",GUILayout.Width(100.0f));
				speakerData.idleLeft.alwaysAnimate = EditorGUILayout.Toggle(	speakerData.idleLeft.alwaysAnimate);
				EditorGUILayout.EndHorizontal();
				
			}
			EditorGUILayout.EndVertical(); // end sprite add remove buttons
			
			EditorGUILayout.EndHorizontal(); 
		} else{
			if(GUILayout.Button("Add", GUILayout.Width(50.0f))){ //display button if no sprites exist
				speakerData.idleLeft.portrait = AddSprite(speakerData.idleLeft.portrait);

			}
		}
		
		//--Display idle portrait right, if asymmetrical--------------------------------------------
		if(speakerData.isAsymmetrical){
			EditorGUILayout.LabelField ("----IDLE PORTRAIT RIGHT------------------");
		
			if(speakerData.idleRight.portrait!=null && speakerData.idleRight.portrait.Length>0){
				EditorGUILayout.BeginHorizontal();
				for (int i=0; i<speakerData.idleRight.portrait.Length; i++){
				 speakerData.idleRight.portrait[i] = (Sprite)EditorGUILayout.ObjectField(speakerData.idleRight.portrait[i],typeof(Sprite), false, GUILayout.Width(iconDisplaySize), GUILayout.Height(iconDisplaySize));
				}
				EditorGUILayout.BeginVertical(); // Sprite add/remove buttons
				if(GUILayout.Button("Add",GUILayout.Width(50.0f))){
					speakerData.idleRight.portrait = AddSprite(speakerData.idleRight.portrait);

				}
				if(GUILayout.Button("Del",GUILayout.Width(50.0f))){
					speakerData.idleRight.portrait = RemoveSprite(speakerData.idleRight.portrait);

				}
				if(speakerData.idleRight.portrait.Length>1){
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("FrameRate:",GUILayout.Width(75.0f));
					speakerData.idleRight.portraitFramerate = EditorGUILayout.FloatField(speakerData.idleRight.portraitFramerate,GUILayout.Width(100.0f));
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Always Animate:",GUILayout.Width(100.0f));
					speakerData.idleRight.alwaysAnimate = EditorGUILayout.Toggle(	speakerData.idleRight.alwaysAnimate);
					EditorGUILayout.EndHorizontal();
					
				}
				EditorGUILayout.EndVertical(); // end sprite add remove buttons
				
				EditorGUILayout.EndHorizontal(); 
			} else{
				if(GUILayout.Button("Add", GUILayout.Width(50.0f))){ //display button if no sprites exist
					speakerData.idleRight.portrait = AddSprite(speakerData.idleRight.portrait);

				}
			}
		}
		
		if(speakerData.isAsymmetrical){
		EditorGUILayout.LabelField ("----EMOTES LEFT------------------");
		} else {
		EditorGUILayout.LabelField("----EMOTES------------------");
		}
		
		//--Define button to create entire emote list-----------------------------------
		if (GUILayout.Button("Use All Recognized Emotes")){
			List<SpeakerData.EmotePortrait> tempEmoLib = new  List<SpeakerData.EmotePortrait>();
			
			//add an entry for every emote
			for (int i=0; i<CoreDialogueUI.emoteLib.Count; i++){
				tempEmoLib.Add(new SpeakerData.EmotePortrait());
				tempEmoLib[i].emote = CoreDialogueUI.emoteLib[i].ascii;
			}
			
			//copy current library into temporary list
			for (int i=0; i<speakerData.emoteLibrary.Length; i++){
			 foreach (var emote in tempEmoLib){
				if(string.Equals(emote.emote, speakerData.emoteLibrary[i].emote)){
				 emote.portrait = new Sprite[speakerData.emoteLibrary[i].portrait.Length];
				 for (int j=0; j<speakerData.emoteLibrary[i].portrait.Length; j++){
					emote.portrait[j] = speakerData.emoteLibrary[i].portrait[j];
				 }
				} 
			 } 
			}
			
			//replace current library with temp
			speakerData.emoteLibrary = new SpeakerData.EmotePortrait[tempEmoLib.Count];
			for (int i=0; i<tempEmoLib.Count; i++){
				speakerData.emoteLibrary[i] = tempEmoLib[i];
			}

			//--Do it again, on the right------------------------------------------------
			if(speakerData.isAsymmetrical){
			tempEmoLib = new  List<SpeakerData.EmotePortrait>();
			
			//add an entry for every emote
			for (int i=0; i<CoreDialogueUI.emoteLib.Count; i++){
				tempEmoLib.Add(new SpeakerData.EmotePortrait());
				tempEmoLib[i].emote = CoreDialogueUI.emoteLib[i].ascii;
			}
			
			//copy current library into temporary list
			for (int i=0; i<speakerData.emoteLibraryRight.Length; i++){
			 foreach (var emote in tempEmoLib){
				if(string.Equals(emote.emote, speakerData.emoteLibraryRight[i].emote)){
				 emote.portrait = new Sprite[speakerData.emoteLibraryRight[i].portrait.Length];
				 for (int j=0; j<speakerData.emoteLibraryRight[i].portrait.Length; j++){
					emote.portrait[j] = speakerData.emoteLibraryRight[i].portrait[j];
				 }
				} 
			 } 
			}
			
			//replace current library with temp
			speakerData.emoteLibraryRight = new SpeakerData.EmotePortrait[tempEmoLib.Count];
			for (int i=0; i<tempEmoLib.Count; i++){
				speakerData.emoteLibraryRight[i] = tempEmoLib[i];
			}
			}


		}
		//Display EmoteLibraryInfo
		if(speakerData.emoteLibrary!=null && speakerData.emoteLibrary.Length>0){
		 for(int i=0; i<speakerData.emoteLibrary.Length; i++){
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField ("Emote: ", GUILayout.Width(50.0f));
				speakerData.emoteLibrary[i].emote = EditorGUILayout.TextField(speakerData.emoteLibrary[i].emote,GUILayout.Width(100.0f)); 
				if(GUILayout.Button("Delete Emote", GUILayout.Width(150.0f))){
					speakerData.emoteLibrary = RemoveEmote(speakerData.emoteLibrary, i);

					break;
				}
				EditorGUILayout.EndHorizontal();
				if(speakerData.emoteLibrary[i].portrait!=null && speakerData.emoteLibrary[i].portrait.Length>0){
				 EditorGUILayout.BeginHorizontal();
					for (int j=0; j<speakerData.emoteLibrary[i].portrait.Length; j++){
						speakerData.emoteLibrary[i].portrait[j] = (Sprite)EditorGUILayout.ObjectField(speakerData.emoteLibrary[i].portrait[j],typeof(Sprite), false, GUILayout.Width(iconDisplaySize), GUILayout.Height(iconDisplaySize));
					}
					EditorGUILayout.BeginVertical(); // Sprite add/remove buttons
					if(GUILayout.Button("Add",GUILayout.Width(50.0f))){
						speakerData.emoteLibrary[i].portrait = AddSprite(speakerData.emoteLibrary[i].portrait);

					}
					if(GUILayout.Button("Del",GUILayout.Width(50.0f))){
						speakerData.emoteLibrary[i].portrait = RemoveSprite(speakerData.emoteLibrary[i].portrait);

					}
					if(speakerData.emoteLibrary[i].portrait.Length>1){
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("FrameRate:",GUILayout.Width(75.0f));
						speakerData.emoteLibrary[i].portraitFramerate = EditorGUILayout.FloatField(speakerData.emoteLibrary[i].portraitFramerate,GUILayout.Width(100.0f));
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Always Animate:",GUILayout.Width(100.0f));
						speakerData.emoteLibrary[i].alwaysAnimate = EditorGUILayout.Toggle(speakerData.emoteLibrary[i].alwaysAnimate);
						EditorGUILayout.EndHorizontal();
					}
					EditorGUILayout.EndVertical(); // end sprite add remove buttons
					EditorGUILayout.EndHorizontal();
				} else {
						if(GUILayout.Button("Add", GUILayout.Width(50.0f))){ //display button if no sprites exist
							speakerData.emoteLibrary[i].portrait = AddSprite(speakerData.emoteLibrary[i].portrait);

						}
				}
			}
		}

		if(GUILayout.Button("Add Emote", GUILayout.Width(150.0f))){
				speakerData.emoteLibrary = AddEmote(speakerData.emoteLibrary);

		}
		
		if(speakerData.isAsymmetrical){
			//Display EmoteLibraryRightInfo
		EditorGUILayout.LabelField ("----EMOTES RIGHT------------------");
		
		if(speakerData.emoteLibraryRight!=null && speakerData.emoteLibraryRight.Length>0){
		 for(int i=0; i<speakerData.emoteLibraryRight.Length; i++){
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField ("Emote: ", GUILayout.Width(50.0f));
				speakerData.emoteLibraryRight[i].emote = EditorGUILayout.TextField(speakerData.emoteLibraryRight[i].emote,GUILayout.Width(100.0f));
				if(GUILayout.Button("Delete Emote", GUILayout.Width(150.0f))){
					speakerData.emoteLibraryRight = RemoveEmote(speakerData.emoteLibraryRight, i);

					break;
				}
				EditorGUILayout.EndHorizontal();
				if(speakerData.emoteLibraryRight[i].portrait!=null && speakerData.emoteLibraryRight[i].portrait.Length>0){
				 EditorGUILayout.BeginHorizontal();
					for (int j=0; j<speakerData.emoteLibraryRight[i].portrait.Length; j++){
						speakerData.emoteLibraryRight[i].portrait[j] = (Sprite)EditorGUILayout.ObjectField(speakerData.emoteLibraryRight[i].portrait[j],typeof(Sprite), false, GUILayout.Width(iconDisplaySize), GUILayout.Height(iconDisplaySize));
					}
					EditorGUILayout.BeginVertical(); // Sprite add/remove buttons
					if(GUILayout.Button("Add",GUILayout.Width(50.0f))){
						speakerData.emoteLibraryRight[i].portrait = AddSprite(speakerData.emoteLibraryRight[i].portrait);

					}
					if(GUILayout.Button("Del",GUILayout.Width(50.0f))){
						speakerData.emoteLibraryRight[i].portrait = RemoveSprite(speakerData.emoteLibraryRight[i].portrait);

					}
					if(speakerData.emoteLibraryRight[i].portrait.Length>1){
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("FrameRate:",GUILayout.Width(75.0f));
						speakerData.emoteLibraryRight[i].portraitFramerate = EditorGUILayout.FloatField(speakerData.emoteLibraryRight[i].portraitFramerate,GUILayout.Width(100.0f));
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Always Animate:",GUILayout.Width(100.0f));
						speakerData.emoteLibraryRight[i].alwaysAnimate = EditorGUILayout.Toggle(speakerData.emoteLibraryRight[i].alwaysAnimate);
						EditorGUILayout.EndHorizontal();
					}
					EditorGUILayout.EndVertical(); // end sprite add remove buttons
					EditorGUILayout.EndHorizontal();
				} else {
						if(GUILayout.Button("Add", GUILayout.Width(50.0f))){ //display button if no sprites exist
							speakerData.emoteLibraryRight[i].portrait = AddSprite(speakerData.emoteLibraryRight[i].portrait);

						}
				}
			}
		}

		if(GUILayout.Button("Add Emote", GUILayout.Width(150.0f))){
				speakerData.emoteLibraryRight = AddEmote(speakerData.emoteLibraryRight);
		}
			
		}
		
		
		EditorUtility.SetDirty(speakerData);
	}
	
	
	
	//----Array management Functions---------------------------------------------
	Sprite[] AddSprite(Sprite[] spriteArray){
		if(spriteArray == null){
			return new Sprite[1];
		}
		Sprite[] newArray = new Sprite[spriteArray.Length+1];
		for (int i=0; i<spriteArray.Length; i++){
		newArray[i] = spriteArray[i]; 
		}
		return newArray;
	}
	
	Sprite[] RemoveSprite(Sprite[] spriteArray){
		if(spriteArray.Length < 2){
		 return new Sprite[0]; 
		}
		
	 Sprite[] newArray = new Sprite[spriteArray.Length-1];
	 for (int i=0; i<spriteArray.Length-1; i++){
		newArray[i] = spriteArray[i]; 
	 }
	 return newArray;
	}
	
	
	SpeakerData.EmotePortrait[] AddEmote(SpeakerData.EmotePortrait[] emotePortraits){
		if(emotePortraits == null){
			return new SpeakerData.EmotePortrait[1];
		}
		
		SpeakerData.EmotePortrait[] newArray = new SpeakerData.EmotePortrait[emotePortraits.Length+1];
		for (int i=0; i<emotePortraits.Length; i++){
			newArray[i] = emotePortraits[i]; 
		}
		newArray[newArray.Length-1] = new SpeakerData.EmotePortrait();
		return newArray;
	}
	
	
	SpeakerData.EmotePortrait[] RemoveEmote(SpeakerData.EmotePortrait[] emotePortraits, int index){
		if(emotePortraits.Length<2){
			return new SpeakerData.EmotePortrait[0]; 
		}
		
		SpeakerData.EmotePortrait[] newArray = new SpeakerData.EmotePortrait[emotePortraits.Length-1];
		for (int i=0; i<emotePortraits.Length-1; i++){
			if(i<index){
				newArray[i] = emotePortraits[i]; 
			} else {
				newArray[i] = emotePortraits[i+1]; 
			}
		}
		return newArray;
	}
	
}
}
