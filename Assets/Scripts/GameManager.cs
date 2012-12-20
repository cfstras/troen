using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	
	//Prefabs
	public GameObject playerPrefab;
	
	public List<Player> players;
	
	//Settings
	public int numPlayers = 0;
	
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
		
		//TODO reset positions
		
		//TODO start overlay with countdown
		
		//TODO freeze time until UnPause()
	}
	
	/**
	 * Unpauses the game
	 */
	public void UnPause() {
		//TODO
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
				if(Input.GetKey(p.keyCodes[i])) {
					p.AddInputEvent((Player.keyCodeIndex)i);
				}
			}
		}
	}
	

}
