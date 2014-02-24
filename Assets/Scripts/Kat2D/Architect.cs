using UnityEngine;
using System.Collections;

public class Architect : MonoBehaviour { 
	
	public enum State {
		FreeMove, 
		StartManageRooms,
		EndManageRooms,
		StartCreateRoom,
		EndCreateRoom,
		StartSceneLoader,
		ObjectSelection 
	}
	private State currentState = State.FreeMove;
	private State nextState = State.FreeMove;
	 
	private bool canControlStamp = true;
	
	private bool canHideCursor = true;
	
	// screenPoint
	// This variable is used during the dragging logic
	Vector3 screenPoint = Vector3.zero; 

	// worldOffset
	// This variable is used during the dragging logic 
	Vector3 worldOffset = Vector3.zero;
	
	// screenPoint
	// This variable is used during the dragging logic
	Vector3 initOffset = Vector3.zero;
	
	// isDragging
	// This will be true if the user is currently dragging the screen. 
	bool isDragging = false;	
	
	// zoomSpeed
	// This will control how much we zoom in and out once triggered.
	float zoomSpeed = 100;
	// zoomDirection
	// This will contain the zoom input direction.
	float zoomDirection = 0;
	
	Vector3 gridPos = Vector3.zero;
	
	//private float clickTimeout = 0;
	
	SceneManager sceneManager = null;
	
	// variable to determine if the cursor is over a GUI window or not...
	bool onWindow = false;
	
	// Generic GUI builder window
	IBuilderWindow builderWindow = null;
	
	IBuilderWindow builderWindow2 = null;
	
	IBuilderWindow sceneLoaderWindow = null;
	
	Engine engine = null;
	
	GameObject stampGO = null;
	Sprite stampSprite = null;
	
	public bool stampDirty = false;
	
	public int colliderType = -1;
	public void setColliderType(int ct){
		if(colliderType!=ct){
			
			colliderType = ct;
			stampDirty = true;
		}
	}
	
	
	public int stampType = -1;
	public void setStampType(int st){
		if(stampType!=st){
			stampType = st;
			stampDirty = true;
		}
	}
	
	public float stampRotation = 0;
	public void setStampRotation(float rot){
		if(stampRotation != rot){
			stampRotation = rot;
			stampDirty = true;
		}
	}
	
	public string sheetname = "";
	public void setSheetName(string ss){
		if(!sheetname.Equals(ss)){
			sheetname = ss;
			stampDirty = true;
		}
	}
	
	public float currentframe = -1;
	public void setCurrentFrame(float cf){
		if(currentframe != cf){
			currentframe = cf;
			stampDirty = true;
		}
	}
	
	public bool invertX = false;
	public void setInvertX(bool ix){
		if(invertX != ix){
			invertX = ix;
			stampDirty = true;
		}
	}
	
	public bool invertY = false;
	public void setInvertY(bool iy){
		if(invertY != iy){
			invertY = iy;
			stampDirty = true;
		}
	}
	
	public float scaleX = 1;
	public void setScaleX(float sx){
		if(scaleX != sx){
			scaleX = sx;
			stampDirty = true;
		}
	}
	
	public float scaleY = 1;
	public void setScaleY(float sy){
		if(scaleY != sy){
			scaleY = sy;
			stampDirty = true;
		}
	}
	
	public void configureStamp(){
		if(stampDirty){
			if(stampType < 0){
				stampSprite.gameObject.SetActive(false);
			}else if(stampType == 0){
				if(this.sheetname == null || this.sheetname.Equals("") || this.currentframe < 0){
					stampSprite.gameObject.SetActive(false);
				}else{
					stampSprite.setRotation(0);
					stampSprite.gameObject.SetActive(true);
					stampSprite.setInvertX(this.invertX);
					stampSprite.setInvertY(this.invertY);
					stampSprite.setScaleX(this.scaleX);
					stampSprite.setScaleY(this.scaleY);
					stampSprite.setTargetFrame((int)this.currentframe);
					stampSprite.setSpriteSheetName(this.sheetname);
					stampSprite.setSpriteSheetPath("TileSheets");
				}
			// Collider
			}else if(stampType == 2){
				
				// Block
				if(colliderType==0){
					stampSprite.setRotation(this.stampRotation);
					stampSprite.gameObject.SetActive(true);
					stampSprite.setInvertX(false);
					stampSprite.setInvertY(false);
					stampSprite.setScaleX(this.scaleX);
					stampSprite.setScaleY(this.scaleY);
					stampSprite.setTargetFrame(4);
					stampSprite.setSpriteSheetName("Grid");
					stampSprite.setSpriteSheetPath("EditorSheets");
				// Ramp
				/*} else if(colliderType==1){
					stampSprite.gameObject.SetActive(true);
					stampSprite.setInvertX(this.invertX);
					stampSprite.setInvertY(this.invertY);
					stampSprite.setScaleX(this.scaleX);
					stampSprite.setScaleY(this.scaleY);
					stampSprite.setTargetFrame(5);
					stampSprite.setSpriteSheetName("Grid");
					stampSprite.setSpriteSheetPath("EditorSheets");*/
				} else {
					stampSprite.gameObject.SetActive(false);
				}
			// DELETE
			}else if(stampType==3){
				stampSprite.setRotation(0);
				stampSprite.gameObject.SetActive(true);
				stampSprite.setInvertX(false);
				stampSprite.setInvertY(false);
				stampSprite.setScaleX(1);
				stampSprite.setScaleY(1);
				stampSprite.setTargetFrame(2);
				stampSprite.setSpriteSheetName("Grid");
				stampSprite.setSpriteSheetPath("EditorSheets");
			}
		}
	}
	
	public void setEngine(Engine e){
		this.engine = e;
	}
	
	private ArchitectData architectData = null;
	
	private void Save(){
		Utility.saveObjectToXML(Application.dataPath+"/", "Architect.xml", this.architectData);
	}
	
	void Start () {
		
		architectData = (ArchitectData)Utility.loadXMLToObject(Application.dataPath + "/Architect.xml", typeof(ArchitectData));
		if(architectData == null){
			architectData = new ArchitectData();
		}
		
		sceneManager = SceneManager.getInstance();
		
		// TODO: Fix this, why TestBlock??
		//genPreviewObject("TestBlock"); // Call it the stamp, mabye
		stampGO = new GameObject();
		stampSprite = (Sprite)stampGO.AddComponent(typeof(Sprite));
		stampGO.name = "Stamp";
		
		if(architectData.lastScene == null || architectData.lastScene.Length==0){
		// Loads scene with this current scene
			sceneManager.loadScene(Application.loadedLevelName);
		}else{
			sceneManager.loadScene(architectData.lastScene);
			if(architectData.lastRoom != null && architectData.lastRoom.Length>0){
				//sceneManager.loadRoom(architectData.lastRoom);
				this.loadRoom(sceneManager.getRoomByName(architectData.lastRoom));
			}
		}
		
	}
	
	public int getStampFrame(){
		if(stampSprite != null){
			return stampSprite.target_frame;
		}
		return -1;
	}
	public string getStampSheet(){
		if(stampSprite != null && stampSprite.getSpriteSheetName() != null){
			return stampSprite.getSpriteSheetName();
		}
		return "";
	}
	
	public void clearStamp(){
		stampSprite.Clear();
	}
	public void disableStamp(bool d){
		stampGO.gameObject.SetActive(d);
	}
	
	public void setDeleteStamp(){
		stampSprite.setSpriteSheetName("Grid");
		stampSprite.setSpriteSheetPath("EditorSheets");
		stampSprite.setTargetFrame(2);
	}
	
	void hideCursor() {
		if(canHideCursor){
			Screen.showCursor = false;
			//Screen.lockCursor = true;
		}
	}
	
	void showCursor(){
		Screen.showCursor = true;
		//Screen.lockCursor = false;
	}
	
	// registerZoom
	// This method will populate the zoomDirection variable based on user input.
	void registerZoom(){
		// Check scroll wheel for zooming. 
		zoomDirection = (Input.GetAxis("Mouse ScrollWheel")*-1);
		// If no input detected, we will try the + and - keys. Someone might not have
		// a scroll mouse or prefer these keys.
		if(zoomDirection != 0){
			if(Input.GetKeyDown(KeyCode.Equals)){
				zoomDirection = -1;
			}else if(Input.GetKeyDown(KeyCode.Minus)){
				zoomDirection = 1;
			}
		}
	}
	
	private string mouse0click = "";
	
	// doZoom ()
	// This method will adjust the camera distance using the zoomDirection variable.
	void doZoom() {
		if(engine.getKCamera() != null){
			engine.getKCamera().Zoom(zoomDirection * zoomSpeed);
		}
	}
	
	// doDrag
	// This method checks for mouse drag events and if they are present, moves the world.
	void doDrag() {
		
		if(Input.GetMouseButtonDown(2)){
			// Get screen point of the game object. 
			screenPoint = engine.getCamera().WorldToScreenPoint(transform.position); 
			
			// Set both init offset and world offset as game object position - world position of the mouse cursor
			initOffset = worldOffset = transform.position - engine.getCamera().ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

			isDragging = true;
		}else if(Input.GetMouseButtonUp(2)){	
			// Stop dragging
			isDragging = false;
		}else if(isDragging && Input.GetMouseButton(2)){
			// From here, we can determine drag.
			// Get screen point of the game object. 
			screenPoint = engine.getCamera().WorldToScreenPoint(transform.position); 
			
			// Get the current screen position of the mouse
			Vector3 curMousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
			
			// Set the world offset as game object position - world position of the mouse cursor
			worldOffset = transform.position - engine.getCamera().ScreenToWorldPoint(curMousePosition);
			
			// Calculate the new position as current world position, plus the diff of init and world offset.
			// This ensures that the object stays the same distance from your mouse pointer while dragging.
			// This just seems to feel "right".
			Vector3 newPosition = transform.position + (worldOffset - initOffset);
			newPosition.x = Mathf.Floor(newPosition.x);
			newPosition.y = Mathf.Floor(newPosition.y); 
			transform.position = newPosition;
			
			// Set the init offset to the current world offset, that way we can have a diff on the next loop.
			initOffset = worldOffset;
		}else{
			// no clicks, no button down, so stop dragging
			isDragging = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		//hideCursor();
		
		configureStamp();
		
		mouse0click += Time.deltaTime;
		
		if(nextState != currentState){
			currentState = nextState;
			// Set the states and reset this bitch
			//return;
		}
		
		// Global key swtiches for the state machine. 
		if(Input.GetKeyDown(KeyCode.P)){
			sceneManager.Save();
			this.Save();
		}
		
		if(Input.GetKeyDown(KeyCode.L)){
			if(currentState == State.StartSceneLoader){
				cleanSceneLoaderWindow();
				setNextState(State.FreeMove);
			}else{
				setNextState(State.StartSceneLoader);
			}
			return;
		}
		
		// Create room
		if(Input.GetKeyDown(KeyCode.C)){
			if(currentState == State.StartCreateRoom){
				setNextState(State.FreeMove);
			}else{
				setNextState(State.StartCreateRoom);
			}
		}
		
		// Global key swtiches for the state machine. 
		if(Input.GetKeyDown(KeyCode.R)){
			if(currentState == State.StartManageRooms || currentState == State.StartCreateRoom){
				setNextState(State.FreeMove);
			}else{
				setNextState(State.StartManageRooms);
			}
		}
		
		// Setup state machine...
		switch(currentState){
		case State.StartSceneLoader:
			if(sceneLoaderWindow != null){
				sceneLoaderWindow.Update();
			}
			break;
		case State.FreeMove:
			// if we are removing all windows, delete them bitches. ...
			cleanBuilderWindow1();
			cleanBuilderWindow2();
			break;
		//case State.EditObjectTexture:
		//	showCursor();
		//	break;
		case State.StartManageRooms:
			if(sceneManager.getRoomCount() == 0){
				setNextState(State.StartCreateRoom);
				break;
			}
			if(builderWindow != null){
				builderWindow.Update();
			}
			if(builderWindow2 != null){
				builderWindow2.Update();
			}
			break;
		case State.StartCreateRoom:
			break;
		case State.EndCreateRoom:
			// Cleanup after room window is success
			cleanBuilderWindow1();
			if(sceneManager.getRoomCount() == 0){
				setNextState(State.FreeMove);
			}else{
				setNextState(State.StartManageRooms);
			}
			break;
		case State.ObjectSelection:
			break;
		}
		
		onWindow = false;
		
		if(builderWindow != null){
			onWindow = builderWindow.isMouseOver();
		}
		if(!onWindow && builderWindow2 != null){
			onWindow = builderWindow2.isMouseOver();
		}
		
		if(!onWindow && sceneLoaderWindow != null){
			onWindow = sceneLoaderWindow.isMouseOver();
		}
		
		// If we are not currently over a menu, we will allow the user to click 
		// and do shit on the screen...
		if(!onWindow){
			registerZoom();	
			doZoom();
			UpdateStamp();
			doDrag();
			
			// If there are no rooms, we can't have a stamp now can we.. fuckk.
			if(sceneManager.getRoomCount() == 0){
				//stampGO.SetActive(false);
				return;
			}

			// If right click, we are going to disable the stamp from following..
			if (Input.GetMouseButtonDown(1)){
				canControlStamp = !canControlStamp;
				
			}
			// If left click, add current object.
	        if (Input.GetMouseButton(0)){
				// If tile
				if(stampType == 0){
					if(!mouse0click.Equals(stampSprite.getPosition().x + "_" + stampSprite.getPosition().y)){
						mouse0click = stampSprite.getPosition().x + "_" + stampSprite.getPosition().y;
						ItemData item = new ItemData(stampSprite, Constants.ITEM_TYPES.TILE);
						sceneManager.AddItemToRoom(item);
						sceneManager.processCurrentRoom();
					}
				}
				// if collider
				if(stampType == 2){
					if(!mouse0click.Equals(stampSprite.getPosition().x + "_" + stampSprite.getPosition().y)){
						mouse0click = stampSprite.getPosition().x + "_" + stampSprite.getPosition().y;
						if(colliderType==0){
							ItemData item = new ItemData(stampSprite, Constants.ITEM_TYPES.COLLIDER_SQUARE);
							sceneManager.AddItemToRoom(item);
							sceneManager.processCurrentRoom();
						}else if(colliderType==1){
							ItemData item = new ItemData(stampSprite, Constants.ITEM_TYPES.COLLIDER_RAMP);
							sceneManager.AddItemToRoom(item);
							sceneManager.processCurrentRoom();
						}else{
							// it is nothing...
						}
					}
				}
				// Delete
				if(stampType==3){
					sceneManager.RemoveItemFromRoom(stampSprite.getPosition().x , stampSprite.getPosition().y);
					sceneManager.processCurrentRoom();
				}
			}
	     	// If right click, delete object. if (Input.GetMouseButton(1)){}
		}else{
			
		}
		
	}
	
	void OnGUI(){
		switch(currentState){
		case State.StartSceneLoader:
			if(sceneLoaderWindow == null){
				cleanSceneLoaderWindow();
				sceneLoaderWindow = new SceneLoaderWindow(this);
			}
			sceneLoaderWindow.Window();
			
			break;
		case State.StartManageRooms:
			// Do window for managing rooms
			if(builderWindow == null || builderWindow.GetType() != typeof(ManageRoomWindow)){
				cleanBuilderWindow1();
				builderWindow = new ManageRoomWindow(this);
			}
			builderWindow.Window();
			
			// Do window for managing rooms
			if(builderWindow2 == null || builderWindow2.GetType() != typeof(PalletWindow)){
				cleanBuilderWindow2();
				builderWindow2 = new PalletWindow(this);
			}
			builderWindow2.Window();
			break;
		case State.EndManageRooms:
			// Stop manage rooms
			break;
		case State.StartCreateRoom:
			// show window for Room Creation.
			cleanBuilderWindow2();
			if(builderWindow == null || builderWindow.GetType() != typeof(CreateRoomWindow)){
				cleanBuilderWindow1();
				builderWindow = new CreateRoomWindow(this);
			}
			builderWindow.Window();
			break;
		case State.EndCreateRoom:
			break;
		case State.FreeMove:
			// No rooms, tell user how to create one.
			cleanSceneLoaderWindow();
			
			int xoffset = 10;
			if(sceneManager.getRoomCount() == 0){
				GUI.Label(new Rect( 10, xoffset, 300, 20), "Press 'R' to create a Room in this scene.");
				xoffset += 20;
			}
			GUI.Label(new Rect( 10, xoffset, 300, 20), "Press 'R' to load editor.");
			xoffset += 20;
			GUI.Label(new Rect( 10, xoffset, 300, 20), "Focus- X:" + gridPos.x + "Y:"+gridPos.y+"Z:"+gridPos.z);
			xoffset += 20;
			GUI.Label(new Rect( 10, xoffset, 300, 20), "Chunk- X:" + Utility.calcChunk(gridPos.x) + "Y:"+Utility.calcChunk(gridPos.y)+"Z:"+Utility.calcChunk(gridPos.z));
			xoffset += 20;
			break;
		}
	}
	
	void cleanBuilderWindow1(){
		if(builderWindow != null){
			builderWindow.CleanUp();
			builderWindow = null;
		}
	}
	void cleanBuilderWindow2(){
		if(builderWindow2 != null){
			builderWindow2.CleanUp();
			builderWindow2 = null;
		}
	}
	void cleanSceneLoaderWindow(){
		if(sceneLoaderWindow != null){
			sceneLoaderWindow.CleanUp();
			sceneLoaderWindow = null;
		}
	}
	
	public void doBuilderWindow(int id, UnityEngine.GUI.WindowFunction func, string title){
		GUI.Window(id, new Rect(Screen.width - 256, 0, 256, Screen.height), func, title);
	}
	
	public void setNextState(State nextS){
		this.nextState = nextS;
	}
	
	/*
	void genPreviewObject(string name){
		// Destroy the preview, create new..
		if(objectPreview!=null){
			DestroyImmediate(objectPreview);
		}

		objectPreview.transform.localScale = new Vector3(Utility.TileWidth, Utility.TileHeight, Utility.TileWidth);
		objectPreviewTrans = objectPreview.transform;
		objectPreview.name = "Stamp";
		//GOEditor.Shade(objectPreview);
	}
	*/
	
	/* Goal: to draw part of a texture on the screen
	parameters:
	u1,v1,u2,v2: pixel coordinates of the textureToDraw
	x,y pixel coordinates of the box (up, left corner)
	WARNING: u2>u1 and v2>v1
	
	void DrawTextureClipped(string name, Texture2D textureToDraw,float u1, float v1,float u2,float v2,float x,float y){
		float du= u2-u1;
		float dv= v2-v1;
		GUI.BeginGroup (new Rect (x, y, du, dv));
		GUIStyle style = new GUIStyle();
		style.border.bottom=style.border.top=style.border.left=style.border.right=0;
		if(GUI.Button(new Rect(-u1, -v1, textureToDraw.width,textureToDraw.height), textureToDraw, style)){
			genxPreviewxObject(name);
			objectPreviewTrans.position = gridPos;
    	}
		GUI.EndGroup ();
	}
	*/
	void UpdateStamp(){ 
		if(!canControlStamp){
			return;
		}
		Vector3 stampPos = engine.getCamera().ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
		gridPos = 
			new Vector3(
				Mathf.Floor(stampPos.x/this.architectData.scaleX) * this.architectData.scaleX,
				(Mathf.Floor( (stampPos.y) /this.architectData.scaleY) * this.architectData.scaleY)+mouseYoffset,
				0);
		stampGO.transform.position = gridPos;
	}
	
	
	public SceneManager getSceneManager() {
		return sceneManager;
	}
	
	public void setPosition(float x, float y){
		Vector3 newPosition = transform.position;
		newPosition.x = x;
		newPosition.y = y; 
		transform.position = newPosition;
	}
	
	public void loadScene(string scene){
		this.architectData.lastScene = scene;
		this.sceneManager.loadScene(scene);
	}
	
	public float mouseXoffset = 0;
	public float mouseYoffset = 0;
	
	public void loadRoom(RoomData rd){
		if(rd == null){
			return;
		}
		this.architectData.lastRoom = rd.Name;
		this.sceneManager.setCurrentRoom(rd);
		this.setPosition(rd.PositionX + (rd.Width/2), rd.PositionY + (rd.Height/2));
		if(currentState != State.StartManageRooms){
			setNextState(State.StartManageRooms);
		}
		// 32 is a magic number..... lololol
		if( (rd.PositionY/32) % 2 == 0){
			//even
			mouseYoffset = 0;
		}else{
			//odd
			mouseYoffset = 32;
		}
	}
	
	public ArchitectData getData(){
		return architectData;
	}
}

