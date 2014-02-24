using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridMaker : MonoBehaviour {
	
    private MeshFilter m_meshFilter;
	
	public Texture2D groundTexture;
	
	private Texture2D testTexture; 
	
    public Shader shader;

    int chunkSize;
	
	private bool isDirty = true;
	
	public float posx = 0;
	public float posy = 0;
	public float width = 0;
	public float height = 0;
	
	public int frame = 0;
	
	SpriteSheet curSheetObject = null;
	
	Vector2[] meshUV;
	// Use this for initialization
	
	public void setFrame(int frame){
		if(this.frame != frame){
			this.frame = frame;
			this.isDirty = true;
		}
	}
	
	public bool setup(float posx, float posy, float width, float height, int frame){
		isDirty = false;
		
		if(this.posx != posx){
			this.posx = posx;
			isDirty = true;
		}
		if(this.posy != posy){
			this.posy = posy;
			isDirty = true;
		}
		if(this.width != width){
			this.width = width;
			isDirty = true;
		}
		if(this.height != height){
			this.height = height;
			isDirty = true;
		}
		setFrame(frame);

		return isDirty;
	}
	
	public float gridWidth = 0;
	public float gridHeight = 0;
	
	void Start () {
		
		
		gameObject.transform.localScale = new Vector3(1,1,0);//new Vector3(Utility.TileWidth,Utility.TileHeight,0);
		
		curSheetObject = TiledSheetFactory.getSpriteSheet("Grid", "EditorSheets");
		
		
		shader = Shader.Find("Transparent/Diffuse");
		
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        gameObject.AddComponent<MeshCollider>();
        
        m_meshFilter = this.GetComponent<MeshFilter>();

        Mesh newMesh = new Mesh();
        m_meshFilter.mesh = newMesh;

	    regenerateMesh();

        renderer.material.shader = shader;

		renderer.material.mainTexture = curSheetObject.spriteTexture;
		
		
		
        this.enabled = true;
	}

	
	// Update is called once per frame
	void Update () {
		if(isDirty){
        	gameObject.transform.position = new Vector3(posx, posy, 800);
			regenerateMesh();
			isDirty = false;
		}
	}
		
    void regenerateMesh() {
		gridWidth = Mathf.Ceil(width / Utility.TileWidth);
		gridHeight = Mathf.Ceil(height / Utility.TileHeight);
		
		meshUV = curSheetObject.frames[frame].uv.Clone() as Vector2[];
		
		//offset = transform.position;
        Mesh subMesh = this.GetComponent<MeshFilter>().mesh;
        MeshCollider mc = gameObject.GetComponent<MeshCollider>();
        
        List<int> indices = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector3> verts = new List<Vector3>();
        List<Color> colors = new List<Color>();

        subMesh.Clear();

        int curVert = 0;
        Vector3 curVec = new Vector3(0, 0, 0);
				
		for (int x = 0; x < gridWidth; x++)
        {
			for (int y = 0; y < gridHeight; y++)
            {
				
                curVec.x = x;
                curVec.y = y;
                curVec.z = 0;
				
				// Need to offset the position, so it matches up with the Focus and saving. 
				float fx = x * 32 ; 
				float fy = y * 32 ; 
				float fz = 0; 
				
                verts.Add(new Vector3(fx, fy + 32, fz));
                verts.Add(new Vector3(fx + 32, fy + 32, fz));
                verts.Add(new Vector3(fx + 32, fy, fz));
                verts.Add(new Vector3(fx, fy, fz));

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
		
        subMesh.vertices = verts.ToArray();
        subMesh.triangles = indices.ToArray();
        subMesh.uv = uvs.ToArray();
        subMesh.colors = colors.ToArray();

        subMesh.Optimize();
        subMesh.RecalculateNormals(); 
        
		mc.sharedMesh = new Mesh();
		mc.sharedMesh = subMesh;
    }
	
	public void destroy(){
		GameObject.DestroyImmediate(this.gameObject);
	}
}