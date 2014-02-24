using System;
using System.Collections.Generic;
using UnityEngine;

public class LayerData {
	public string Name {get; set;}
	public Constants.LAYER_TYPES layerType {get; set;}
	public List<ItemData> Items {get; set;}
	
	public LayerData () {
		this.Construct(Constants.LAYER_TYPES.NONE);
	}
	
	public LayerData (Constants.LAYER_TYPES lt) {
		this.Construct(lt);
	}
	
	private void Construct(Constants.LAYER_TYPES lt){
		layerType = lt;
		Items = new List<ItemData>();
	}
	
	public void removeItem(float x, float y){
		if(!active){
			return;
		}
		// check if exists...
		int ix = 0;
		int delix = -1;
		while(ix < Items.Count){
			ItemData oli = Items[ix];
			//Debug.Log(oli.PositionX +" == "+ x +" && "+ oli.PositionY +" == "+ y);
			if(oli.PositionX == x && oli.PositionY == y){
				delix = ix;
				//Debug.Log("Delete this one " + delix);
				// we found and replaced the item, return;
				break;
			}
			ix++;
		}
		if(delix > -1){
			Items.RemoveAt(delix);
		}
	}
	
	public void addItem(ItemData item){
		if(!active){
			return;
		}
		// check if exists...
		int ix = 0;
		while(ix < Items.Count){
			ItemData oli = Items[ix];
			if(oli.PositionX == item.PositionX && oli.PositionY == item.PositionY){
				Items[ix] = item;
				// we found and replaced the item, return;
				return;
			}
			ix++;
		}
		// obviously nothing was found, so just add it in
		//Debug.Log("Adding Item");
		Items.Add(item);
	}
	
	private Dictionary<string, LayerObject> layerObjects = new Dictionary<string, LayerObject>();
	
	//private List<GameObject> objects 
	
	public void GenerateLayer(RoomData room) {
		
		// Destroy all objects in here...
		foreach(KeyValuePair<string, LayerObject> item in layerObjects){
			item.Value.destroy(); 
		}
		layerObjects = new Dictionary<string, LayerObject>();
		
		foreach(ItemData item in Items){
			// Ok this generates tiles.. fuck them
			if(
				layerType == Constants.LAYER_TYPES.BACKGROUND_1
				|| layerType == Constants.LAYER_TYPES.BACKGROUND_2
				|| layerType == Constants.LAYER_TYPES.FOREGROUND_1
				|| layerType == Constants.LAYER_TYPES.FOREGROUND_2
			){
				if(layerObjects.ContainsKey(item.SpriteSheet)){
					LayerObject mo = layerObjects[item.SpriteSheet];
					mo.addItem(item);
				}else{
					GameObject mogobj = new GameObject();
					mogobj.transform.parent = room.getRoomObject().transform;
					mogobj.name = layerType+"_"+item.SpriteSheet;
					MaterialObject mo  = (MaterialObject)mogobj.AddComponent(typeof(MaterialObject));
					mo.spriteSheetName = item.SpriteSheet;
					mo.spriteSheetPath = item.SheetPath;
					mo.posx = room.PositionX;
					mo.posy = room.PositionY;
					mo.posz = Constants.LayerNameToZIndex(this.layerType);
					mo.height = room.Height;
					mo.width = room.Width;
					mo.gameObject.SetActive(this.active);
					mo.addItem(item);
					
					layerObjects.Add(item.SpriteSheet, mo);
				}
			// Colliders work differently...in that, we will have a new object for every collider...
			}else if(layerType == Constants.LAYER_TYPES.COLLIDERS){
				GameObject colObj = new GameObject();
				colObj.transform.position = new Vector3(item.PositionX, item.PositionY, Constants.LayerNameToZIndex(this.layerType));
				colObj.transform.parent = room.getRoomObject().transform;
				
				float height = 32*item.ScaleY;
				float width =32*item.ScaleX;
				
				float offset = 0;
				
							
							
				colObj.layer = Constants.COLLIDER_BOX_LAYER;
				
				if(item.Rotation != 0){
					colObj.tag = Constants.TAG_COLLIDER_STAIR;
					
					Vector3 bottomLeft = new Vector3(colObj.transform.position.x, colObj.transform.position.y + 16, 0);
					Vector3 topRight = new Vector3(colObj.transform.position.x + width, colObj.transform.position.y + height + 16,0);
					offset = Vector3.Distance(bottomLeft, topRight);
					
					if(height > width){
						colObj.transform.localScale = new Vector3(width,offset,Constants.ColliderZSize);
					}else{
						colObj.transform.localScale = new Vector3(offset,height,Constants.ColliderZSize);
					}
				}else{
					colObj.tag = Constants.TAG_COLLIDER_BOX;
					
					colObj.transform.localScale = new Vector3(width,height,Constants.ColliderZSize);
					

				}
					
				colObj.name = "Collider_"+item.PositionX+"_"+item.PositionY;
				BoxCollider bc = (BoxCollider)colObj.AddComponent(typeof(BoxCollider));
				bc.isTrigger = false;
				bc.center = new Vector3(.5f, .5f, 1.5f);	
				Sprite so = (Sprite)colObj.AddComponent(typeof(Sprite));
				so.auto_resize = false;
				so.setSpriteSheetName(item.SpriteSheet);
				so.setSpriteSheetPath(item.SheetPath);
				so.setRotation(item.Rotation);
				so.setScaleX(item.ScaleX);
				so.setScaleY(item.ScaleY);
				so.setScaleZ(Constants.ColliderZSize);
				so.setTargetFrame(item.TargetFrame);
				
				// new sprite, we need to set all of the pertinant values....
				layerObjects.Add(colObj.name, so);
			}
		}
		
		//Colliders are more fun...
		//if(){
			
		//}
		
		//if(this.layerType == Constants.LAYER_TYPES.BACKGROUND_1){
		//	Debug.Log("Generate Layer: Count: " + thisLayerMaterialObjects.Count);
		//}
	}
	
	private bool active = true;
	public bool getActive(){
		return active;
	}
	public void setActive(bool act){
		if(active == act){
			return;
		}
		//if(this.layerType == Constants.LAYER_TYPES.BACKGROUND_1){
		//	Debug.Log(thisLayerMaterialObjects.Count);
		//}
		active = act;
		foreach(KeyValuePair<string, LayerObject> item in layerObjects){
			
			item.Value.gameObject.SetActive(this.active);
		}
	}
	
	public void TearDown() {
		//Debug.Log("TEAR DOWN!!");
		foreach(KeyValuePair<string, LayerObject> item in layerObjects){
			item.Value.destroy();
		}
		layerObjects = new Dictionary<string, LayerObject>();
	}

}