using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Akitacore{
[CustomEditor(typeof(CinematicLibrary))]
public class CinematicLibraryInspector : Editor{
    
    float iconDisplaySize = 100;
    
    public override void OnInspectorGUI(){
        CinematicLibrary library = (CinematicLibrary)target;
        //DrawDefaultInspector();
        
        
        if(library.cinematicImages == null ||library.cinematicImages.Length<1){
            if(GUILayout.Button("Add Cinematic Image")){
                library.cinematicImages = AddCinematic(library.cinematicImages);
                library.cinematicImages[library.cinematicImages.Length-1].sprite = AddSprite( library.cinematicImages[library.cinematicImages.Length-1].sprite);
            }
        } else{
            for (int i=0; i<library.cinematicImages.Length;i++){
                using (var horizScopeName = new GUILayout.HorizontalScope ()){
                    EditorGUILayout.LabelField ("Name: ", GUILayout.Width(60.0f));
                    library.cinematicImages[i].name = EditorGUILayout.TextField( library.cinematicImages[i].name,GUILayout.Width(200.0f));
                    if(library.cinematicImages.Length>1){
                        if(GUILayout.Button("Delete Cinematic Image", GUILayout.Width(200.0f))){
                            library.cinematicImages = RemoveCinematic(library.cinematicImages, i);
                            break;
                        }
                    }
                }
                using (var horizScopeScale = new GUILayout.HorizontalScope ()){
                    EditorGUILayout.LabelField ("Scale: ", GUILayout.Width(60.0f));
                    library.cinematicImages[i].scale = EditorGUILayout.FloatField( library.cinematicImages[i].scale,GUILayout.Width(200.0f));
                }
                if(library.cinematicImages[i].sprite == null || library.cinematicImages[i].sprite.Length <1){
                    if(GUILayout.Button("Add Sprite")){
                        library.cinematicImages[i].sprite = AddSprite(library.cinematicImages[i].sprite);
                    }
                } else if(library.cinematicImages[i].sprite.Length <2){
                    using (var horizScopeSprites = new GUILayout.HorizontalScope ()){
                        library.cinematicImages[i].sprite[0] = (Sprite)EditorGUILayout.ObjectField(library.cinematicImages[i].sprite[0],typeof(Sprite), false, GUILayout.Width(iconDisplaySize), GUILayout.Height(iconDisplaySize));
                        using(var vertScopeButtons = new GUILayout.VerticalScope()){
                            if(GUILayout.Button("Add",GUILayout.Width(50.0f))){
                                library.cinematicImages[i].sprite = AddSprite(library.cinematicImages[i].sprite);
                            }
                            if(GUILayout.Button("Del",GUILayout.Width(50.0f))){
                                library.cinematicImages[i].sprite = RemoveSprite(library.cinematicImages[i].sprite);
                                break;
                            }
                        }
                    }
                } else{
                    using (var horizScopeSprites = new GUILayout.HorizontalScope ()){
                        for (int j=0; j<library.cinematicImages[i].sprite.Length; j++){
                            library.cinematicImages[i].sprite[j] = (Sprite)EditorGUILayout.ObjectField(library.cinematicImages[i].sprite[j],typeof(Sprite), false, GUILayout.Width(iconDisplaySize), GUILayout.Height(iconDisplaySize));
                        }
                        using(var vertScopeButtons = new GUILayout.VerticalScope()){
                            if(GUILayout.Button("Add",GUILayout.Width(50.0f))){
                                library.cinematicImages[i].sprite = AddSprite(library.cinematicImages[i].sprite);
                            }
                            if(GUILayout.Button("Del",GUILayout.Width(50.0f))){
                                library.cinematicImages[i].sprite = RemoveSprite(library.cinematicImages[i].sprite);
                                break;
                            }
                             using (var horizScopeFramerate = new GUILayout.HorizontalScope ()){
                                EditorGUILayout.LabelField ("Framerate: ", GUILayout.Width(70.0f));
                                library.cinematicImages[i].framerate = EditorGUILayout.FloatField( library.cinematicImages[i].framerate,GUILayout.Width(100.0f));
                            }
                        }
                    }
                }
                EditorGUILayout.LabelField ("---------------------------------------------------------------------------------------------------------------------------");
            }
            if(GUILayout.Button("Add Cinematic Image")){
                library.cinematicImages = AddCinematic(library.cinematicImages);
                //Debug.Log(library.cinematicImages.Length);
                //Debug.Log( library.cinematicImages[library.cinematicImages.Length-1]);
                library.cinematicImages[library.cinematicImages.Length-1].sprite = AddSprite( library.cinematicImages[library.cinematicImages.Length-1].sprite);
            }
        }
        EditorUtility.SetDirty(library);
    }
    
    
    
    
    
    Sprite[] AddSprite(Sprite[] spriteArray){
        if(spriteArray == null||spriteArray.Length==0){
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
    
    
    CinematicLibrary.CinematicImage[] AddCinematic(CinematicLibrary.CinematicImage[] cinematics){
        if(cinematics == null||cinematics.Length==0){
            return  new CinematicLibrary.CinematicImage[1];
        }
        
        CinematicLibrary.CinematicImage[] newArray = new CinematicLibrary.CinematicImage[cinematics.Length+1];
        for (int i=0; i<cinematics.Length; i++){
            newArray[i] = cinematics[i]; 
        }
        newArray[cinematics.Length] = new CinematicLibrary.CinematicImage();
        return newArray;
    }


    CinematicLibrary.CinematicImage[] RemoveCinematic(CinematicLibrary.CinematicImage[] cinematics, int index){
        if(cinematics.Length<2){
            return new CinematicLibrary.CinematicImage[0]; 
        }
        
        CinematicLibrary.CinematicImage[] newArray = new CinematicLibrary.CinematicImage[cinematics.Length-1];
        for (int i=0; i<cinematics.Length-1; i++){
            if(i<index){
                newArray[i] = cinematics[i]; 
            } else {
                newArray[i] = cinematics[i+1]; 
            }
        }
        return newArray;
    }
    
}
}
