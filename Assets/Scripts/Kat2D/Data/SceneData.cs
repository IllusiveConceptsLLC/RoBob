using System;
using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("Scene")]
public class SceneData
{
	public string Name {get; set;}
	
	public string DefaultRoom {get; set;}
	
	//[XmlArray("Rooms")]
 	//[XmlArrayItem("Room")]
	private List<RoomData> Rooms {get; set;}
	public void setRooms(List<RoomData> rooms){
		this.Rooms = rooms;
	}
	public List<RoomData> getRooms(){
		return this.Rooms;
	}
	public void addRoom(RoomData rd){
		this.Rooms.Add(rd);
	}
	public SceneData() {
		Rooms = new List<RoomData>();
		Name = "Nothing";
	}
}