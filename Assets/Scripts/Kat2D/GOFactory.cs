using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GOFactory {
	// This class will contain a definition for every available object
	// that can be created by God.
	private static GOFactory instance = null;
	//private Dictionary<string, GameObject> objectMap = new Dictionary<string, GameObject>();
	private string blockPath = "Builder/Blender/Block/";
	private string rampPath = "Builder/Blender/Block/";
	private string genericPath = "Builder/Blender/";
	private string builderPath = "Builder/";
	private string basePath = "";
	
	private GOFactory() {
		// Hide constructor
	}
	
	public static GOFactory getInstance(){
		if(GOFactory.instance == null){
			GOFactory.instance = new GOFactory();
		}
		return GOFactory.instance;
	}
	
	public GameObject Instanciate(string name){
		string path = null;
		switch(name){
		case "Architect":
			//path = builderPath;break;
			GameObject archObject = new GameObject();
			archObject.name = "Architect";
			archObject.AddComponent(typeof(Architect));
			return archObject;
		case "Grass":
			path = blockPath;break;
		case "Brick":
			path = blockPath;break;
		case "Dirt":
			path = blockPath;break;
		case "Focus":
			path = genericPath;break;
		case "Ramp":
			path = rampPath;break;
		case "Chunker":
			path = genericPath;break;
		case "KCamera":
			path = basePath;break;
		case "TestBlock":
			path = builderPath;break;
		default:
			// Did not exist, we will return a block...
			path = blockPath;
			name = "Block";
			break;
		}
		//Debug.Log (path + name);
		GameObject go = (GameObject)MonoBehaviour.Instantiate(Resources.Load(path + name));
		if(go != null){
			go.name = name;
		}
		return go;
	}
	
	//public Texture2D getObjTexture(string name){
	//	switch(name){
	//	case "Block_Map1":
	//		return (Texture2D)MonoBehaviour.Instantiate(Resources.Load(blockPath + name));
	//	}
	//	return null;
	//}
}