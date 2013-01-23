using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	
	public static float playerBaseLine = 4.0f;
	
	//Prefabs
	public GameObject playerPrefab;
	public GameObject cameraPrefab;
	
	public List<Player> players;
	
	//Settings
	public int numPlayers = 0;
	
	//Variables
	public bool pause = true;
	
	public float offsetX, offsetY;
	
	public static GameManager instance;
	
	string countdownText;
	
	void Start () {
		instance = this;
		//reset lists
		if(players==null) {
			players = new List<Player>();	
		}
		
		//delete old players
		foreach (Player p in players)
		{
			p.Kill(true);
			p.Delete();
		}
		players.Clear();
		
		
	}
	
	/**
	 * New Level: first call StartGame.
	 * Now, give the players their keyCodes (Player.SetKeyCodes())
	 */
	public void StartGame(int numPlayers) {
		this.numPlayers = numPlayers;
		Start();
		
		//create player instances
		for(int i=0;i<numPlayers;i++) {
			GameObject po = (GameObject)Instantiate (playerPrefab);
			po.name = "Player "+i;
			Player p = (Player) po.GetComponent(typeof(Player));
			p.manager = this;
			p.number = i;			
			p.playerCamera = ((GameObject) Instantiate(cameraPrefab)).camera;
			players.Add (p);	
		}
		
		//create cameras
		PositionCameras();
		SetCameraTextPositioningOffsets();
		PositionPlayerNotifications();
		
		//init players
		foreach (Player p in players) {
			p.InitializePlayer();	
		}
		
		newRound(false);
		
		//freeze time until UnPause()
		pause = true;
	}
	
	public void newRound(bool startNow) {
		//reset positions
		if(numPlayers == 2) {
			for(int i = 0; i < numPlayers; i++) {
				players[i].transform.position = new Vector3(
					playerBaseLine * Mathf.Pow(-1,i),
					-5.0f+Player.playerHeight,
					0.0f);
				if(i==0) {
					players[i].orientation = players[i].newOrientation = Player.Orientation.West;
				} else {
					players[i].orientation = players[i].newOrientation = Player.Orientation.East;
				}
				players[i].SetOrientation();
			}
		} else {
			int player = 0;
			int playersPerSide = Mathf.Min(numPlayers/4,1);
			for(int side=0; side<4; side++) {
				float sideX = 0, sideY = 0;
				Player.Orientation orientation = Player.Orientation.North;
				switch(side) {
				case 0:
					sideX = playerBaseLine;
					orientation = Player.Orientation.West;
					break;
				case 1:
					sideX = -playerBaseLine;
					orientation = Player.Orientation.East;
					break;
				case 2:
					sideY = playerBaseLine;
					orientation = Player.Orientation.South;
					break;
				case 3:
					sideY = -playerBaseLine;
					orientation = Player.Orientation.North;
					break;
				default:
					Debug.LogError("Error while positioning players");
					break;
				}
				for(int i=0; i < playersPerSide; i++) {
					if(player >= numPlayers) {
						break;
					}
					float offset = (1.0f/(playersPerSide+1)*(i+1))*10.0f-5.0f;
					if(sideY == 0) {
						sideY = offset;
					} else {
						sideX = offset;
					}
					players[player].transform.position = new Vector3(sideX,
						-5.0f+Player.playerHeight, 
						sideY);
					players[player].orientation = players[player].newOrientation = orientation;
					
					player++;
				}
			}
		}
		foreach (Player p in players) {
			p.newRound();
		}
		if(startNow) {
			StartCoroutine(Countdown("GO"));
		}
	}
	
	/**
	 * Unpauses the game
	 */
	public void UnPause() {
		StartCoroutine(Countdown("GO!"));
	}
	
	/**
	 * Pauses the game
	 */
	public void Pause() {
		pause = true;
	}
	
	System.Collections.IEnumerator Countdown(string text) {
		yield return new WaitForSeconds(1);
		countdownText = "3";
		yield return new WaitForSeconds(1);
		countdownText = "2";
		yield return new WaitForSeconds(1);
		countdownText = "1";
		yield return new WaitForSeconds(1);
		countdownText = text;
		pause = false;
		yield return new WaitForSeconds(1);
		countdownText = "";
	}
	
	// Update is called once per frame
	void Update () {
		DoInput();
		//check for dead players
		//do something?
		if(pause == false) {
			DoGame();
		}
	}
	
	void DoInput() {
		foreach (Player p in players) {
			for(int i=0;i<p.keyCodes.Length;i++) {
				if(Input.GetKeyDown(p.keyCodes[i]) && p.alive) {
					p.AddInputEvent((Player.keyCodeIndex)i);
				}
			}
		}
	}
	
	void DoGame() {
		int aliveCount = 0;
		foreach(Player p in players) {
			if(p.alive) {
				aliveCount++;
			}
		}
		if(aliveCount == 1) {
			// get last alive player
			Player last = null;
			foreach (Player p in players) {
				if(p.alive) {
					last = p;
				}
			}
			//wait five seconds then end game and give winner points
			StartCoroutine(DoEndGame(last));
		}
	}
	
	System.Collections.IEnumerator DoEndGame(Player player) {
		//TODO add some indicator here
		yield return new WaitForSeconds(5);
		if(player.alive) {
			player.winner = true;
			player.addPoint();
		}
		pause = true;
		player.alive = false;
		yield return new WaitForSeconds(1);
		countdownText = "PREPARE FOR NEXT ROUND!";
		foreach(Player p in players) {
			p.Kill(true);
		}
		yield return new WaitForSeconds(2);
		newRound(true);
	}
	
	void PositionCameras() {
		switch(numPlayers) {
			case 2:
				players[0].playerCamera.rect = new Rect(  0,0,0.5f,1);
				players[1].playerCamera.rect = new Rect(0.5f,0,0.5f,1);
				break;
			case 3:
				players[0].playerCamera.rect = new Rect(0,    0,0.5f,0.5f);
				players[1].playerCamera.rect = new Rect(0.5f,  0,0.5f,0.5f);
				players[2].playerCamera.rect = new Rect(0,  0.5f,0.5f,0.5f);
				break;
			case 4:
				players[0].playerCamera.rect = new Rect(0,    0,0.5f,0.5f);
				players[1].playerCamera.rect = new Rect(0.5f,  0,0.5f,0.5f);
				players[2].playerCamera.rect = new Rect(0,  0.5f,0.5f,0.5f);
				players[3].playerCamera.rect = new Rect(0.5f,0.5f,0.5f,0.5f);
				break;
		}
	}
	
	void PositionPlayerNotifications() {
		switch(numPlayers) {
			case 2:
				players[0].textPosX = 0;
				players[0].textPosY = 0;
				players[1].textPosX = Camera.main.pixelWidth/2;
				players[1].textPosY = 0;
				break;
			case 3:
				players[0].textPosX = 0;
				players[0].textPosY = 0;
				players[1].textPosX = 0;
				players[1].textPosY = Camera.main.pixelHeight/2;
				players[2].textPosX = Camera.main.pixelWidth/2;
				players[2].textPosY = Camera.main.pixelHeight/2;
				break;
			case 4:
				players[0].textPosX = 0;
				players[0].textPosY = 0;
				players[1].textPosX = Camera.main.pixelWidth/2;
				players[1].textPosY = 0;
				players[2].textPosX = 0;
				players[2].textPosY = Camera.main.pixelHeight/2;
				players[3].textPosX = Camera.main.pixelWidth/2;
				players[3].textPosY = Camera.main.pixelHeight/2;
				break;
		}
		Debug.Log("offsetX: " + offsetX + " offsetY: " + offsetY);
	}
	
	void SetCameraTextPositioningOffsets() {
		switch(numPlayers) {
			case 2:
				offsetX = Camera.main.pixelWidth/4;
				offsetY = Camera.main.pixelHeight/2;
				break;
			case 3:
				offsetX = Camera.main.pixelWidth/4;
				offsetY = Camera.main.pixelWidth/4;
				break;
			case 4:
				offsetX = Camera.main.pixelWidth/4;
				offsetY = Camera.main.pixelWidth/4;
				break;
		}
	}
	
	void OnGUI(){
		foreach(Player p in players) {
			if(p != null) {
				GUIStyle stylePoints = new GUIStyle();
				stylePoints.fontSize = 20;
				stylePoints.normal.textColor = p.color;
    			GUI.Label(new Rect(p.textPosX,p.textPosY,250,40),p.name+": "+p.points+" points",stylePoints);
				if(!p.alive && !p.winner) {
					GUIStyle style = new GUIStyle();
					style.fontSize = 40;
					style.normal.textColor = p.color;
					float ydiff = (numPlayers==2?125:250);
					GUI.Label(new Rect(p.textPosX+offsetX-125,p.textPosY+offsetY-ydiff,250,250),"YOU DIED!",style);
				}
				if(!p.alive && p.winner) {
					GUIStyle style = new GUIStyle();
					style.fontSize = 40;
					style.normal.textColor = p.color;
					float ydiff = (numPlayers==2?125:250);
					GUI.Label(new Rect(p.textPosX+offsetX-125,p.textPosY+offsetY-ydiff,250,250),"YOU WON!",style);
				}
			}
		}
		GUIStyle styleCountdown = new GUIStyle();
		styleCountdown.fontSize = 50;
		styleCountdown.normal.textColor = Color.white;
		styleCountdown.alignment = TextAnchor.MiddleCenter;
		GUI.Label(new Rect(0,0,Camera.main.pixelWidth,Camera.main.pixelHeight),countdownText,styleCountdown);
	}

}
