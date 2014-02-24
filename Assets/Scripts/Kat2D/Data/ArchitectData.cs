using System;
using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("Architect")]
public class ArchitectData{
	public float scaleX {get; set;}
	public float scaleY {get; set;}
	
	public string lastScene {get; set;}
	public string lastRoom {get; set;}
	
	public ArchitectData (){
		scaleX = 32;
		scaleY = 32;
		lastScene = null;
		lastRoom = null;
	}
}
