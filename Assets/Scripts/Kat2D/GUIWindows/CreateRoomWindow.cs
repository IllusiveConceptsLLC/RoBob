using System;
using UnityEngine;

public class CreateRoomWindow : IBuilderWindow{
	
	string roomName="Room";
	int posx = 0;
	int posy = 0;
	int height = 736;
	int width = 1280;
	string error = "";
	
	private bool done = false;
	
	GridMaker gridPreview = null;
	
	public override void Update() {
	}
	
	public CreateRoomWindow(Architect g){
		
		GameObject gpo = new GameObject();
		gpo.name = "CreateGridPreview";
		gridPreview = (GridMaker)gpo.AddComponent(typeof(GridMaker));
		//gridPreview.setup(posx, posy, width, height, 2);
		architect = g;
		
		if(gridPreview.setup(posx, posy, width, height, 2)){
			// position the camera in the middle of the new room
			architect.setPosition(posx+(width/2), posy+(height/2));
		}
		
		title = "Create Room";
	}
	
	public override void Create(int id)  {
		int xpos = 40;
		bool doCreate = false;
		GUILayout.BeginVertical();
		
			if(!error.Equals("")){
				GUILayout.BeginHorizontal();
					// Room Name
					GUI.Label(new Rect(10, 20, 300, 20), "ERROR: "+error);
				GUILayout.EndHorizontal();
			}
			GUILayout.BeginHorizontal();
				// Room Name
				roomName = "Room"+posx+"_"+posy;
				GUI.Label(new Rect(10, xpos, 100, 20), "Room Name:");
				//roomName = GUI.TextField(new Rect(100, xpos, 100, 20), roomName);
				GUI.Label(new Rect(100, xpos, 100, 20), roomName);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
				// Position X
				xpos += 30;
				GUI.Label(new Rect(10, xpos, 100, 20), "Position X:");
				if(GUI.Button(new Rect(90, xpos, 20, 20), "-")){
					posx -= 1280;
				}
				if(GUI.Button(new Rect(210, xpos, 20, 20), "+")){
					posx += 1280;
				}
				GUI.TextField(new Rect(110, xpos, 100, 20), posx+"");
			GUILayout.EndHorizontal();
		
			GUILayout.BeginHorizontal();
				// Position Y
				xpos += 30;
				GUI.Label(new Rect(10, xpos, 100, 20), "Position Y:");
				if(GUI.Button(new Rect(90, xpos, 20, 20), "-")){
					posy -= 736;
				}
				if(GUI.Button(new Rect(210, xpos, 20, 20), "+")){
					posy += 736;
				}
				GUI.TextField(new Rect(110, xpos, 100, 20), posy+"");
		
			GUILayout.EndHorizontal();
		
			GUILayout.BeginHorizontal();
				// Height
				xpos += 30;
				GUI.Label(new Rect(10, xpos, 100, 20), "Height:");
				if(GUI.Button(new Rect(90, xpos, 20, 20), "-")){
					height -= 736;
				}
				if(GUI.Button(new Rect(210, xpos, 20, 20), "+")){
					height += 736;
				}
				if(height < 736){
					height = 736;
				}
				GUI.TextField(new Rect(110, xpos, 100, 20), height+"");
		
			GUILayout.EndHorizontal();
		
			GUILayout.BeginHorizontal();
				// Width
				xpos += 30;
				GUI.Label(new Rect(10, xpos, 100, 20), "Width:");
				if(GUI.Button(new Rect(90, xpos, 20, 20), "-")){
					width -= 1280;
				}
				if(GUI.Button(new Rect(210, xpos, 20, 20), "+")){
					width += 1280;
				}
				if(width < 1280){
					width = 1280;
				}
				GUI.TextField(new Rect(110, xpos, 100, 20), width+"");
		
			GUILayout.EndHorizontal();
		
			if(gridPreview.setup(posx, posy, width, height, 2)){
				// position the camera in the middle of the new room
				architect.setPosition(posx+(width/2), posy+(height/2));
			}
		
			GUILayout.BeginHorizontal();
				// Width
				xpos += 30;
		
				doCreate = GUI.Button(new Rect(10, xpos, 100, 20), "Create Room");
		
				if(GUI.Button(new Rect(120, xpos, 100, 20), "Cancel")){
					//nextState = State.FreeMove;
					architect.setNextState(Architect.State.EndCreateRoom);
					CleanUp();
				}
				
			GUILayout.EndHorizontal();
		
		GUILayout.EndVertical();

		//float posxf=0;
		//float posyf=0;
		//float hf=0;
		//float wf=0;
				
		if(!done && doCreate){
			// first valdate
			error = "";
			//if(!float.TryParse(posx+"",out posxf)){
			//	error = "Position X invalid.";
			//}
			//if(!float.TryParse(posy,out posyf)){
			//	error = "Position Y invalid.";
			//}
			//if(!float.TryParse(height,out hf)){
			//	error = "Height invalid.";
			//}
			//if(!float.TryParse(width,out wf)){
			//	error = "Width invalid.";
			//}
			
			if(error.Length > 0){
				return;
			}
			// create RoomData object
			RoomData room = new RoomData();

			// Fill in the data
			room.Name = roomName;
			room.PositionX = posx;
			room.PositionY = posy;
			room.Height = height;
			room.Width = width;

			/*
			Debug.Log("Room: " + roomName);
			Debug.Log("PositionX: " + room.PositionX);
			Debug.Log("PositionY: " +room.PositionY);
			Debug.Log("Height: "+room.Height);
			Debug.Log("Width: "+room.Width);
			*/
			
			// pass this RoomData back to the SceneManager
			architect.getSceneManager().AddRoomToScene(room);
			
			// Change State
			architect.setNextState(Architect.State.EndCreateRoom);
			
			CleanUp();
			
			done = true;
		}
	}
	
	public override void CleanUp() {
		if(gridPreview != null){
			gridPreview.destroy();
			gridPreview = null;
		}
	}
}

