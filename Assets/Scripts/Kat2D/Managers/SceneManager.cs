using System;
using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class SceneManager {
	private static SceneManager instance = null;
	
	public SceneData currentScene = null;
	public RoomData currentRoom = null;
	public Constants.LAYER_TYPES currentLayer = Constants.LAYER_TYPES.NONE;

	
	public static SceneManager getInstance(){
		if(SceneManager.instance == null){
			SceneManager.instance = new SceneManager();
		}
		return SceneManager.instance;
	}
	
	private SceneManager () {
		// Singleton
	}

	public RoomData getRoomByName(string name){
		foreach(RoomData rd in currentScene.getRooms()){
			if(rd.Name.Equals(name)){
				return rd;
			}
		}
		return null;
	}
	
	public void losadRoom(string name){
		foreach(RoomData rd in currentScene.getRooms()){
			if(rd.Name.Equals(name)){
				this.setCurrentRoom(rd);
				return;
			}
		}
	}
	
	private SceneData createScene(string name){
		currentScene = new SceneData();
		currentScene.Name = name;
		currentScene.setRooms(new List<RoomData>());
		return currentScene;
	}
	
	public int getRoomCount(){
		if(currentScene == null || currentScene.getRooms() == null){
			return 0;
		}
		return currentScene.getRooms().Count;
	}
	
	public List<RoomData> getRooms(){
		if(currentScene == null || currentScene.getRooms() == null){
			return null;
		}
		return currentScene.getRooms();
	}
	
	public RoomData getCurrentRoom(){
		return this.currentRoom;
	}
	
	public Constants.LAYER_TYPES getCurrentLayer(){
		return this.currentLayer;
	}
	
	public void AddRoomToScene(RoomData roomToAdd){
		if(roomToAdd.getLayers() == null || roomToAdd.getLayers().Count == 0){
			AddLayersToRoom(roomToAdd);
		}
		/*if(roomToAdd.getColliderLayer() == null){
			roomToAdd.setColliderLayer(new LayerData());
		}*/
		currentScene.addRoom(roomToAdd);
		setCurrentRoom(roomToAdd);
	}
	
	private void AddLayersToRoom(RoomData roomToEdit){
		if(roomToEdit.getLayers() == null){
			roomToEdit.setLayers(new List<LayerData>());
		}
		
		if(roomToEdit.getLayerByType(Constants.LAYER_TYPES.BACKGROUND_1)==null){
			// Bacground 1 layer
			LayerData bg1 = new LayerData();
			bg1.layerType = Constants.LAYER_TYPES.BACKGROUND_1;
			roomToEdit.addLayer(bg1);
		}
		
		if(roomToEdit.getLayerByType(Constants.LAYER_TYPES.BACKGROUND_2)==null){
			// Background 2 layer
			LayerData bg2 = new LayerData();
			bg2.layerType = Constants.LAYER_TYPES.BACKGROUND_2;
			roomToEdit.addLayer(bg2);
		}
		
		if(roomToEdit.getLayerByType(Constants.LAYER_TYPES.OBJECTS)==null){
			// Object layer
			LayerData obj = new LayerData();
			obj.layerType = Constants.LAYER_TYPES.OBJECTS;
			roomToEdit.addLayer(obj);
		}
		
		if(roomToEdit.getLayerByType(Constants.LAYER_TYPES.FOREGROUND_1)==null){
			// Foregound 1 layer
			LayerData fg1 = new LayerData();
			fg1.layerType = Constants.LAYER_TYPES.FOREGROUND_1;
			roomToEdit.addLayer(fg1);
		}
		
		if(roomToEdit.getLayerByType(Constants.LAYER_TYPES.FOREGROUND_2)==null){
			// Foregound 2 layer
			LayerData fg2 = new LayerData();
			fg2.layerType = Constants.LAYER_TYPES.FOREGROUND_2;
			roomToEdit.addLayer(fg2);
		}
		
		if(roomToEdit.getLayerByType(Constants.LAYER_TYPES.COLLIDERS)==null){
			// Collider  layer
			LayerData col = new LayerData();
			col.layerType = Constants.LAYER_TYPES.COLLIDERS;
			roomToEdit.addLayer(col);
		}
	}
	
	void SetRoomDirty(RoomData roomToEdit){
		roomToEdit.setIsDirty(true);
	}
	
	public void processCurrentRoom() {
		if(this.currentRoom != null){
			this.currentRoom.GenerateRoom();
		}
	}
	
	public void processRooms() {
		// delete all KRooms
		foreach(RoomData rd in currentScene.getRooms()){
			// Need a game object for KRoom
			// Need a game object for the Grid
			//KRoom kr = new KRoom(rd);
			//kr.Create();
			rd.GenerateRoom();
		}
	}
	
	public void setCurrentRoom(RoomData rd){
		unsetCurrentRoom();
		this.currentRoom = rd;
		this.currentRoom.GenerateRoom();
		this.currentRoom.setSelected(true);
	}
	
	private void unsetCurrentRoom(){
		if(this.currentRoom != null){
			//this.currentRoom.setSelected(false);
			this.currentRoom.TearDown();
		}
	}
	
	public void setCurrentLayer(Constants.LAYER_TYPES lt){
		this.currentLayer = lt;
	}
	
	public void AddItemToRoom(ItemData item) {
		if(item.SpriteSheet == null){
			return;
		}
		//RoomData rd = currentScene.Rooms[0]; 
		if(currentRoom == null){
			// alert user to select room
			return;
		}
		if(currentLayer.Equals(Constants.LAYER_TYPES.NONE)){
			// alert user to select layer.
			return;
		}
		if(item.PositionX >= currentRoom.PositionX && item.PositionX < currentRoom.PositionX + currentRoom.Width){
			if(item.PositionY >= currentRoom.PositionY && item.PositionY < currentRoom.PositionY + currentRoom.Height){
				LayerData ld  = currentRoom.getLayerByType(currentLayer);
				if(ld != null){
					ld.addItem(item);
				}
				return;
			}
		}
	}
	
	public void RemoveItemFromRoom(float x, float y) {

		//RoomData rd = currentScene.Rooms[0];
		if(currentRoom == null){
			// alert user to select room
			return;
		}
		if(currentLayer.Equals(Constants.LAYER_TYPES.NONE)){
			// alert user to select layer.
			return;
		}
		if(x >= currentRoom.PositionX && x <= currentRoom.PositionX + currentRoom.Width){
			if(y >= currentRoom.PositionY && y <= currentRoom.PositionY + currentRoom.Height){
				LayerData ld  = currentRoom.getLayerByType(currentLayer);
				if(ld != null){
					//ld.addItem(item);
					ld.removeItem(x,y);
				}
				return;
			}
		}
	}
	
	public void Save(){
		//Debug.Log ("Saving");
		string path = Application.dataPath + "/SceneData/" + currentScene.Name + "/";
		string name = "Scene.xml";
		Utility.saveObjectToXML(path, name, currentScene);
		saveRooms(currentScene);
	}
	
	private void saveRooms(SceneData scene){
		string path = Application.dataPath + "/SceneData/" + currentScene.Name + "/Rooms/";
		List<RoomData> rooms = scene.getRooms();
		foreach(RoomData rd in rooms){
			string name = rd.Name+".xml";
			Utility.saveObjectToXML(path, name, rd);
			saveLayers(scene, rd);
		}
	}
	
	private void saveLayers(SceneData scene, RoomData rd){
		string path = Application.dataPath + "/SceneData/" + currentScene.Name + "/Rooms/" + rd.Name + "/";
		List<LayerData> layers = rd.getLayers();
		foreach(LayerData ld in layers){
			string name = ld.Name+".xml";
			Utility.saveObjectToXML(path, name, ld);
		}
	}
	
	public void loadScene(string sceneName){
		string path = Application.dataPath + "/SceneData/" + sceneName + "/";
		string name = "Scene.xml";
		currentScene = (SceneData) Utility.loadXMLToObject(path + name, typeof(SceneData));
		if(currentScene == null){
			currentScene = createScene(sceneName);
		}else{
			loadRooms(currentScene);
		}
	}
	private void loadRooms(SceneData scene){
		if(Directory.Exists(Application.dataPath + "/SceneData/" + scene.Name + "/Rooms/")){
			string[] rooms = Directory.GetFiles(Application.dataPath + "/SceneData/" + scene.Name + "/Rooms/", "*.xml");
			int ix = 0;
			while(ix < rooms.Length){
				string ff = rooms[ix];
				RoomData room = (RoomData) Utility.loadXMLToObject(ff, typeof(RoomData));
				if(room != null){
					loadLayers(scene, room);
					this.AddLayersToRoom(room);
					scene.addRoom(room);
				}
				ix++;
			}
		}
	}
	private void loadLayers(SceneData scene, RoomData rd){
		string[] layers = Directory.GetFiles(Application.dataPath + "/SceneData/" + scene.Name + "/Rooms/" + rd.Name + "/", "*.xml");
		int ix = 0;
		while(ix < layers.Length){
			string ff = layers[ix];
			LayerData layer = (LayerData) Utility.loadXMLToObject(ff, typeof(LayerData));
			
			if(layer != null){
				rd.addLayer(layer);
			}
			ix++;
		}
	}
}
