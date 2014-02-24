using UnityEngine;
using System.Collections;

public class SpriteSheet { 
	
	// The name of this spritesheet
	public string sheetName = "SomethingUnique";
	
	public int firstgid = 0;
	
	public int frame_count = 0;
	
	// The width of the texture
	public int textureWidth = 0;
	
	// The height of the texture
	public int textureHeight = 0;
	
	// The texture reference
	public Texture2D spriteTexture;
	
	// The material to use
	public Material sharedMaterial;
	
	//public float renderLayer = 0;
	
	// The file to import
	// WE DO NOT WANT TO REFERENCE THIS CRAP
	//public TextAsset spriteDataFile;
	
	public FrameData[] frames = new FrameData[] {new FrameData()};
	
	public void destroy() {
		spriteTexture = null;
		sharedMaterial = null;
	}
}