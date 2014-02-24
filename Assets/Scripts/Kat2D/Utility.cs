using System;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class Utility {
	
	public static float TileWidth = 32;
	public static float TileHeight = 32;
	
	public static float ChunkSize = 16;
	public static int ChunkSizeI = 16;
	
	
	public static float calcChunk(float pos){
		return Mathf.Floor(pos / ChunkSize);
	}
	
	public static string genBlockName(Vector3 pos) {
		return genBlockName(pos.x, pos.y, pos.z);
	}
	public static string genBlockName(float x, float y, float z) {
		return "x:"+x+"y:"+y+"z:"+z;
	}
	
	public static object loadXMLToObject(string fullpath, Type type){
		object o = null;
		if(File.Exists(fullpath)){
			var serializer = new XmlSerializer(type);
			var stream = new FileStream(fullpath, FileMode.Open);
			o = serializer.Deserialize(stream);
			stream.Close();
		}else{
			//Debug.Log(fullpath + " does not exist.");
		}
		return o;
	}
	
	public static void saveObjectToXML(string path, string name, System.Object obj){
		//Debug.Log ("Saving");
		var serializer = new XmlSerializer(obj.GetType());
		
		// Scene data is one thing, but we need to do the rooms seperately...
		Directory.CreateDirectory(path);
		
		//Debug.Log ("Using path: " + path + name);
		var stream = new FileStream(path + name, FileMode.Create);
		serializer.Serialize(stream, obj);
		stream.Close();
	}
	
	/*
	// Need to find package for StringBuilder...
	public static string readTextFile(string filename) {
        StringBuilder builder = new StringBuilder();
    	StreamReader reader=new  StreamReader(filename);
        try{    
            do {
				builder.Append(reader.ReadLine());
            }   
            while(reader.Peek() != -1);
        }catch{ 
            // Nothing really...
		}finally{
            reader.Close();
        }
		
        return builder.ToString();
	}
	*/
}