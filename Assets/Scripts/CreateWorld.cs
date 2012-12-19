using UnityEngine;
using System.Collections;
using System;

public class CreateWorld : MonoBehaviour {
	//GAME
	public GameManager manager;
	public GameObject planeFloor, planeLeft, planeRight, planeFront, planeBack;
	public GameObject prefabPlane;
	//GUI
	private Rect windowRect;
	private int playerCount = 0;
	public int maxplayers = 10;
	//DATA
	// Use this for initialization
	void Start () {
		windowRect = new Rect(Camera.main.pixelWidth/2-150, Camera.main.pixelHeight/2-250, 500, 500);
		createArena();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void createArena() {
		planeFloor  = (GameObject) Instantiate(prefabPlane);
		planeLeft 	= (GameObject) Instantiate(prefabPlane);
		planeRight 	= (GameObject) Instantiate(prefabPlane);
		planeFront 	= (GameObject) Instantiate(prefabPlane);
		planeBack  	= (GameObject) Instantiate(prefabPlane);
		
		planeFloor.name = "planeFloor";
		planeLeft.name = "planeLeft";
		planeRight.name = "planeRight";
		planeFront.name = "planeFront";
		planeBack.name = "planeBack";
		
		planeBack.transform.rotation = Quaternion.Euler(90,0,0);
		planeFront.transform.rotation = Quaternion.Euler(-90,0,0);
		planeLeft.transform.rotation = Quaternion.Euler(-90,0,90);
		planeRight.transform.rotation = Quaternion.Euler(90,0,-90);
		
		planeBack.transform.position = new Vector3(0,0,-5f);
		planeFront.transform.position = new Vector3(0,0,5f);
		planeLeft.transform.position = new Vector3(5,0,0);
		planeRight.transform.position = new Vector3(-5,0,0);
		planeFloor.transform.position = new Vector3(0,-5,0);
	}
	void OnGUI () {
		windowRect = GUI.Window (0, windowRect, WindowFunction, "Setting");
	}
	void WindowFunction (int windowID) {
		string[] keyLeft = new string[maxplayers];
		string[] keyRight  = new string[maxplayers];
		string[] keyBrake = new string[maxplayers];
		string[] keySpeed = new string[maxplayers];
		string[] keyPower = new string[maxplayers];
		// how many player?
		GUI.Label (new Rect (10, 20, 180, 20), "How many players (Max. 10)?");
		playerCount = int.Parse(GUI.TextField(new Rect(185, 20, 50, 20), playerCount.ToString(), 2));		
		// set inputs for each player
		if(playerCount > maxplayers) {
				playerCount = maxplayers;
		}
		for(int i = 0;i<playerCount;i++) {
			//TODO: allow the players to change their names
			GUI.Label (new Rect (10, 50+i*30, 150, 20), "Keys for player " + (i+1).ToString()+":");
			keyLeft[i] = GUI.TextField(new Rect(120, 50+i*30, 50, 20), "", 15);		
			keyRight[i] = GUI.TextField(new Rect(180, 50+i*30, 50, 20), "", 15);		
			keyBrake[i] = GUI.TextField(new Rect(240, 50+i*30, 50, 20), "", 15);
			keySpeed[i] = GUI.TextField(new Rect(300, 50+i*30, 50, 20), "", 15);		
			keyPower[i] = GUI.TextField(new Rect(360, 50+i*30, 50, 20), "", 15);
		}
		if (GUI.Button(new Rect(100, 50+30*playerCount, 100, 30), "Start")) {
			KeyCode left, right, brake,speed,power;
			manager.numPlayers = playerCount;
			manager.Start();
			int i = 0;
			foreach(Player p in manager.players) {
				left = (KeyCode) Enum.Parse(typeof(KeyCode),keyLeft[i]);
				right = (KeyCode) Enum.Parse(typeof(KeyCode),keyRight[i]);
				brake = (KeyCode) Enum.Parse(typeof(KeyCode),keyBrake[i]);
				speed = (KeyCode) Enum.Parse(typeof(KeyCode),keySpeed[i]);
				power = (KeyCode) Enum.Parse(typeof(KeyCode),keyPower[i]);
				p.SetKeyCodes(left,right,brake,speed,power);
				i++;
			}
			//TODO: start game
		}    
	}
}