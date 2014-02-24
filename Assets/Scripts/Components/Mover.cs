using UnityEngine;
using System.Collections;


public class Mover : MonoBehaviour {
	
	public bool COLLISION = false;
	
	//public bool IS_GROUNDED = true;
	public bool COLLISION_VERTICAL = false;
	public bool COLLISION_BOTTOM = false;
	public bool COLLISION_TOP = false;
	
	// HORIZ collision
	public bool COLLISION_HORIZONTAL = false;
	public bool COLLISION_LEFT = false;
	public bool COLLISION_RIGHT = false;
	
	// TYPE collisions
	public bool COLLISION_STAIRS = false;
	
	// movement flags
	public bool MOVED = false;
	public bool MOVED_UP = false;
	public bool MOVED_RIGHT = false;
	public bool MOVED_DOWN = false;
	public bool MOVED_LEFT = false;
	
	
	// check for stairs??
	public bool checkStairs = true;
	
	// Private stuff, clean up?

	private Transform trans;
	private float halfHeight = 0;
	private float halfWidth = 0;
	private int bcl = 1 << Constants.COLLIDER_BOX_LAYER;
	private int scl = 1 << Constants.COLLIDER_STAIR_LAYER;
	private float moveXAmount = 4;
	private float moveYAmount = 4;
	private float moveY = 0;
	private float moveYForced = 0;
	private float moveX = 0;
	private Vector3 moveXDirection = Vector3.zero;
	private Vector3 moveYDirection = Vector3.zero;
	private float currentYPosition = 0;
	private int xdir=0; 
	private int ydir=0;
	
	
	public bool checkCollisions = true;
	public bool showRays = false;
	public float horizontalCenterOffset = 0;
	public Vector3[] horizontalOffsets = new Vector3[0];
	public Vector3[] verticalOffsetsDown = new Vector3[0];
	public Vector3[] verticalOffsetsUp = new Vector3[0];
	public Vector3 stairOffset = Vector3.zero;
	public float climbHeight = 8;	
	
	// Use this for initialization
	void Start () {
		trans = gameObject.transform;
		halfWidth=gameObject.collider.bounds.size.x/2;
		halfHeight=gameObject.collider.bounds.size.y/2;
		
		//horizontalOffsets = new Vector3[3];
		//horizontalOffsets[0] = new Vector3(12,64,0);
		//horizontalOffsets[1] = new Vector3(12,0,0);
		//horizontalOffsets[2] = new Vector3(12,-64,0);
		
		//horizontalOffsets[0] = new Vector3(12,-63,0);
		//horizontalOffsets[1] = new Vector3(12,0,0);
		//horizontalOffsets[2] = new Vector3(12,63,0);
		/*
		horizontalOffsets = new Vector3[5];
		horizontalOffsets[0] = new Vector3(12,-64,0);
		horizontalOffsets[1] = new Vector3(12,-32,0);
		horizontalOffsets[2] = new Vector3(12,0,0);
		horizontalOffsets[3] = new Vector3(12,32,0);
		horizontalOffsets[4] = new Vector3(12,60,0);
		
		verticalOffsetsDown = new Vector3[2];
		verticalOffsetsDown[0] = new Vector3(-12,64,0);
		verticalOffsetsDown[1] = new Vector3(12,64,0);
		
		verticalOffsetsUp = new Vector3[2];
		verticalOffsetsUp[0] = new Vector3(-12,60,0);
		verticalOffsetsUp[1] = new Vector3(12,60,0);
		*/
	}
	
	
	
	private void reset_collisions(){
		COLLISION = COLLISION_VERTICAL = COLLISION_BOTTOM = 
			COLLISION_TOP = COLLISION_HORIZONTAL = 
				COLLISION_LEFT = COLLISION_RIGHT = false;
		//= COLLISION_STAIRS
	}
	
	private void reset_movements(){
		MOVED = MOVED_LEFT = MOVED_RIGHT = MOVED_UP = MOVED_DOWN = false;
	}

	private void setCollisionStairs(){
		COLLISION = COLLISION_VERTICAL = COLLISION_BOTTOM = COLLISION_STAIRS = true;
	}
	private void setCollisionBottom(){
		COLLISION = COLLISION_VERTICAL = COLLISION_BOTTOM = true;
	}
	private void setCollisionTop(){
		COLLISION = COLLISION_VERTICAL = COLLISION_TOP = true;
	}
	private void setCollisionRight(){
		COLLISION = COLLISION_HORIZONTAL = COLLISION_RIGHT = true;
	}
	private void setCollisionLeft(){
		COLLISION = COLLISION_HORIZONTAL = COLLISION_LEFT = true;
	}
	
	RaycastHit hit;

	private float getDistanceToCollider(float x, float y, float z, float amount, Vector3 direction, int ctype){
		Color c = Color.white;
		if(direction.Equals(Vector3.left)){
			c = Color.red;
		}else if(direction.Equals(Vector3.right)){
			c = Color.blue;
		}else if(direction.Equals(Vector3.up)){
			c = Color.yellow;
		}
		return getDistanceToCollider(x, y, z, amount, direction, ctype, c);
	}

	private float getDistanceToCollider(float x, float y, float z, float amount, Vector3 direction, int ctype, Color c){
		
		Vector3 leftOne = new Vector3(x, y, z);
		if(showRays){

			Debug.DrawRay (leftOne, direction*amount, c, 1);
		}
		if (Physics.Raycast (leftOne, direction, out hit, amount+1, ctype)) {
			if(!hit.collider.isTrigger){
								
				float md = Mathf.Ceil(hit.distance);
				// if move distance = 0, just return 0;
				if(md == 0){
					return 0;
				}
				// if move distance greater than move amount, return amount -1
				// this ensures we are always one pixel away.
				if(md > amount){
					return amount - 1 ;
				}
				// if move distance less or equal to amount, return move distance -1
				// this ensures we are always one pixel away.
				if(md <= amount){
					return (md - 1);
				}
			}
		}
		return amount;
	}

	public void setCheckCollisions(bool check){
		this.checkCollisions = check;
	}
	
	public void Move(Vector3 moveDirs){
		reset_collisions();
		reset_movements();
		
		if(moveDirs.x > 0){
			xdir = 1;
			moveXDirection = Vector3.right;
		}else if(moveDirs.x < 0){
			xdir = -1;
			moveXDirection = Vector3.left;
		}else{
			xdir = 0;
		}

		if(moveDirs.y > 0){
			ydir = 1;
			moveYDirection = Vector3.up;
		}else if(moveDirs.y < 0){
			ydir = -1;
			moveYDirection = Vector3.down;
		}else{
			ydir = 0;
		}
		
		if(trans==null){return;}
		moveY = 0;
		moveYForced = 0;
		moveX = 0;
		
		currentYPosition = trans.position.y;
		moveXAmount = Mathf.Abs(moveDirs.x);

		// If player is trying to move.
		if(horizontalOffsets!=null && !COLLISION_STAIRS){
			if(moveXAmount > 0){
				foreach(Vector3 hv in horizontalOffsets){
					//Debug.Log(hv.x*xdir);
					moveX = getDistanceToCollider(transform.position.x+horizontalCenterOffset+(hv.x*xdir), currentYPosition+hv.y, transform.position.z, moveXAmount, moveXDirection, bcl);
					if(moveX<moveXAmount){
						if(hit.collider.CompareTag(Constants.TAG_COLLIDER_STAIR)){
							continue;
						}
						moveXAmount = 0;
						break;
					}
				}
				moveX = moveX * xdir;
			}
		}
		
		// Horizontal movement...
		if(moveXAmount > 0){
			if(stairOffset != Vector3.zero){
				//Debug.Log("Doint it");
				moveX = getDistanceToCollider(transform.position.x+stairOffset.x, currentYPosition-stairOffset.y, transform.position.z, moveXAmount, moveXDirection, bcl, Color.green);
				if(moveX==0){
					float tmoveX = getDistanceToCollider(transform.position.x+stairOffset.x, (currentYPosition-stairOffset.y)+climbHeight, transform.position.z, moveXAmount, moveXDirection, bcl, Color.green);
					if(tmoveX != 0){
						moveX = tmoveX;
						moveYForced = climbHeight;
					}
				}
				moveX = moveX * xdir;
			}
		}
		
		
		if(moveYForced!=0){
			moveYAmount = moveYForced;
		}else{
			moveYAmount = Mathf.Abs(moveDirs.y);
		}
		
		// Check for special vertical collision.
		COLLISION_STAIRS = false;
		if(stairOffset != Vector3.zero && moveYDirection!=Vector3.up){
			moveY = getDistanceToCollider(transform.position.x+stairOffset.x+moveX, (currentYPosition-stairOffset.y)+moveYForced, transform.position.z, moveYAmount, moveYDirection, bcl);	
			if(Mathf.Abs(moveY) < moveYAmount){
				if(hit.collider.CompareTag(Constants.TAG_COLLIDER_STAIR)){
					COLLISION_STAIRS = true;
				}
				moveYAmount = 0;
				setCollisionBottom();
			}
			moveY = moveY * ydir;
		}
		float prevmoveY = moveY;
		if(moveYAmount > 0){
	
			if(verticalOffsetsUp != null && ydir > 0){
				foreach(Vector3 vv in verticalOffsetsUp){
					moveY = getDistanceToCollider(transform.position.x+moveX+vv.x, (currentYPosition+(vv.y*ydir))+moveYForced, transform.position.z, moveYAmount, moveYDirection, bcl);	
					if(hit.collider != null && hit.collider.CompareTag(Constants.TAG_COLLIDER_STAIR)){
						moveY = prevmoveY;
						continue;
					}else{
						moveY = moveY * ydir;
						if(Mathf.Abs(moveY) < moveYAmount){
							setCollisionTop();
							break;
						}
					}
				}
			}else if(verticalOffsetsDown != null && ydir < 0){
				foreach(Vector3 vv in verticalOffsetsDown){
					moveY = getDistanceToCollider(transform.position.x+moveX+vv.x, (currentYPosition+(vv.y*ydir))+moveYForced, transform.position.z, moveYAmount, moveYDirection, bcl);	
					if(hit.collider != null && hit.collider.CompareTag(Constants.TAG_COLLIDER_STAIR)){
						moveY = prevmoveY;
						continue;
					}else{
						moveY = moveY * ydir;
						if(Mathf.Abs(moveY) < moveYAmount){
							setCollisionBottom();
							break;
						}
					}
				}
			}
			
		}
		
		// set movement flags
		
		if(moveX < 0){
			//Debug.Log("moving left");
			MOVED_LEFT = MOVED = true;
		}else if(moveX >0){
			MOVED_RIGHT = MOVED = true;
		}
		
		if(moveY < 0){
			MOVED_DOWN = MOVED = true;
		}else if(moveY > 0){
			//Debug.Log("MOVED UP!");
			MOVED_UP = MOVED = true;
		}
		
		trans.position = new Vector3(trans.position.x+moveX, (currentYPosition+moveYForced)+moveY, trans.position.z);
		//Debug.Log(trans.position);
	}
}









































/*
	public void moveFixed(int xdir, int ydir, MoverSpeed speed){
		if(trans==null){return;}
		moveY = 0;
		moveYForced = 0;
		moveX = 0;
		
		int moveXAmount = 0;
		
		currentYPosition = Mathf.Ceil(trans.position.y);
		moveX = moveXAmount;
		if(xdir > 0){
			moveDirection = Vector3.right;
		}else if(xdir < 0){
			moveDirection = Vector3.left;
		}else{
			moveX = 0;
		}
		if(moveX > 0){
			moveX = getDistanceToCollider(transform.position.x, currentYPosition-halfHeight, transform.position.z, moveXAmount, moveDirection) * xdir;
			if(moveX==0){
				float tmoveX = getDistanceToCollider(transform.position.x, (currentYPosition-halfHeight)+climbDistance, transform.position.z, moveXAmount, moveDirection) * xdir;
				if(tmoveX != 0){
					moveX = tmoveX;
					moveYForced = climbDistance;
				}
			}
		}
		
		if(verticalForce>0){
			moveY = -(Time.deltaTime * verticalForce);
			verticalForce -= 10;
		}else{
			// Gravity , apply if needed
			if(applyGravity){
				moveY = getDistanceToCollider(transform.position.x+moveX, (currentYPosition-halfHeight)+moveYForced, transform.position.z, gravity, Vector3.down);
			}
			if(moveY==0){
				IS_GROUNDED = true;
			}else{
				IS_GROUNDED = false;
			}
		}
		
		if(transform.position.y < 0){
			moveY = 0;
		}
		
		trans.position = new Vector3(trans.position.x+moveX, (currentYPosition+moveYForced)-moveY, trans.position.z);
	}
*/