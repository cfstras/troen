using UnityEngine;
using System.Collections;
using System;

public class CreateWorld : MonoBehaviour {
	
	//GAME
	public GameManager manager;
	protected GameObject planeFloor, planeLeft, planeRight, planeFront, planeBack;
	public GameObject prefabPlane;
	
	//GUI
	private Rect windowRect;
	
	KeyCode[,] keyCodes;
	string playerCountField = "2";
	int inputSelectedPlayer;
	int inputSelectedKey;
	int inputMaxKeys = 5;
	
	//DATA
	protected bool showGUI = true;
	private int playerCount = 0;
	
	// Use this for initialization
	void Start () {
		windowRect = new Rect(Camera.main.pixelWidth/2-150, Camera.main.pixelHeight/2-250, 500, 500);
		playerCountField = "2";
		Debug.Log("CreateWorld Start()");
		inputSelectedPlayer = -1;
		inputSelectedKey = -1;
		
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
		if(Input.GetKeyDown(KeyCode.Escape)) {
			if(showGUI) {
				manager.UnPause();
				showGUI = false;
			} else {
				manager.Pause();
				showGUI = true;
			}
		}
		if(showGUI) {
			windowRect = GUI.Window (0, windowRect, WindowFunction, "Settings");
		}
	}
	
	void WindowFunction (int windowID) {
		int ypos = 20;
		// how many players?
		GUI.Label (new Rect (10, ypos, 180, 20), "How many players?");
		playerCountField = GUI.TextField(new Rect(185, ypos, 50, 20), playerCountField, 3);
		int newPlayerCount;
		bool parseSuccess = int.TryParse(playerCountField, out newPlayerCount);
		ypos += 30;
		
		//create storage for player inputMaxKeys
		if(parseSuccess && (newPlayerCount != playerCount || keyCodes == null || keyCodes.GetLength(0) != playerCount)) {
			playerCount = newPlayerCount;
			inputSelectedPlayer = -1;
			inputSelectedKey = -1;
			keyCodes = new KeyCode[playerCount,inputMaxKeys];
			//TODO copy old values
		}
		
		// set inputs for each player
		GUI.Label(new Rect(120,ypos,50,20),"Left");
		GUI.Label(new Rect(180,ypos,50,20),"Right");
		GUI.Label(new Rect(240,ypos,50,20),"Brake");
		GUI.Label(new Rect(300,ypos,50,20),"Speed");
		GUI.Label(new Rect(360,ypos,50,20),"Powerup");
		ypos += 25;
		for(int i = 0; i < playerCount; i++) {
			//TODO: allow the players to change their names
			GUI.Label (new Rect (10, ypos, 150, 20), "Keys for player " + (i+1).ToString()+":");
			
			if(GUI.Button(new Rect(120, ypos, 50, 20),Enum.GetName(typeof(KeyCode),keyCodes[i,0]))) {
				inputSelectedPlayer = i; inputSelectedKey = 0;
			}
			if(GUI.Button(new Rect(180, ypos, 50, 20),Enum.GetName(typeof(KeyCode),keyCodes[i,1]))) {
				inputSelectedPlayer = i; inputSelectedKey = 1;
			}
			if(GUI.Button(new Rect(240, ypos, 50, 20),Enum.GetName(typeof(KeyCode),keyCodes[i,2]))) {
				inputSelectedPlayer = i; inputSelectedKey = 2;
			}
			if(GUI.Button(new Rect(300, ypos, 50, 20),Enum.GetName(typeof(KeyCode),keyCodes[i,3]))) {
				inputSelectedPlayer = i; inputSelectedKey = 3;
			}
			if(GUI.Button(new Rect(360, ypos, 50, 20),Enum.GetName(typeof(KeyCode),keyCodes[i,4]))) {
				inputSelectedPlayer = i; inputSelectedKey = 4;
			}
			
			ypos += 30;
		}
		
		Event e = Event.current;
        
		if(e.isKey && e.type == EventType.KeyDown && e.keyCode != KeyCode.None &&
			inputSelectedKey >= 0 && inputSelectedKey < inputMaxKeys
			&& inputSelectedPlayer >= 0 && inputSelectedPlayer < playerCount) {
			keyCodes[inputSelectedPlayer,inputSelectedKey] = Event.current.keyCode;
			e.Use();
			inputSelectedPlayer = -1; inputSelectedKey = -1;
		}
		
		if (GUI.Button(new Rect(100, ypos, 100, 30), "Start") && keyCodes != null && keyCodes.GetLength(0) == playerCount) {
			showGUI = false;
			KeyCode left, right, brake, speed, power;
			manager.StartGame(playerCount);
			for(int i=0;i < playerCount; i++) {
				left = keyCodes[i,0];
				right = keyCodes[i,1];
				brake = keyCodes[i,2];
				speed = keyCodes[i,3];
				power = keyCodes[i,4];
				manager.players[i].SetKeyCodes(left,right,brake,speed,power);
			}
			manager.UnPause();
		}    
	}
}