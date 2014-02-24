using System;
using UnityEngine;


public abstract class IBuilderWindow {
	public Architect architect = null;		
	
	
	public Rect screenPos;
	
	int pos = 1; 
	
	public string title = "Nothing";
	
	public IBuilderWindow(){
	}
	
	public IBuilderWindow(Architect g){
		architect = g;
	}
	
	
	public void Window() {
		// left
		if(pos == 1){
			this.screenPos = new Rect(Screen.width - 256, 0, 256, Screen.height);
		// center
		}else if(pos == 2){
			this.screenPos = new Rect((Screen.width/2)-128, 0, 256, Screen.height);
			
		// right
		}else if(pos == 0){
			this.screenPos = new Rect(0, 0, 256, Screen.height);
		}
		GUI.backgroundColor = new Color(1,1,1,1);
		GUI.Window(pos, screenPos, Create, title);
	}
	
	public bool isMouseOver(){
		return screenPos.Contains(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
	}
	
	public void setPosition(int pos){
		this.pos = pos;
	}
	
	public abstract void Update();
	
	public abstract void Create(int id);
	
	public abstract void  CleanUp();
}