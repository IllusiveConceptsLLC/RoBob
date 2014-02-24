using UnityEngine;
using System.Collections.Generic;

public class TiledSheetFactory {
	
	private static Dictionary<string, SpriteSheet> tiledSheets = new Dictionary<string, SpriteSheet>();

	public static SpriteSheet getSpriteSheet(TileSheetDefinition definition){
		string key = definition.textureData.id;
		if(tiledSheets.ContainsKey(key)){
			SpriteSheet sp = tiledSheets[key];
			if(sp.spriteTexture == null){
				tiledSheets.Remove(key);
				return generateSpriteSheet(definition);
			}
			return tiledSheets[key];
		}else{
			return generateSpriteSheet(definition);
		} 
	} 

	public static SpriteSheet getSpriteSheet(string name, string path){
		if(path == null){
			return getSpriteSheet(name);
		}
		string key = path + "-" + name;
		if(tiledSheets.ContainsKey(key)){
			SpriteSheet sp = tiledSheets[key];
			if(sp.spriteTexture == null){
				tiledSheets.Remove(key);
				return generateSpriteSheet(name, path);
			}
			return tiledSheets[key];
		}else{
			return generateSpriteSheet(name, path);
		} 
	} 
	
	public static SpriteSheet getSpriteSheet(string name){
		return getSpriteSheet(name, "TileSheets");
	}

	private static SpriteSheet generateSpriteSheet(TileSheetDefinition definition){

		TextureData td = definition.textureData;
		
		SpriteSheet sp = new SpriteSheet();
		sp.sheetName = td.id;
		
		sp.textureHeight = td.height;
		sp.textureWidth = td.width;
		
		Vector2 texSize = new Vector2(sp.textureWidth, sp.textureHeight);
		sp.frame_count = td.images.Count;
		FrameData[] frames = new FrameData[sp.frame_count];
		foreach(ImageData img in td.images){
			FrameData frame = new FrameData();
			frame.name = "Sprite"+img.index;
			frame.offset = Vector2.zero;
			frame.size = new Vector2(img.width, img.height);
			frame.imageSize = new Vector2(img.width, img.height);
			
			frame.position = new Vector2(img.origin_x, img.origin_y); 
			
			frame.uv = new Vector2[4];
			float sx = frame.position.x / texSize.x;
			//float sy = 1 - ((frame.position.y + frame.size.y) / texSize.y);
			float sy = 1 - (frame.position.y / texSize.y);
			float scx = frame.size.x / texSize.x;
			float scy = frame.size.y / texSize.y;
			//Debug.Log("sx="+sx+",sy="+sy+",scx="+scx+",scy="+scy);
			
			//frame.uv[0] = new Vector2(sx, sy + scy);
			//frame.uv[1] = new Vector2(sx + scx, sy + scy);
			//frame.uv[2] = new Vector2(sx + scx, sy);
			//frame.uv[3] = new Vector2(sx, sy);
			
			frame.uv[0] = new Vector2(sx, sy);
			frame.uv[1] = new Vector2(sx + scx, sy);
			frame.uv[2] = new Vector2(sx + scx, sy-scy);
			frame.uv[3] = new Vector2(sx, sy-scy);
			frames[img.index] = frame;
		}
		sp.frames = frames;
		
		Texture2D texture = new Texture2D(sp.textureWidth, sp.textureHeight, TextureFormat.DXT1, false);
		
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		
		//Debug.Log("Loading File: " + "file://" + Application.dataPath + "/Resources/Textures/TileSheets/"+sp.sheetName+".png");
		
		//string targetFile = "file://" + Application.dataPath + "/Resources/Textures/"+path+"/"+sp.sheetName+".png";
		
		//WWW t_load = new WWW(targetFile);
		
		
		
		//t_load.LoadImageIntoTexture(texture);
		sp.spriteTexture = definition.texture;
		
		//Material newMaterial = new Material(Shader.Find("Unlit/Transparent"));
		Material newMaterial = new Material(Shader.Find("NewShader"));
		newMaterial.mainTexture = texture;
		sp.sharedMaterial = newMaterial;
		
		tiledSheets.Add(td.id, sp);
		
		return sp;
	}

	private static SpriteSheet generateSpriteSheet(string texname, string path){
		string filepath = Application.dataPath+"/Resources/Textures/"+path+"/"+texname+".txt";
		//Debug.Log("Loading: " + filepath);
		TextureData td = (TextureData)Utility.loadXMLToObject(filepath, typeof(TextureData));
		
		SpriteSheet sp = new SpriteSheet();
		sp.sheetName = texname;

		sp.textureHeight = td.height;
		sp.textureWidth = td.width;
		
		Vector2 texSize = new Vector2(sp.textureWidth, sp.textureHeight);
		sp.frame_count = td.images.Count;
		FrameData[] frames = new FrameData[sp.frame_count];
		foreach(ImageData img in td.images){
				FrameData frame = new FrameData();
				frame.name = "Sprite"+img.index;
				frame.offset = Vector2.zero;
		        frame.size = new Vector2(img.width, img.height);
		        frame.imageSize = new Vector2(img.width, img.height);
				
				frame.position = new Vector2(img.origin_x, img.origin_y); 
				
		        frame.uv = new Vector2[4];
		        float sx = frame.position.x / texSize.x;
		        //float sy = 1 - ((frame.position.y + frame.size.y) / texSize.y);
				float sy = 1 - (frame.position.y / texSize.y);
		        float scx = frame.size.x / texSize.x;
		        float scy = frame.size.y / texSize.y;
				//Debug.Log("sx="+sx+",sy="+sy+",scx="+scx+",scy="+scy);
				
	            //frame.uv[0] = new Vector2(sx, sy + scy);
	            //frame.uv[1] = new Vector2(sx + scx, sy + scy);
	            //frame.uv[2] = new Vector2(sx + scx, sy);
	            //frame.uv[3] = new Vector2(sx, sy);
	            
	            frame.uv[0] = new Vector2(sx, sy);
	            frame.uv[1] = new Vector2(sx + scx, sy);
	            frame.uv[2] = new Vector2(sx + scx, sy-scy);
	            frame.uv[3] = new Vector2(sx, sy-scy);
				frames[img.index] = frame;
		}
		sp.frames = frames;
		
		Texture2D texture = new Texture2D(sp.textureWidth, sp.textureHeight, TextureFormat.DXT1, false);
		
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		
		//Debug.Log("Loading File: " + "file://" + Application.dataPath + "/Resources/Textures/TileSheets/"+sp.sheetName+".png");
		
    	string targetFile = "file://" + Application.dataPath + "/Resources/Textures/"+path+"/"+sp.sheetName+".png";
    	
    	WWW t_load = new WWW(targetFile);
		
		
		
		t_load.LoadImageIntoTexture(texture);
		sp.spriteTexture = texture;
		
		Material newMaterial = new Material(Shader.Find("Unlit/Transparent"));
		newMaterial.mainTexture = texture;
		sp.sharedMaterial = newMaterial;
		
		tiledSheets.Add(path + "-" + texname, sp);
		
		return sp;
	}
}
