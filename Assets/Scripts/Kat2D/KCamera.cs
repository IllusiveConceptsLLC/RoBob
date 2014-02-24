using UnityEngine;
using System.Collections;

public class KCamera : MonoBehaviour {
	// pixelPerfect 
	// if set to true, this script will automatically adjust the size of the camera so that
	//   one world unit = one pixel.
	private bool pixelPerfect = false;
	
	// followTarget
	// This will be the GameObject that this camera will follow. That is done by setting it as the parent.
	private GameObject followTarget = null;
		
	// setPixelPerfect ( bool pixelPerfect )
	// This method sets the pixel perfect value
	public void setPixelPerfect(bool pixelPerfect) {
		this.pixelPerfect = pixelPerfect;
	}
	
	public bool forceResize = false;
	
	// Start ()
	// Used for initialization
	void Start () { 
		
		// To start, push the Z index far enough back.
		
		Vector3 ppos = transform.position;
		ppos.x = 0;
		ppos.y = 0;
		//ppos.z = -500;
		ppos.z = 0;
		
		transform.position = ppos;
		
		//if(KEngine.isEditor) {
			camera.backgroundColor = Color.gray;
		//}
		
		// check for pixel perfect requirements
		resize();
		
		// set the taret to follow
		follow();
		
		transform.position = ppos;
		transform.localPosition = ppos;
	}
	
	// Update is called once per frame
	void Update () { 
		if(forceResize){
			resize ();
		}
	}
	
	// resize
	// This method will set the camera size to be pixel perfect if requested.
	private void resize() {
		if(this.pixelPerfect){
			if(!camera.isOrthoGraphic){
				camera.orthographic = true;
			}
			// Enhance!
			camera.orthographicSize = Screen.height / 2;
		}
	}
	
	public void setFollowTarget(GameObject target){
		this.followTarget = target;
		follow();
	}
	
	// follow
	// If follow game object is set, it will be set as this objects parent. Once parent, this object will 
	// automatically follow it.
	private void follow(){
		if(followTarget != null) {
			//transform.position = new Vector3(0, 0, 0);
			transform.parent = followTarget.transform;
		}
	}
	
	// Zoom (Float amount)
	// This method attempts to apply a zoom to the camera. Based on whether it is orthographic or not.
	public void Zoom(float amount) {
		if(camera.isOrthoGraphic){
			camera.orthographicSize += amount;
			// Oh man, magic numbers!! NOOOOOO
			// But using the Editor, I found that 50 gave it a decent look in this case. 
			if(camera.orthographicSize < 50){
				camera.orthographicSize = 50;
			}else if(camera.orthographicSize > 1000){
				camera.orthographicSize = 1000;
			}
		}else{
			Vector3 newPos = camera.transform.position;
			newPos.z += amount;
			if(newPos.z > 0){
				newPos.z = 0;
			}
		}
	}
	
	public Camera getCamera() {
		return camera;
	}
}
