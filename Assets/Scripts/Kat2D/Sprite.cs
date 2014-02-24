using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Sprite : LayerObject {
	
	public enum ANCHOR_POINT {
		TOP_LEFT=0,
		TOP_CENTER=1,
		TOP_RIGHT=2,
		MIDDLE_LEFT=3,
		MIDDLE_CENTER=4,
		MIDDLE_RIGHT=5,
		BOTTOM_LEFT=6,
		BOTTOM_CENTER=7,
		BOTTOM_RIGHT=8
	}
	
	public ANCHOR_POINT AnchorPoint = ANCHOR_POINT.BOTTOM_LEFT;
	public int target_frame = -1;
	public bool invert_x = false;
	public bool invert_y = false;
	private bool isDirty = false;
	private SpriteSheet curSheetObject = null;
	public bool auto_resize = true;
	public float scale_x = 1;
	public float scale_y = 1;
	public float scale_z = 1;
	public float rotation = 0;

	public void setIsDirty(bool dirt) {
		this.isDirty = dirt;
	}

	public void setRotation(float rot){
		if(this.rotation != rot){
			this.rotation = rot;
			isDirty = true;
		}
	}
	public void setScaleZ(float sz){
		if(this.scale_z != sz){
			this.scale_z = sz;
			isDirty = true;
		}
	}
	
	public void setScaleX(float sx){
		if(this.scale_x != sx){
			this.scale_x = sx;
			isDirty = true;
		}
	}
	public void setScaleY(float sy){
		if(this.scale_y != sy){
			this.scale_y = sy;
			isDirty = true;
		}
	}
	
	public void setInvertX(bool invx){
		if(invert_x != invx){
			invert_x = invx;
			isDirty = true;
		}
	}
	
	public void setInvertY(bool invy){
		if(invert_y != invy){
			invert_y = invy;
			isDirty = true;
		}
	}
	
	public void setTargetFrame(int tf){
		if(target_frame != tf){
			target_frame = tf;
			isDirty = true;
		}
	}
	
	
	
	public void faceRight(){
		setInvertX(false);
	}
	public void faceLeft(){
		setInvertX(true);
	}
	
	public string spriteSheetName = "";
	public string spriteSheetPath = "";

	public TileSheetDefinition tileSheetDefinition;

	private SpriteSheet spriteSheet = null;
	
	Mesh mesh = null;
	
	public string getSpriteSheetName() {
		return spriteSheetName;
	}
	public string getSpriteSheetPath() {
		return spriteSheetPath;
	}
	
	public void setSpriteSheetName(string sheetname){
		if(!this.spriteSheetName.Equals(sheetname)){
			this.spriteSheetName = sheetname;
			isDirty = true;
		}
	}
	
	public void setSpriteSheetPath(string path){
		if(!this.spriteSheetPath.Equals(path)){
			spriteSheetPath = path;
			isDirty = true;
		}
	}
	
	//public void setSpriteSheetName(string sheetname, string sheetpath){
	//	if(spriteSheetName == null || !spriteSheetName.Equals(sheetname)){
	//		spriteSheetName = sheetname;
	//		spriteSheetPath = sheetpath;
	//		isDirty = true;
	//	} 
	//}/
	
	public void destroySpriteSheet() {
		if (tileSheetDefinition == null) {
			if (spriteSheetName == null || spriteSheetName.Trim ().Length == 0) {
				return;
			}
		}
		// Destroy current.
		if(spriteSheet != null){
			spriteSheet.destroy();
			this.spriteSheet = null;
		}

		if (tileSheetDefinition != null) {
			spriteSheet = TiledSheetFactory.getSpriteSheet(tileSheetDefinition);
		} else {
			spriteSheet = TiledSheetFactory.getSpriteSheet(spriteSheetName, spriteSheetPath);
		}

	}
	
	public virtual void Start () {
		destroySpriteSheet();
		
		// No sprite sheet loaded, this is expected..
		if(this.spriteSheet == null){
			// 
			return;
		}
		CreateMesh();
		FindSpriteSheet();
		SetSprite();
		SetRotation();
	}
	
	public void Clear(){
		return;
		//curSheetObject = null;
		//spriteSheetName = "";
		//CreateMesh();
	}
	
	public virtual void Update(){
		if(isDirty){
			//Debug.Log("Update Frame");
			destroySpriteSheet();
			CreateMesh();
			FindSpriteSheet();
			SetSprite();
			SetRotation();
			isDirty = false;
		}
	}
	
	private void SetRotation(){
		transform.localEulerAngles = new Vector3(0,0,rotation);
	}
	
	public Vector3 getPosition(){
		return transform.position;
	}
	
	public void SetSprite(){
		if(curSheetObject == null){
			return;
		}
		renderer.sharedMaterial = curSheetObject.sharedMaterial;
				
		renderer.sharedMaterial.mainTexture = curSheetObject.spriteTexture;

		if(target_frame >= curSheetObject.frames.Length){
			target_frame = 0;
		}else if(target_frame < 0){
			target_frame = curSheetObject.frames.Length-1;
		}
		
		if(auto_resize){
			Vector2 size = curSheetObject.frames[target_frame].size;
			gameObject.transform.localScale = new  Vector3(size.x*scale_x, size.y*scale_y, scale_z);
		}
		
        Vector2[] meshUV = curSheetObject.frames[target_frame].uv.Clone() as Vector2[];
        if (invert_x)
        {
            Vector2 v;
            v = meshUV[0];
            meshUV[0] = meshUV[1]; meshUV[1] = v;
			
            v = meshUV[2];
            meshUV[2] = meshUV[3]; meshUV[3] = v;
        }

        if (invert_y)
        {
            Vector2 v;
            v = meshUV[0];
            meshUV[0] = meshUV[3]; meshUV[3] = v;
            v = meshUV[1];
            meshUV[1] = meshUV[2]; meshUV[2] = v;
        }
		
		mesh.uv = meshUV;
		
		mesh.RecalculateNormals();
		
		//if(customColor){
		//	Color[] cs = new Color[4];
		//	cs[0]=cs[1]=cs[2]=cs[3]=textureColor;
		//	mesh.colors = cs;
		//}
	}

	private void FindSpriteSheet(){
		curSheetObject = this.spriteSheet;
	}
	
	private void CreateMesh(){
		GetComponent<MeshFilter>().sharedMesh = mesh = new Mesh();
		mesh.Clear();
		mesh.name = "Vash";
		mesh.hideFlags = HideFlags.HideAndDontSave;

		// TODO: FINISH THE ANCHOR POINT MAPPING,,,
		// Generate verticies. 
		switch(AnchorPoint){
		case ANCHOR_POINT.TOP_LEFT:
		case ANCHOR_POINT.TOP_CENTER:
		case ANCHOR_POINT.TOP_RIGHT:
		case ANCHOR_POINT.MIDDLE_LEFT:
			mesh.vertices = new Vector3[] { 
	            new Vector3(0,.5f,0),
	            new Vector3(1,.5f,0),
	            new Vector3(1,-.5f,0),
	            new Vector3(0,-.5f,0)
	        };
			break;
		case ANCHOR_POINT.MIDDLE_CENTER:
			mesh.vertices = new Vector3[] { 
	            new Vector3(-.5f,.5f,0),
	            new Vector3(.5f,.5f,0),
	            new Vector3(.5f,-.5f,0),
	            new Vector3(-.5f,-.5f,0)
	        };
			break;
		case ANCHOR_POINT.BOTTOM_CENTER:
		case ANCHOR_POINT.BOTTOM_RIGHT:
		case ANCHOR_POINT.MIDDLE_RIGHT:
		case ANCHOR_POINT.BOTTOM_LEFT:
			mesh.vertices = new Vector3[] { 
	            new Vector3(0,1,0),
	            new Vector3(1,1,0),
	            new Vector3(1,0,0),
	            new Vector3(0,0,0)
	        };
			break;
		}
		
		//if(center){
		//mesh.vertices = new Vector3[] { 
        //    new Vector3(-.5f,.5f,0),
        //    new Vector3(.5f,.5f,0),
        //    new Vector3(.5f,-.5f,0),
        //    new Vector3(-.5f,-.5f,0)
        //};
		//}
		
		// if(bottom_left){
		//mesh.vertices = new Vector3[] { 
        //    new Vector3(0,1,0),
        //    new Vector3(1,1,0),
        //    new Vector3(1,0,0),
        //    new Vector3(0,0,0)
        //};
		//{
		
		
		// Set triangles
		//0,2,3,0,1,2
		mesh.triangles = new int[] {
			0,2,3,0,1,2
		};
	}
	
	public override void addItem(ItemData item){
	}
	
	public override void destroy(){
		GameObject.DestroyImmediate(this.gameObject);
	}

}