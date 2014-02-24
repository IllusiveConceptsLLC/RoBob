using System;
using UnityEngine;
using System.Collections.Generic;

public class ItemData{
	public Constants.ITEM_TYPES Type {get; set;}
	//public List<string> SpriteSheets {get; set;}
	public string SpriteSheet {get; set;}
	public string SheetPath {get; set;}
	public int TargetFrame {get; set;}
	public Sprite.ANCHOR_POINT AnchorPoint {get; set;}
	public float PositionX {get; set;}
	public float PositionY {get; set;}
	public bool InvertX {get; set;}
	public bool InvertY {get; set;}
	public float ScaleX {get; set;}
	public float ScaleY {get; set;}
	public float Rotation {get; set;}
	
	// if object, can use prefab key in the GO Factory
	public string PrefabKey {get; set;}
	
	public ItemData () {
		// good idea to atleast default some of these
		Type = Constants.ITEM_TYPES.NONE;
		// Set default for backwards compat
		SheetPath = "TileSheets";
		Rotation=0;
	}
	
	public ItemData (Sprite sprite, Constants.ITEM_TYPES tpe) {
		// Build item data based off of the sprite it is using...
		Type = tpe;
		AnchorPoint = sprite.AnchorPoint;
		ScaleX = sprite.scale_x;
		ScaleY = sprite.scale_y;
		Vector3 pos = sprite.getPosition();
		PositionX = pos.x;
		PositionY = pos.y;
		InvertX = sprite.invert_x;
		InvertY = sprite.invert_y;
		Rotation = sprite.rotation;
		// we only need the frame and sheet name if it is tile.
		//if(tpe.Equals(Constants.ITEM_TYPES.TILE)){
		TargetFrame = sprite.target_frame;
		SpriteSheet = sprite.getSpriteSheetName();
		SheetPath = sprite.getSpriteSheetPath();
		//}
	}
}