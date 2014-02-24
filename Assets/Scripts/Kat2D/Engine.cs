using UnityEngine;
using System.Collections;

public class Engine : MonoBehaviour {
	// Engine delta time will be used by other objects instead of Time.deltaTime
	// This will allow us to control the speed independently. 
	
	public bool isEditing = true;
	
	private static Engine instance = null;
	private static float deltaSpeed = 1;
	private GOFactory objFactory = null;
	private Architect architect = null;
	private SceneManager sceneManager = null;
	private KCamera KCameraObj = null;
	
	public static Engine getInstance() {
		return Engine.instance;
	}
	
	private enum EngineState {
		Loading,
		Running,
		Quitting
	}
	
	private EngineState state = EngineState.Loading;
	
	// Use this for initialization
	void Start () {
		// Set up the shit...
		Engine.instance = this;
		//world = World.getInstance();
		sceneManager = SceneManager.getInstance();
		objFactory = GOFactory.getInstance();
	}
	
	// Update is called once per frame
	void Update () {
		switch(state){
		case EngineState.Loading:
			// Lets load up a Camera.
			//GameObject cam = objFactory.Instanciate("KCamera");
			GameObject cam = GameObject.Find("KCamera");
			KCameraObj = (KCamera)cam.GetComponent(typeof(KCamera));
			KCameraObj.setPixelPerfect(false);
			
			if(isEditing){
				// Create Arhitect...
				GameObject archObject = objFactory.Instanciate("Architect");
				architect = (Architect)archObject.GetComponent(typeof(Architect));
				architect.setEngine(this);
				KCameraObj.setFollowTarget(archObject);
			}else{
				//sceneManager.loadScene("Builder");
				//sceneManager.processRooms();
				
				GameObject playerObject = GameObject.Find("Player");
				
				KCameraObj.setFollowTarget(playerObject);
			}
			state = EngineState.Running;
			break;
		} 
	}
	
	public float getDeltaTime(){
		return Time.deltaTime * deltaSpeed;
	}
	
	public KCamera getKCamera(){
		return this.KCameraObj;
	}
	public Camera getCamera(){
		return this.KCameraObj.getCamera();
	}
	
	public SceneManager getSceneManager(){
		return this.sceneManager;
	}
}