using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider), typeof(Rigidbody), typeof(AnimatedSprite))]
[RequireComponent(typeof(Mover))]
public class PlayerController : MonoBehaviour {
	/* 
	 * Movement definitions 
	 */
	public float walkingSpeed = 256;
	public float runningSpeed = 20;
	
	public float maxFallingSpeed = -5000;
	public float gravity = 32;
	public float jumpMultiplyer = .1f;
	
	public float jumpingPower = 512;
	
	public float jumpingPowerSuper = 50;

	public float colliderXOffset = 0;
	public float colliderYOffset = 0;
	
	
	
	//private CharacterController controller = null;

	/* 
	 * Standing Animation 
	 */
	static float standingAnimationTimout = 2;
	float standingAnimationClock = standingAnimationTimout;
	
	/*
	 * The next movement of the player
	 */
	private Vector3 moveDirection = Vector3.zero;
	
	/* 
	 * Jumping Related Variables 
	 */
	public bool jumpCanceled = false;
	bool canDoubleJump = false;
	bool didDoubleJump = false;

	
	public float currentRoatation=0;
	
	/* Fire Related Variables */
	public bool attack1Up = true;
	public bool holdingAttack1 = false;
	public bool attack2Up = true;
	public bool holdingAttack2 = false;
	
	
	public float swordAttackKneel = -50f;
	
	static float fireRate = 0.25f;
	float fireTimeout = fireRate;
	public float fireHeight = 4.5f;//1.2f;
	public float fireDistance = 2;
	public float fireHeightKneel = 2.625f;//.7f;
	
	private float colliderHeight = 0.85f;
	private float colliderRadius = 0.2f;
	private float colliderCenterY = 0.45f;
	
	public float colliderKneelHeight = 0.47f;
	public float colliderKneelRadius = 0.2f;
	public float colliderKneelCenterY = 0.24f;
	
	/* 
	 * Input Flags
	 */
	// This is the player holding the jump button right now.
	public bool holdingJump = false;
	// Is the player holding left / right buttons
	public float inputXAxis = 0;
	// Is the player holding the up / down buttons
	public float inputYAxis = 0;
	
	/*
	 * These are animation and action flags.
	 */
	// Is the player hitting their head.
	private bool isHittingHead = false;
	// Is the player touching the side of something.
	private bool isTouchingSide = false;
	// Is the player currently standing on the ground.
	private bool isGrounded = false;
	// If true the player is in a kneeling position.
	private bool isKneeling = false;
	// If true the player has pressed the jump button.
	private bool isJumping = false;
	// If true the player has double jumped.
	private bool isDoubleJumping = false;
	// If true the player has jumped off of a wall
	private bool isWallJumping = false;
	// If true the player is currently falling.
	private bool isFalling = false;
	// If true the player is being hurt.
	private bool isHurt = false;
	// If the player is standing on stairs.
	private bool isOnStairs = false;
	// If the player is currently attacking.
	private bool isAttacking = false;

	/*
	 * The player collision flags
	 */
	// The current collision flags from character controller
	//private CollisionFlags flags;
	
	// Components
	
	private Mover mover;
	
	private AnimatedSprite animator;
	
	/*
	 * Action Timers 
	 */
	// This will be the count down clock for being hurt
	private float isHurtClock = 0;
	private float isHurtDuration = 1;
	
	//private float isJumpingClock = 0;
	//private float isJumpingDuration = .25f;
	//private static bool created = false;

	
	// Use this for initialization
	public void Start () {
		rigidbody.isKinematic = true;
		
		mover = (Mover)GetComponent(typeof(Mover));
		
		//mover.setCheckCollisions(true);
		animator = (AnimatedSprite)GetComponent(typeof(AnimatedSprite));
		
		BoxCollider bc = (BoxCollider)GetComponent(typeof(BoxCollider));
		bc.center = new Vector3(colliderXOffset,colliderYOffset,0);
	}

	// We will use this function to handle all player input
	public void HandleInput(){
		// The horizontal axis amount
		//inputXAxis = Input.GetAxisRaw("Horizontal");
		inputXAxis = 1;

		// The vertical axis amount
		inputYAxis = Input.GetAxisRaw("Vertical");
		
		// Is the jump button pressed down?
		holdingJump = Input.GetButton("Jump");
		
		
		// Is the player holding down attack 1
		//holdingAttack1 = Input.GetButton("Fire1");
		
		// Is the player holding down attack 2
		//holdingAttack2 = Input.GetButton("Fire2");
	}
	public void Update(){
		if (transform.position.x >= 1600) {
			transform.position = new Vector3(330, transform.position.y, transform.position.z);
		}
		// Check inputs
		HandleInput();		
	}
	public void FixedUpdate() {
		if(mover==null){
			
			return;
		}
		
		// Here we need to decide what actions should not allow and Update to occur.
		// Death
		
		// Get the players current position.
		transform.position = new Vector3(transform.position.x,transform.position.y,300);
		
		
		// TODO: Fille out this section with comments.
		// If the player is touching the ground.
		//isJumping = false;
		if(isHurt){
			if(isHurtClock==isHurtDuration){
				moveDirection = new Vector3(0, jumpingPower/2, 0);
				//showDamage("10");
			}else{
				if(isGrounded || isHurtClock<=0){
					isHurt = false;
					return;
				}
			}
			isHurtClock -= Time.deltaTime;
			if(currentRoatation==0){
				// Facing right
				moveDirection.x = -1;
			}else{
				moveDirection.x = 1;
			}
			moveDirection.x*=walkingSpeed;
		}else if(isWallJumping){
			if(currentRoatation == 0){
				moveDirection = new Vector3(1, moveDirection.y, 0);	
			}else{
				moveDirection = new Vector3(-1, moveDirection.y, 0);	
			}
			moveDirection.x*=walkingSpeed;
			if(isFalling){
				isWallJumping = false;
			}
		}else if (isFalling && isTouchingSide){
			moveDirection = new Vector3(inputXAxis, -10, 0);
			if(inputXAxis>0){
				currentRoatation = 180;
			}else{
				currentRoatation = 0;
			}
			canDoubleJump = false;
			didDoubleJump = true;
			if(holdingJump){
				moveDirection = new Vector3(-inputXAxis, jumpingPower/1.2f, 0);
				isWallJumping = true;
			}
		}else {
			if(isGrounded){
				jumpCanceled = false;
				canDoubleJump = false;
				didDoubleJump = false;
				isJumping = false;
				
				// Determine if the player is kneelin33g.
				if(inputYAxis!=0){
					isKneeling = true;
					// Prevent the player from moving when they are kneeling.
					inputXAxis = 0;
				}else{
					isKneeling = false;
				}
				
				moveDirection = new Vector3(inputXAxis, -walkingSpeed, 0);
				moveDirection.x*=walkingSpeed;
				
				if(holdingJump){
					isJumping = true;
					moveDirection.y = jumpingPower;
				}
			}else{
				// If the user activated this jump
				if(isJumping){
					moveDirection = new Vector3(inputXAxis, moveDirection.y, 0);
					// This if for if the user has simply let go of the Jump button
					// They should start falling to the ground immediately. This may
					// be controversial. It makes gameplay seem more irratic.
					// TODO: Decide whether this block should be thrown away.
					if(!holdingJump){
						jumpCanceled = true;
						canDoubleJump = true;
						if(!jumpCanceled && moveDirection.y > 0){
							jumpCanceled = true;
							moveDirection = new Vector3(inputXAxis, .2f, 0);	
						}
					}
					
					// The user has hit their head on a ceiling. 
					// Hithead remains true, until one of the collision detectors says
					// so. We have to make sure the block below is only executed once
					// while they are in the air.
					// See Below: 
					if(isHittingHead && moveDirection.y > 0){
						jumpCanceled = true;
						moveDirection = new Vector3(inputXAxis, .2f, 0);		
					}		
					
					// move the sprite
					moveDirection.x*=walkingSpeed;
					
					// Check if the player can perform a Double Jump
					//if(Database.getPlayerHasDoubleJump()){
					//	if(canDoubleJump && !didDoubleJump){
					//		// Is the user holding jump?
					//		if(holdingJump){
					//			didDoubleJump = true;
					//			moveDirection.y = jumpingPower;
					//		}
					//	}
					//}
				}else{
					// move the sprite falling sprite
					moveDirection = new Vector3(inputXAxis, moveDirection.y, 0);
					moveDirection.x*=walkingSpeed;
				}
				
			}
			
			// Set player rotation
			if(moveDirection.x > 0){
				currentRoatation = 0;
			}else if(moveDirection.x < 0){
				currentRoatation = 180;
			}else{
				currentRoatation = transform.eulerAngles.y;
			}
		}
		
		// We apply simple gravity here gravity*Time.deltaTime will give you 
		// the amount of gravity you define every second.
		//moveDirection.y-=gravity*Time.deltaTime;
		moveDirection.y-=gravity;
		
		// Limit the falling walkingSpeed
		if(moveDirection.y < maxFallingSpeed){
			moveDirection.y = maxFallingSpeed;
		}
		
		//Debug.Log(moveDirection.y);
		if(moveDirection.y<0){
			isFalling = true;
		}else{
			isFalling = false;
		}
					
		if(mover!=null){
			//Debug.Log(moveDirection);
			mover.Move(moveDirection*Time.deltaTime);
		
			// If we are colliding with NOTHING, then we are definantly off the ground
			// and we are not hitting our head
			if(!mover.COLLISION){
				// Off Ground
				isGrounded = false;
				isTouchingSide = false;
				isHittingHead = false;
			}else{
				// If we are touching the ground, then we are obviously isGrounded.
				// Unless we are stuck under and object and the floor. Hithead and
				// ground should NEVER be true at the same time.
				if (mover.COLLISION_BOTTOM){
					// Only touching the floor
					isGrounded = true;
					isTouchingSide = false;
					isHittingHead = false;
				}else{
					isGrounded = false;
				}
				
				// If use is on the sides, we make sure we set isGrounded = false. If
				// the system thinks he is isGrounded, it will let him try to walk 
				// into the walls. 
				// To slide down a wall, the user must not be touching the ground or
				// ceiling.
				if (mover.COLLISION_HORIZONTAL){
					// Only touching the sides
					isGrounded = false;
					isTouchingSide = true;
					isHittingHead = false;
				}
				// If the user has hit their had, we have to make sure they start
				// to fall on the next frame.
				if (mover.COLLISION_TOP){
					// Only hit the top
					isHittingHead = true;
					isGrounded = false;
				}
			}
			
			// Set direction...
			if(mover.MOVED_RIGHT){
				animator.faceRight();
			}else if(mover.MOVED_LEFT){
				animator.faceLeft();
			}else{
				// Just stay the same... beoch
			}
				/*
			if(isGrounded){
				// only ground animations //
				//if(isKneeling){
					// Kneeling
					//DoAnimate(3, 1, true, -1);
				//	return;
				//}
				//if(moveDirection.x!=0){
				if(mover.MOVED_LEFT || mover.MOVED_RIGHT){
					// Walking
					//DoAnimate(1, 2, false, -1);
					animator.Pause();
					return;
				}
				if(mover.COLLISION_STAIRS){
					// Standing on stairs
					//DoAnimate(6, 1, true, -1);
				}else{
					// Just standing
					//DoAnimate(0, 1, true, -1);
					animator.DoAnim("Idle");
				}
			}else{
				if(isFalling){
					if(isTouchingSide){
						//DoAnimate(12, 1, true, -1);
						return;
					}
					//DoAnimate(2, 3, true, -1, true);
					// Falling
					return;
				}
				if(isJumping){
					// Jumping
					//DoAnimate(2, 1, true, -1, true);
					if(animator.animationName.Equals("Jump")){
						animator.DoAnim("Jump");
					}else{
						animator.PlayAnim("Jump");
					}
					
				}
			}
			*/
		}
		
	
}
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	//}
		/*
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, currentRoatation, transform.eulerAngles.z);

		
		// Do Animations
		if(isHurt){
			//DoAnimate(11, 2, true, -1);
			return;
		}
		if(isGrounded){
			// only ground animations //
			if(isKneeling){
				// Kneeling
				//DoAnimate(3, 1, true, -1);
				return;
			}
			if(moveDirection.x!=0){
				// Walking
				//DoAnimate(1, 2, false, -1);
				return;
			}
			if(isOnStairs){
				// Standing on stairs
				//DoAnimate(6, 1, true, -1);
			}else{
				// Just standing
				//DoAnimate(0, 1, true, -1);
			}
		}else{
			if(isFalling){
				if(isTouchingSide){
					//DoAnimate(12, 1, true, -1);
					return;
				}
				//DoAnimate(2, 3, true, -1, true);
				// Falling
				return;
			}
			if(isJumping){
				// Jumping
				//DoAnimate(2, 1, true, -1, true);
			}
		}
		*/
	}
	
	/*
	// Simple string map to know what this sprite should have as far as 
	// AnimationMap
	void playAnimation(string type){
		if(isHurt){
			return;
		}
		if(type == "Stand" && !isAttacking){
			// 0
			controller.height = this.colliderHeight;
			controller.radius = this.colliderRadius;
			controller.center = new Vector3(0, this.colliderCenterY, 0);

		}
		if(type == "Run" && !isAttacking){
			// You don't want to stop the animation for Run so it looks fluid.
			// Do just continues from last point, starts it if necissary.
			// 1 
			//controller.height = 1;
			//controller.radius = this.colliderRadius;
			//controller.center = new Vector3(0, this.colliderCenterY, 0);
			DoAnimate(1, 2, false, -1);
		}
		if(type == "Jump"){
			// 2
			//controller.height = 1;
			//controller.radius = this.colliderRadius;
			//controller.center = new Vector3(0, this.colliderCenterY, 0);
			//Debug.Log("Jump animation");
			DoAnimate(2, 1, true, -1, true);
		}
		if(type == "Fall"){
			// 2
			//controller.height = 1;
			//controller.radius = this.colliderRadius;
			//controller.center = new Vector3(0, this.colliderCenterY, 0);
			//Debug.Log("Jump animation");
			DoAnimate(2, 3, true, -1, true);
		}
		if(type == "Kneel" && !isAttacking){
			// 3
			controller.height = this.colliderKneelHeight;
			controller.radius = this.colliderKneelRadius;
			controller.center = new Vector3(0, this.colliderKneelCenterY, 0);
			DoAnimate(3, 1, true, -1);
		}
		if(type == "MagicAttackStand"){
			// 3
			//controller.height = this.colliderKneelHeight;
			//controller.radius = this.colliderKneelRadius;
			//controller.center = new Vector3(0, this.colliderKneelCenterY, 0);
			DoAnimate(7, 1, true, -1);
		}
		if(type == "MagicAttackKneel"){
			// 3
			//controller.height = this.colliderKneelHeight;
			//controller.radius = this.colliderKneelRadius;
			//controller.center = new Vector3(0, this.colliderKneelCenterY, 0);
			DoAnimate(8, 1, true, -1);
		}
		
		if(type == "SwordAttackStand"){
			// 3
			//controller.height = this.colliderKneelHeight;
			//controller.radius = this.colliderKneelRadius;
			//controller.center = new Vector3(0, this.colliderKneelCenterY, 0);
			DoAnimate(9, 1, true, -1);
		}
		if(type == "SwordAttackKneel"){
			// 3
			//controller.height = this.colliderKneelHeight;
			//controller.radius = this.colliderKneelRadius;
			//controller.center = new Vector3(0, this.colliderKneelCenterY, 0);
			DoAnimate(10, 1, true, -1);
		}
		
		if(type == "FaceForward"){
			// 4
			DoAnimate(4, 1, true, -1);
		}
		
		if(type == "Hurt"){
			// 4
			DoAnimate(11, 1, true, -1);
		}
	}
	
	
	void OnControllerColliderHit(ControllerColliderHit hit) {
		if(hit.gameObject.CompareTag("Stairs")){
			isOnStairs = true;
			return;
		}
		isOnStairs = false;
	}
	
	public override void GOOnTriggerStay(Collider other) {
		this.GOOnTriggerEnter(other);
	}
	public override void GOOnTriggerEnter(Collider other) {
		if(other.CompareTag("Enemy")){
			if(isHurt){
				return;
			}
			isHurt = true;
			isHurtClock = isHurtDuration;
			if(other.transform.position.x > gameObject.transform.position.x){
				// on the right
				currentRoatation = 0;
			}else{
				// on the left
				currentRoatation = 180;
			}
		}
	}

	
	
	
	private GameObject longRangeWeapon = null;
	private GameObject WeaponSword = null;
	
	private void DoAttack2(){
			// Prefab offsets rotation.
			//InstanciatePrefab(longRangeWeapon, .5f, .8f, gameObject);
			
			float xoff = fireDistance;;
			float yoff = fireHeight;
			if(isKneeling){
				yoff = fireHeightKneel;
			}
			if(currentRoatation > 0){
				xoff = -1*fireDistance;//.5f;
			}
			// TODO: REwrite this
			isAttacking = true;
			
			if(!isKneeling){
				//playAnimation("MagicAttackStand");
			
			}else{
				//playAnimation("MagicAttackKneel");
			}
			Scene.InstanciatePrefab(longRangeWeapon, new Vector3(gameObject.transform.position.x+xoff, gameObject.transform.position.y+yoff, 0), gameObject.transform.rotation, "");		
	}
	
	
	private void DoAttack1(){
			// Prefab offsets rotation.
			isAttacking = true;
			
			if(!isKneeling){
				//playAnimation("SwordAttackStand");
			}else{
				//playAnimation("SwordAttackKneel");
			}

			float yoff = 0;
			if(isKneeling){
				yoff = swordAttackKneel;
			}
			GameObject swizle = Scene.InstanciatePrefab(WeaponSword, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y+yoff, 1), gameObject.transform.rotation, "");		
			Sword sweap = (Sword)swizle.GetComponent("Sword");
			sweap.setPlayer(gameObject);
			sweap.setYoffset(yoff);
			
	}
	*/