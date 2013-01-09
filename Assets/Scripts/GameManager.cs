using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	
	//Prefabs
	public GameObject playerPrefab;
	
	public List<Player> players;
	
	//Settings
	public int numPlayers = 0;
	
	//Variables
	public bool pause = true;
	
	void Start () {
		//reset lists
		if(players==null) {
			players = new List<Player>();	
		}
		
		//delete old players
		foreach (Player p in players)
		{
			Destroy(p);
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
			Player p = (Player) po.GetComponent(typeof(Player));
			p.manager = this;
			players.Add (p);	
		}
		
		//reset positions
		foreach (Player p in players) {
			p.transform.position = new Vector3(
				Random.Range(-4.5f,4.5f),
				-5.0f+Player.playerHeight,
				Random.Range(-4.5f,4.5f));
			
			float rot = Random.Range(0,4);
			if(rot<1) {
				p.orientation = Player.Orientation.North;
			} else if(rot<2) {
				p.orientation = Player.Orientation.South;
			} else if(rot<3) {
				p.orientation = Player.Orientation.East;
			} else {
				p.orientation = Player.Orientation.West;
			}
			p.SetOrientation();
		}
		
		//TODO start overlay with countdown
		
		//freeze time until UnPause()
		pause = true;
	}
	
	/**
	 * Unpauses the game
	 */
	public void UnPause() {
		pause = false;
	}
	
	/**
	 * Pauses the game
	 */
	public void Pause() {
		pause = true;
	}
	
	// Update is called once per frame
	void Update () {
		DoInput();
		//check for dead players
		//do something?
	}
	
	void DoInput() {
		
		foreach (Player p in players) {
			for(int i=0;i<p.keyCodes.Length;i++) {
				if(Input.GetKeyDown(p.keyCodes[i])) {
					p.AddInputEvent((Player.keyCodeIndex)i);
				}
			}
		}
	}
	

}
