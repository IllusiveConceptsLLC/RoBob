using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine; 

[XmlRoot("Room")]
public class RoomData
{
	private bool isDirty = false;
	
	public string Name {get; set;}
	public float PositionX {get; set;}
	public float PositionY {get; set;}
	public float Height {get; set;}
	public float Width {get; set;}
	
	private List<LayerData> Layers {get; set;}
	
	//private LayerData colliderLayer {get; set;}
	
	private GameObject parentRoomObject = null;
	
	public GameObject getRoomObject() {
		return this.parentRoomObject;
	}
	
	public void setIsDirty(bool dirty){
		this.isDirty = dirty;
	}
	
	public RoomData () {
		this.Layers = new List<LayerData>();
		//this.colliderLayer = new LayerData(Constants.LAYER_TYPES.COLLIDERS);
	}
	
	public void addLayer(LayerData ld){
		ld.Name = ld.layerType + "";
		this.Layers.Add(ld);
	}
	public void setLayers(List<LayerData> ldl){
		this.Layers = ldl;
	}
	public List<LayerData> getLayers(){
		return this.Layers;
	}
	public void setSelected(bool sel){
		if(sel){
			gridMaker.setFrame(3);
		}else{
			gridMaker.setFrame(0);
		}
	}
	
	/*public LayerData getColliderLayer(){
		return this.colliderLayer;
	}
	public void setColliderLayer(LayerData ld){
		this.colliderLayer = ld;
	}*/
	
	public LayerData getLayerByType(Constants.LAYER_TYPES lType){
		if(Layers != null){
			foreach(LayerData ld in Layers){
				if(ld.layerType.Equals(lType)){
					return ld;
				}
			}
		}
		return null;
	}
	
	private GameObject grid = null;
	private GridMaker gridMaker = null;
	
	public void GenerateRoom(){
		
		if(parentRoomObject == null){
			parentRoomObject = new GameObject();
			parentRoomObject.name = "Room"+this.PositionX+"_"+this.PositionY;
			parentRoomObject.transform.localScale = new Vector3(1,1,1);
			parentRoomObject.transform.position = new Vector3(this.PositionX, this.PositionY, 0);
		}
		
		if(grid != null){
			//GameObject.DestroyImmediate(grid);
			//grid = null;
		}else{
			grid = new GameObject();
			grid.name = "DisplayGrid";
			grid.transform.parent = parentRoomObject.transform;
			gridMaker = (GridMaker)grid.AddComponent(typeof(GridMaker));
			gridMaker.posx = this.PositionX;
			gridMaker.posy = this.PositionY;
			gridMaker.width = this.Width;
			gridMaker.height = this.Height;
		}
		if(Layers != null){
			int ix = 0;
			while(ix < Layers.Count){
				Layers[ix].GenerateLayer(this);
				ix++;
			}
		}
	}
	
	public void TearDown() {
		// need to delete everything...
		if(isDirty){
			// save it
		}
		if(gridMaker != null){
			gridMaker.destroy();
			gridMaker = null;
		}
		if(Layers != null){
			int ix = 0;
			while(ix < Layers.Count){
				Layers[ix].TearDown();
				ix++;
			}
		}
		if(parentRoomObject != null){
			GameObject.DestroyImmediate(parentRoomObject);
		}
	}
}