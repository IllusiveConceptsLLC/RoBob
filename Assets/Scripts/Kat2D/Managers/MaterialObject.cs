using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MaterialObject : LayerObject {
	Dictionary<string, ItemData> tiles = new Dictionary<string, ItemData>();
	
	public string spriteSheetName = "";
	public string spriteSheetPath = "";
	
	public int xsize = 0;
	public int ysize = 0;
	
    private MeshFilter m_meshFilter;
	
	public Texture2D groundTexture;
	
	private Texture2D testTexture; 
	
    public Shader shader;
	
	public float posx = 0;
	public float posy = 0;
	public float posz = 0;
	public float width = 0;
	public float height = 0;
	
	
    int chunkSize;
		
	SpriteSheet curSheetObject = null;
	
	Vector2[] meshUV;
	// Use this for initialization
	
	public float gridWidth = 0;
	public float gridHeight = 0;
	
	void Start () {
		gridWidth = Mathf.Ceil(width / Utility.TileWidth);
		gridHeight = Mathf.Ceil(height / Utility.TileHeight);
		
		gameObject.transform.position = new Vector3(posx, posy, posz);
		
		gameObject.transform.localScale = new Vector3(1,1,1);//new Vector3(Utility.TileWidth,Utility.TileHeight,0);
		
		curSheetObject = TiledSheetFactory.getSpriteSheet(spriteSheetName, spriteSheetPath);
		
		//meshUV = curSheetObject.frames[0].uv.Clone() as Vector2[];
		
		//shader = Shader.Find("Transparent/Diffuse");
		shader = Shader.Find("Unlit/Transparent");

        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        gameObject.AddComponent<MeshCollider>();
        
        m_meshFilter = this.GetComponent<MeshFilter>();

        Mesh newMesh = new Mesh();
        m_meshFilter.mesh = newMesh;

	    regenerateMesh();

        renderer.material.shader = shader;

		renderer.material.mainTexture = curSheetObject.spriteTexture;
		
		//renderer.material.color = new Color(1,1,1,1);
		
        //this.enabled = true;
	}

	
	// Update is called once per frame
	void Update () {
        
	}
		
    void regenerateMesh() {
		//offset = transform.position;
        Mesh subMesh = this.GetComponent<MeshFilter>().mesh;
        MeshCollider mc = gameObject.GetComponent<MeshCollider>();
        
        List<int> indices = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector3> verts = new List<Vector3>();
        List<Color> colors = new List<Color>();
		
		
		
        subMesh.Clear();

        int curVert = 0;
        //Vector3 curVec = new Vector3(0, 0, 0);
		ItemData tile = null;
		
		foreach(KeyValuePair<string, ItemData> item in tiles){
			tile = (ItemData)item.Value;
			
			float fx = tile.PositionX - posx; 
			float fy = tile.PositionY - posy; 
			float fz = 0; 
			
			float tw = curSheetObject.frames[tile.TargetFrame].size.x * tile.ScaleX;
			float th = curSheetObject.frames[tile.TargetFrame].size.y * tile.ScaleY;
			

            verts.Add(new Vector3(fx, fy + th, fz));
            verts.Add(new Vector3(fx + tw, fy + th, fz));
            verts.Add(new Vector3(fx + tw, fy, fz));
            verts.Add(new Vector3(fx, fy, fz));
			
			Vector2[] meshUV = curSheetObject.frames[tile.TargetFrame].uv.Clone() as Vector2[];
			
	        if (tile.InvertX){
	            Vector2 v;
	            v = meshUV[0];
	            meshUV[0] = meshUV[1]; meshUV[1] = v;
				
	            v = meshUV[2];
	            meshUV[2] = meshUV[3]; meshUV[3] = v;
	        }
	
	        if (tile.InvertY){
	            Vector2 v;
	            v = meshUV[0];
	            meshUV[0] = meshUV[3]; meshUV[3] = v;
	            v = meshUV[1];
	            meshUV[1] = meshUV[2]; meshUV[2] = v;
	        }
			
			uvs.Add(meshUV[0]);
			uvs.Add(meshUV[1]);
			uvs.Add(meshUV[2]);
			uvs.Add(meshUV[3]);
		
            indices.Add(curVert);
            indices.Add(curVert + 2);
            indices.Add(curVert + 3);

            indices.Add(curVert);
            indices.Add(curVert + 1);
            indices.Add(curVert + 2);
			
            curVert += 4;
			
			//Debug.Log(id.PositionX);
		}
		
		/*
		for (int x = 0; x < gridWidth; x++)
        {
			for (int y = 0; y < gridHeight; y++)
            {
				
                curVec.x = x;
                curVec.y = y;
                curVec.z = 0;
				
				// Need to offset the position, so it matches up with the Focus and saving. 
				float px = x * 32 + posx; 
				float py = y * 32 + posy; 
				float fx = x * 32; 
				float fy = y * 32; 
				float fz = 0; 

				if(tiles.ContainsKey(px + "_" + py)){
					tile = tiles[px + "_" + py];
					
					float tw = curSheetObject.frames[tile.TargetFrame].size.x * tile.ScaleX;
					float th = curSheetObject.frames[tile.TargetFrame].size.y * tile.ScaleY;
					
	                verts.Add(new Vector3(fx, fy + th, fz));
	                verts.Add(new Vector3(fx + tw, fy + th, fz));
	                verts.Add(new Vector3(fx + tw, fy, fz));
	                verts.Add(new Vector3(fx, fy, fz));
					
					Vector2[] meshUV = curSheetObject.frames[tile.TargetFrame].uv.Clone() as Vector2[];
					
			        if (tile.InvertX){
			            Vector2 v;
			            v = meshUV[0];
			            meshUV[0] = meshUV[1]; meshUV[1] = v;
						
			            v = meshUV[2];
			            meshUV[2] = meshUV[3]; meshUV[3] = v;
			        }
			
			        if (tile.InvertY){
			            Vector2 v;
			            v = meshUV[0];
			            meshUV[0] = meshUV[3]; meshUV[3] = v;
			            v = meshUV[1];
			            meshUV[1] = meshUV[2]; meshUV[2] = v;
			        }
					
					uvs.Add(meshUV[0]);
					uvs.Add(meshUV[1]);
					uvs.Add(meshUV[2]);
					uvs.Add(meshUV[3]);
				
		            indices.Add(curVert);
	                indices.Add(curVert + 2);
		            indices.Add(curVert + 3);
		
		            indices.Add(curVert);
		            indices.Add(curVert + 1);
		            indices.Add(curVert + 2);
					
	                curVert += 4;
				}
            }
        }
		*/
        subMesh.vertices = verts.ToArray();
        subMesh.triangles = indices.ToArray();
        subMesh.uv = uvs.ToArray();
        subMesh.colors = colors.ToArray();

        subMesh.Optimize();
        subMesh.RecalculateNormals(); 
        
		mc.sharedMesh = new Mesh();
		mc.sharedMesh = subMesh;
    }
	
	public override void addItem(ItemData item){
		if(tiles.ContainsKey(item.PositionX + "_" + item.PositionY)){
		}else{
			tiles.Add(item.PositionX + "_" + item.PositionY, item);
		}
	}
	
	public override void destroy(){
		GameObject.DestroyImmediate(this.gameObject);
	}
}