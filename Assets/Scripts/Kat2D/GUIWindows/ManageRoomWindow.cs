using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class ManageRoomWindow : IBuilderWindow{
		
	public ManageRoomWindow(Architect g){
		architect = g;
		title = "Scene Manager";
	}
	
	string snapX = "32";
	
	float sx = 0;
	
	string snapY = "32";
	
	float sy = 0;
	
	public override void Update() {
		
			architect.getData().scaleX = sx;
		
		
			architect.getData().scaleY = sy;
		
	}
	
	public override void Create(int id) {
		GUILayout.BeginVertical();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Snap X:");
		snapX = GUILayout.TextField(snapX);
		if(float.TryParse(snapX, out sx)){
			if(sx<1){
				sx = 1;
			}else{
				if(sx > 2048){
					sx = 2048;
				}
			}
		}else{
			sx = 32;
		}
		snapX=sx.ToString();
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Snap Y:");
		snapY = GUILayout.TextField(snapY);
		if(float.TryParse(snapY, out sy)){
			if(sy<1){
				sy = 1;
			}else{
				if(sy > 2048){
					sy = 2048;
				}
			}
		}else{
			sy = 32;
		}
		snapY = sy.ToString();
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		
		// Room selection,,,
		GUILayout.BeginVertical();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Choose Room");
		GUILayout.EndHorizontal();
		List<RoomData> rooms = architect.getSceneManager().getRooms();
		RoomData curRoom = architect.getSceneManager().getCurrentRoom();
		string current = "";
		if(rooms != null){
			
			foreach(RoomData rd in rooms){
				GUILayout.BeginHorizontal();
				current = "";
				if(curRoom != null && curRoom.Equals(rd)){
					// This is the one....
					current = " * ";
				}
				bool hit = GUILayout.Button(current + "Room" + rd.PositionX + "_" + rd.PositionY);
				if(hit){
					architect.loadRoom(rd);
				}
				GUILayout.EndHorizontal();
			}
		} 
		GUILayout.EndVertical();
		
		GUILayout.BeginVertical();
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("Create a Room")){
			architect.setNextState(Architect.State.StartCreateRoom);
		}
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		
		// This current room information...
		if(curRoom!=null){
			GUILayout.BeginVertical();
			GUILayout.Label("Room Name: " + curRoom.Name);
			GUILayout.Label("Room Position: x:" + curRoom.PositionX + " y:"+curRoom.PositionY);
			GUILayout.Label("Room Size: w:" + curRoom.Width+ " h:"+curRoom.Height);
			GUILayout.EndVertical();
		}
		

		
	
	}
	
	public override void CleanUp() {
		
	}
}


