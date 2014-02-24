using UnityEngine;
using System.Collections;

public class AnimatedSprite : Sprite
{

	public bool playOnStart = false;
	
	public enum EndAction {
		DoNothing,
		Loop,
		LoopReverse,
		Destroy
	};
	
	public EndAction endAction = EndAction.Loop;
	
	private enum AnimationState {
		Idle,
		Init,
		Playing,
		PlayFrame,
		NextFrame,
		StateChange,
		Complete
	}
	
	private AnimationState state = AnimationState.Idle;
	
	private int loopdir = 1;
	
	public int defaultSpeed = 32;
	int speed = 0;
	public string animationName = "";
	public Anim[] animations;
	
	private bool animActive = true;
		
	private int curAnimation=0;
	
	private int curAnimFrame=0;
	
	public void Pause(){
		animActive = false;
	}
	public void Continue(){
		animActive = true;
	}
	
	
	
	public override void Start(){
		if(playOnStart){
			state = AnimationState.Init;
		}
		base.Start();
	}
	
	private int findAnimation(string name){
		int ix = 0;
		foreach(Anim asanim in this.animations){
			if(asanim.name.Equals(name)){
				break;
			}
			ix++;
		}
		return ix;
	}
	
	private void loadAnimation(string anim){
		curAnimation = findAnimation(anim);
		
		
	}
	
	/*
	public void Continue(string anim){
		animActive = true;
		if(!anim.Equals(animationName)){
			curAnimFrame = 0;
			animationName = anim;
			state = AnimationState.Init;
		}else{
			//
		}
	}
	
	public void Play(string anim){
		curAnimFrame = 0;
		animationName = anim;
		animActive = true;
		state = AnimationState.Init;
	}
	*/
	
	public override void Update(){
		if(animActive){
			switch(state){
			case AnimationState.Idle: 
				break;
			case AnimationState.Init:
				if(animations!=null){
					
					if(animationName == null || animationName.Length==0){
						curAnimation = 0;
					}else{
						loadAnimation(animationName);
					}
					
					if(animations[curAnimation].frames!=null && curAnimFrame>=0 && curAnimFrame < animations[curAnimation].frames.Length){
						// We seem to be good to go, lets fire this bitch up.
						state = AnimationState.PlayFrame;
					}else{
						// default frame is also shit
						state = AnimationState.Idle;
					}
				}else{
					// defaultAnimation is shit.
					state = AnimationState.Idle;
				}
				break;
			case AnimationState.PlayFrame:
				if(curAnimFrame > -1 && curAnimFrame < animations[curAnimation].frames.Length){
					AnimFrame frame = animations[curAnimation].frames[curAnimFrame];
					target_frame = frame.index;
					
					// Default speed
					speed = defaultSpeed;
					
					// Then animation level
					if(animations[curAnimation].speed > 0){
						speed = animations[curAnimation].speed;
					}
					// Lastly individual frame level
					if(frame.speed > 0){
						speed = frame.speed;
					}

					StartCoroutine(StateUpdate(AnimationState.NextFrame, speed));
				}else{
					state = AnimationState.Complete;
				}
				break;
			case AnimationState.NextFrame:
				curAnimFrame += loopdir;
				state = AnimationState.PlayFrame;
				setIsDirty(true);
				break;
			case AnimationState.StateChange:
				break;
			case AnimationState.Complete:
				switch(animations[curAnimation].endAction){
					case EndAction.Loop:
						curAnimFrame = 0;
						loopdir = 1;
						state = AnimationState.PlayFrame;
						break;
					case EndAction.LoopReverse:
						loopdir *= -1;
						curAnimFrame += loopdir;
						state = AnimationState.NextFrame;
						break;
					case EndAction.DoNothing:
						state = AnimationState.Idle;
						break;
					case EndAction.Destroy:
						DestroyObject(gameObject);
						break;
				}
				break;
			}
		}
		base.Update();
	}
	
    IEnumerator StateUpdate(AnimationState next, int time) {
		//Debug.Log(Time.deltaTime);
		state = AnimationState.StateChange;
        yield return new WaitForSeconds((float)(time/1000.0f));
		state = next;
    }
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	/*
	/// <summary>
	/// Starts playing the specified animation
	/// Note: this doesn't resume from a pause,
	/// it completely restarts the animation. To
	/// unpause, use <see cref="UnpauseAnim"/>.
	/// </summary>
	/// <param name="anim">A reference to the animation to play.</param>
	/// <param name="frame">The zero-based index of the frame at which to start playing.</param>
	public void PlayAnim(UVAnimation anim, int frame)
	 */
	public void PlayAnim(string name){
		// set the current animation frame to zero
		SetCurFrame(0);
		// set the current animation name
		animationName = name;
		// set animation active = true
		animActive = true;
		// set loop direction to forward (1)
		loopdir = 1;
		// set state to Init, so that it loads up
		state = AnimationState.Init;
	}
	
	/*
	/// <summary>
	/// Like PlayAnim, but plays the animation in reverse.
	/// See <see cref="PlayAnim"/>.
	/// </summary>
	/// <param name="anim">Reference to the animation to play in reverse.</param>
	public void PlayAnimInReverse(UVAnimation anim)
	 */
	public void PlayAnimInReverse(string name){
		// set the current animation frame to zero
		SetCurFrame(0);
		// set the current animation name
		animationName = name;
		// set animation active = true
		animActive = true;
		// set loop direction to forward (1)
		loopdir = -1;
		// set state to Init, so that it loads up
		state = AnimationState.Init;
	}
	
	/*
	/// <summary>
	/// Plays the specified animation only if it
	/// is not already playing.
	/// </summary>
	/// <param name="index">Index of the animation to play.</param>
	public void DoAnim(int index)
	 */
	public void DoAnim(string name){
		if(!name.Equals(animationName)){
			PlayAnim(name);
		}else{
			// set animation active = true
			animActive = true;
			// set loop direction to forward (1)
			loopdir = 1;
		}
	}
	
	/*
	/// <summary>
	/// Like DoAnim, but plays the animation in reverse.
	/// </summary>
	/// <param name="index">Index of the animation to play.</param>
	public void DoAnim(int index)
	 */
	public void DoAnimInReverse(string name){
		if(!name.Equals(animationName)){
			PlayAnimInReverse(name);
		}else{
			// set animation active = true
			animActive = true;
			// set loop direction to forward (1)
			loopdir = -1;
		}
	}
	
	/*
	/// <summary>
	/// Sets the current frame of the current animation immediately.
	/// </summary>
	/// <param name="index">Zero-based index of the desired frame.</param>
	public void SetCurFrame(int index)
	 */
	public void SetCurFrame(int index) {
		// set the current animation frame.
		curAnimFrame = index;
		
		// Force a frame update...
	}
	
	/*
	/// <summary>
	/// Stops the current animation from playing
	/// and resets it to the beginning for playing
	/// again.  The sprite then reverts to the static
	/// image.
	/// </summary>
	public override void StopAnim()
	 */
	public void StopAnim(){
		// set the current animation frame to zero
		SetCurFrame(0);
		// set animation active = true
		animActive = false;
		// set state to Init, so that it loads up
		state = AnimationState.Init;
	}
	
	/*
	/// <summary>
	/// Resumes an animation from where it left off previously.
	/// </summary>
	public void UnpauseAnim()
	 */
	public void UnpauseAnim(){
		// set animation active = true
		animActive = true;
		// set state to Init, so that it loads up
		state = AnimationState.PlayFrame;
	}
	
	/*
	//--------------------------------------------------------------
	// Accessors:
	//--------------------------------------------------------------
	/// <summary>
	/// Returns a reference to the currently selected animation.
	/// NOTE: This does not mean the animation is currently playing.
	/// To determine whether the animation is playing, use <see cref="IsAnimating"/>
	/// </summary>
	/// <returns>Reference to the currently selected animation.</returns>
	public UVAnimation GetCurAnim() { return curAnim; }
	 */
}

[System.Serializable]
public class Anim { 
	public string name;
	public int speed;
	public AnimatedSprite.EndAction endAction;
	public AnimFrame[] frames;
	
	public Anim(){
		speed = 0;
	}
}

[System.Serializable]
public class AnimFrame {
	public int index;
	public int speed;
	
	public AnimFrame(){
		speed = 0;
	}	
}
