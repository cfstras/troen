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
		windowRect = new Rect(Camera.main.pixelWidth/2-250, Camera.main.pixelHeight/2-250, 500, 500);
		playerCountField = "2";
		Debug.Log("CreateWorld Start()");
		inputSelectedPlayer = -1;
		inputSelectedKey = -1;
		
		createArena();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)) {
			if(showGUI) {
				manager.UnPause();
				showGUI = false;
			} else {
				manager.Pause();
				showGUI = true;
			}
		}
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
		//disable collison for floor
		planeFloor.collider.enabled = false;
	}
	
	void OnGUI () {
		if(showGUI) {
			windowRect = GUI.Window (0, windowRect, WindowFunction, "Settings");
		}
	}
	
	void WindowFunction (int windowID) {
		int ypos = 20;
		// how many players?
		GUI.Label (new Rect (10, ypos, 180, 20), "How many players?");
		playerCountField = GUI.TextField(new Rect(150, ypos, 50, 20), playerCountField, 3);
		int newPlayerCount;
		bool parseSuccess = int.TryParse(playerCountField, out newPlayerCount);
		if(newPlayerCount<2) {
			GUI.Label(new Rect(200,ypos,180,20),"Minimum is 2 players!");	
		}
		ypos += 30;
		
		//create storage for player inputMaxKeys
		if(parseSuccess && (newPlayerCount != playerCount || keyCodes == null || keyCodes.GetLength(0) != playerCount)) {
			inputSelectedPlayer = -1;
			inputSelectedKey = -1;
			KeyCode[,] newKeys = new KeyCode[newPlayerCount,inputMaxKeys];
			//TODO copy old values
			for(int p = 0; p < newPlayerCount && p < playerCount; p++) {
				for(int k = 0; k < inputMaxKeys; k++) {
					newKeys[p,k] = keyCodes[p,k];
				}
			}			
			keyCodes = newKeys;
			playerCount = newPlayerCount;
			DefaultKeys();
		}
		
		// set inputs for each player
		
		for(int i = 0; i < playerCount; i++) {
			int xpos = 70;
			int xsize = 75;
			int dist = 10;
			
			if(i==0) {
				GUI.Label(new Rect(xpos,ypos,xsize,20),"      Left");
				xpos += xsize + dist;
				GUI.Label(new Rect(xpos,ypos,xsize,20),"     Right");
				xpos += xsize + dist;
				GUI.Label(new Rect(xpos,ypos,xsize,20),"    Brake");
				xpos += xsize + dist;
				GUI.Label(new Rect(xpos,ypos,xsize,20),"    Speed");
				xpos += xsize + dist;
				GUI.Label(new Rect(xpos,ypos,xsize,20),"   Powerup");
				ypos += 25;
				xpos = 70;
			}
			
			//TODO: allow the players to change their names
			GUI.Label (new Rect (10, ypos, 150, 20), "Player " + (i+1).ToString()+":");
			
			if(GUI.Button(new Rect(xpos, ypos, xsize, 25),Enum.GetName(typeof(KeyCode),keyCodes[i,0]))) {
				inputSelectedPlayer = i; inputSelectedKey = 0;
			}
			xpos += xsize+dist;
			if(GUI.Button(new Rect(xpos, ypos, xsize, 25),Enum.GetName(typeof(KeyCode),keyCodes[i,1]))) {
				inputSelectedPlayer = i; inputSelectedKey = 1;
			}
			xpos += xsize+dist;
			if(GUI.Button(new Rect(xpos, ypos, xsize, 25),Enum.GetName(typeof(KeyCode),keyCodes[i,2]))) {
				inputSelectedPlayer = i; inputSelectedKey = 2;
			}
			xpos += xsize+dist;
			if(GUI.Button(new Rect(xpos, ypos, xsize, 25),Enum.GetName(typeof(KeyCode),keyCodes[i,3]))) {
				inputSelectedPlayer = i; inputSelectedKey = 3;
			}
			xpos += xsize+dist;
			if(GUI.Button(new Rect(xpos, ypos, xsize, 25),Enum.GetName(typeof(KeyCode),keyCodes[i,4]))) {
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
		
		ypos += 10;
		if (GUI.Button(new Rect(410, ypos, 75, 30), "Start") && keyCodes != null && keyCodes.GetLength(0) == playerCount) {
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
		if (GUI.Button(new Rect(300, ypos, 75, 30), "Quit") ) {
			Application.Quit();
		}
	}
	
	public readonly KeyCode[,] defaultKeys = {
		{KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.DownArrow, KeyCode.UpArrow, KeyCode.LeftControl},
		{KeyCode.A, KeyCode.D, KeyCode.S, KeyCode.W,KeyCode.F},
		{KeyCode.J, KeyCode.L, KeyCode.K, KeyCode.I,KeyCode.BackQuote},
		{KeyCode.Keypad4, KeyCode.Keypad6, KeyCode.Keypad5, KeyCode.Keypad8,KeyCode.KeypadPlus}
	};
	
	private void DefaultKeys() {
		for(int p = 0; p < playerCount; p++) {
			for(int k = 0; k < inputMaxKeys; k++) {
				if(keyCodes[p,k]==KeyCode.None && defaultKeys.GetLength(0) > p) {
					keyCodes[p,k] = defaultKeys[p,k];
				}
			}
		}
	}
}