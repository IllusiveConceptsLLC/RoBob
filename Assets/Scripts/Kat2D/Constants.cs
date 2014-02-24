using System;

public class Constants {
	// All static shit
	
	// Define ITEM types for the system.
	public enum ITEM_TYPES {
		TILE,
		OBJECT,
		COLLIDER_SQUARE,
		COLLIDER_RAMP,
		NONE
	}
	
	// Define all layers available
	public enum LAYER_TYPES {
		BACKGROUND_1,
		BACKGROUND_2,
		OBJECTS,
		FOREGROUND_1,
		FOREGROUND_2,
		NONE,
		COLLIDERS
	}
	
	// convert layer names to their zindex
	public static float LayerNameToZIndex(Constants.LAYER_TYPES layerType){
		switch(layerType){
		case Constants.LAYER_TYPES.BACKGROUND_1:
			return 500;
		case Constants.LAYER_TYPES.BACKGROUND_2:
			return 400;
		case Constants.LAYER_TYPES.OBJECTS:
			return 300;
		case Constants.LAYER_TYPES.COLLIDERS:
			return 0;
		case Constants.LAYER_TYPES.FOREGROUND_1:
			return 200;
		case Constants.LAYER_TYPES.FOREGROUND_2:
			return 100;
		} 
		return 0;
	}
	
	public static float ColliderZSize = 200;
	
	// hmm
	
	public static int COLLIDER_BOX_LAYER = 8;
	
	public static int COLLIDER_STAIR_LAYER = 9;
	
	public static string TAG_COLLIDER_BOX = "ColliderBox";
	public static string TAG_COLLIDER_STAIR = "ColliderStair";
	public static string TAG_COLLIDER_PLATFORM = "ColliderPlatform";
}