using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class PalletWindow : IBuilderWindow{
	string[] availableSpriteSheets;
	
	
	
	SpriteSheet curSheetObject = null;
	
	
	Texture2D[] textures = null;
	
	string SscaleX = "0";
	float scaleX = 1;
	string SscaleY = "0";
	float scaleY = 1;
	bool invertX = false;
	bool invertY = false;
	private int selType = -1;
	private int selColliderType = 0;
	private float selColliderRotation = 0;
	private int curFrame = -1;
	//private int selFrame = -1;
	
	private Constants.LAYER_TYPES curLayer = Constants.LAYER_TYPES.NONE;
	
	//string selSheet = "";
	string curSheet = "";
	
	string[] types = {"Tiles", "Objects", "Colliders", "Delete"};
	
	string[] colliders = {"Block"}; //, "Ramp"};
	
	public override void Update() {
		if(!curSheet.Equals(architect.sheetname)){
			architect.setSheetName(curSheet);
			loadTextures();
		}
		architect.setStampRotation(selColliderRotation);
		architect.setColliderType(selColliderType);
		architect.setStampType(selType);
		architect.setInvertX(invertX);
		architect.setInvertY(invertY);
		architect.setScaleX(scaleX);
		architect.setScaleY(scaleY);
		architect.setCurrentFrame(curFrame);

		
		if(selType == 1){
			// if we select objects, force the layer change.
			architect.getSceneManager().setCurrentLayer(Constants.LAYER_TYPES.OBJECTS);
			
		}else if(selType == 2){
			// if we select colliders, force the layer change.
			architect.getSceneManager().setCurrentLayer(Constants.LAYER_TYPES.COLLIDERS);
		}else if(selType == 0){
			if(curLayer == Constants.LAYER_TYPES.COLLIDERS || curLayer == Constants.LAYER_TYPES.OBJECTS){
				// DO NOT ALLOW US TO LOOK AT COLLIDERS HERE
				curLayer = Constants.LAYER_TYPES.NONE;
			}
			architect.getSceneManager().setCurrentLayer(curLayer);
		}else{
			architect.getSceneManager().setCurrentLayer(curLayer);
		}
	}
	
	public PalletWindow(Architect g){
		architect = g;
		title = "Pallet Manager"; 
		setPosition(0);
		loadSpriteSheets();
		
		// Set default values here. 
		curSheet = architect.sheetname;
		curFrame = (int)architect.currentframe;
		invertX = architect.invertX;
		invertY = architect.invertX;
		scaleX = architect.scaleX;
		scaleY = architect.scaleY;
		selType = architect.stampType;
		curLayer = architect.getSceneManager().getCurrentLayer();
		loadTextures();
	}
	
	private void loadTextures(){
		if(curSheet == null || curSheet.Trim().Length==0){
			return;
		}
		curSheetObject = TiledSheetFactory.getSpriteSheet(curSheet);
		textures = null;
		scrollpos = Vector2.zero;
		if(curSheetObject != null){
			textures = new Texture2D[curSheetObject.frame_count];
			int ix = 0; 
			FrameData fd = null;
			while(ix < curSheetObject.frame_count){
				fd = curSheetObject.frames[ix];
				int ox = Mathf.FloorToInt(fd.position.x);
				int oy = Mathf.FloorToInt(fd.position.y);
				int wx = Mathf.FloorToInt(fd.imageSize.x);
				int wy = Mathf.FloorToInt(fd.imageSize.y);
				//Debug.Log(fd.name + " ("+ox+","+oy+") ("+wx+","+wy+")");
				Texture2D newText = new Texture2D(wx, wy);
				oy = curSheetObject.textureHeight - oy - wy;
				Color[] colors = curSheetObject.spriteTexture.GetPixels(ox, oy, wx, wy);
				newText.SetPixels(colors);
				newText.Apply();
				textures[ix] = newText;
				ix++;
			}
		}
	}
	
	public override void Create(int id)  {
		string current = "";
		
		GUILayout.BeginVertical();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Stamp Type");
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		selType= GUILayout.SelectionGrid(selType, types, 4);
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		
		if(architect.stampType==-1){
			GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Select a Stamp Type");
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		}
		
		// TILES
		if(architect.stampType==0){
			GUILayout.BeginVertical();
			float fx = 0;
			float fy = 0;
			GUILayout.BeginHorizontal();
			GUILayout.Label("Scale X: ");
			SscaleX = GUILayout.TextField(SscaleX);
			GUILayout.EndHorizontal();
			if(float.TryParse(SscaleX, out fx)){
				if(fx < 1){
					fx = 1;
				}
			}else{
				fx = 1;
			}
			scaleX = fx;
			SscaleX = fx.ToString();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("Scale Y: ");
			SscaleY = GUILayout.TextField(SscaleY);
			GUILayout.EndHorizontal();
			if(float.TryParse(SscaleY, out fy)){
				if(fy < 1){
					fy = 1;
				}
			}else{
				fy = 1;
			}
			scaleY = fy;
			SscaleY = fy.ToString();;
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("Invert X: ");
			invertX = GUILayout.Toggle(invertX, "");
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("Invert Y: ");
			invertY = GUILayout.Toggle(invertY, "");
			GUILayout.EndHorizontal();
			
			
			foreach(string sheet in availableSpriteSheets){
				GUILayout.BeginHorizontal();
				current = "";
				if(curSheet.Equals(sheet)){
					current = " * ";
				}
				bool hit = GUILayout.Button(current + sheet);
				if(hit){
					curSheet = sheet;
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
			
			if(textures != null && textures.Length > 0){
				GUILayout.BeginVertical();
				scrollpos = GUILayout.BeginScrollView(scrollpos, GUILayout.Width (screenPos.width-20), GUILayout.Height (256));
				curFrame= GUILayout.SelectionGrid(curFrame, textures, 3);
				GUILayout.EndScrollView();
				GUILayout.EndVertical();
			}
			
		}
		
		// COLLIDERS
		if(architect.stampType==2){
			GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Collider Type");
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			selColliderType= GUILayout.SelectionGrid(selColliderType, colliders, colliders.Length);
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			
			if(selColliderType != -1){
				GUILayout.BeginVertical();
				float fx = 0;
				float fy = 0;
				GUILayout.BeginHorizontal();
				GUILayout.Label("Scale X: ");
				SscaleX = GUILayout.TextField(SscaleX);
				GUILayout.EndHorizontal();
				if(float.TryParse(SscaleX, out fx)){
					if(fx < 1){
						fx = 1;
					}
				}else{
					fx = 1;
				}
				scaleX = fx;
				SscaleX = fx.ToString();
				
				GUILayout.BeginHorizontal();
				GUILayout.Label("Scale Y: ");
				SscaleY = GUILayout.TextField(SscaleY);
				GUILayout.EndHorizontal();
				if(float.TryParse(SscaleY, out fy)){
					if(fy < 1){
						fy = 1;
					}
				}else{
					fy = 1;
				}
				scaleY = fy;
				SscaleY = fy.ToString();;
				
				GUILayout.BeginHorizontal();
				GUILayout.Label("Rotation: ");
				if(GUILayout.Button("-")){
					selColliderRotation--;
					if(selColliderRotation < 0){
						selColliderRotation = 360;
					}
				}
				GUILayout.Label("" + selColliderRotation);
				if(GUILayout.Button("+")){
					selColliderRotation++;
					if(selColliderRotation > 360){
						selColliderRotation = 0;
					}
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				selColliderRotation = GUILayout.HorizontalSlider(selColliderRotation, 0, 360);
				selColliderRotation = Mathf.Floor(selColliderRotation);
				GUILayout.EndHorizontal();
				
				GUILayout.EndVertical();
			}
		}
		
			RoomData curRoom = architect.getSceneManager().getCurrentRoom(); 
			GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Layer Selection");
			GUILayout.EndHorizontal();
			if(curRoom==null){
				GUILayout.BeginHorizontal();
				GUILayout.Label("YOU MUST CHOOSE ROOM FIRST");
				GUILayout.EndHorizontal();
			}else{
				foreach(Constants.LAYER_TYPES lt in Enum.GetValues(typeof(Constants.LAYER_TYPES))){
					// if none, then fuck you...
					if(lt.Equals(Constants.LAYER_TYPES.NONE)){
						continue;
					}
					GUILayout.BeginHorizontal();
					current = "";
					if(architect.getSceneManager().getCurrentLayer().Equals(lt)){
						current = " * ";
					}
					
					LayerData ld = curRoom.getLayerByType(lt);
					if(ld != null){
						ld.setActive(GUILayout.Toggle(ld.getActive(), ""));
					}
					bool layerHit1 = GUILayout.Button(current + lt.ToString());
					if(layerHit1){
						curLayer = lt;
					}
					GUILayout.EndHorizontal();
				}
			}
			GUILayout.EndVertical();
		
	}
	
	Vector2 scrollpos = Vector2.zero;
	
	public override void CleanUp() {
		
	}
	
	public void loadSpriteSheets() {
		//Debug.Log("Load Sprite sheets:");
		availableSpriteSheets = Directory.GetFiles(Application.dataPath+"/Resources/Textures/TileSheets/", "*.txt");
		int ix = 0;
		while(ix < availableSpriteSheets.Length){
			string ff = availableSpriteSheets[ix];
			ff = Path.GetFileName(ff);
			ff = ff.Replace(".txt", "");
			availableSpriteSheets[ix] = ff;
			ix++;
		}
	}
}