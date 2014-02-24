using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class SceneLoaderWindow : IBuilderWindow{
	
	
	string[] sceneNames = null;
	string curScene = "";
	string selScene = "";
	
	bool createScene = false;
	string sceneName = "";
	
	public override void Update() {
		if(!selScene.Equals(curScene)){
			curScene = selScene;
			architect.loadScene(curScene);//getSceneManager().loadScene(curScene);
		}
		if(createScene && sceneName.Trim().Length > 0){
			architect.getSceneManager().loadScene(sceneName);
			createScene = false;
		}
	}
	
	public SceneLoaderWindow(Architect g){
		architect = g;
		setPosition(2);
		title = "Load Scene";
		loadSceneNames();
	}
	
	public override void Create(int id)  {
		string current = " ";
		
		GUILayout.BeginVertical();
		GUILayout.BeginHorizontal();
		if(architect.getSceneManager().currentScene == null){
			GUILayout.Label("Select a Scene or Create one");
		}else{
			GUILayout.Label("Scene Selected: " + architect.getSceneManager().currentScene.Name);
		}
		
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		
		GUILayout.BeginVertical();
		GUILayout.Label("");
		GUILayout.BeginHorizontal();
		GUILayout.Label("Create Scene");
		sceneName = GUILayout.TextField(sceneName);
		sceneName = sceneName.Trim();
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		createScene = GUILayout.Button("Create Scene");
		GUILayout.EndHorizontal();
		GUILayout.Label("");
		GUILayout.EndVertical();
		
		GUILayout.BeginVertical();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Select Existing Scene");
		GUILayout.EndHorizontal();
		foreach(string sheet in sceneNames){
			GUILayout.BeginHorizontal();
			current = "";
			
			if(architect.getSceneManager().currentScene != null && architect.getSceneManager().currentScene.Name.Equals(sheet)){
				current = " * ";
			}
			bool hit = GUILayout.Button(current + sheet);
			if(hit){
				selScene = sheet;
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndVertical();
	}
	
	public override void CleanUp() {

	}
	
	private void loadSceneNames() {
		//Debug.Log("Load Sprite sheets:");
		sceneNames = Directory.GetDirectories(Application.dataPath+"/SceneData/");
		int ix = 0;
		while(ix < sceneNames.Length){
			string ff = Path.GetFileName(sceneNames[ix]);
			sceneNames[ix] = ff;
			ix++;
		}
	}
}
