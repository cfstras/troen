using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	
	//Prefabs
	public Player playerPrefab;
	
	public List<Player> players;
	
	//Settings
	public int numPlayers = 2;
	
	/**
	 * New Level: first call start.
	 * Now, give the players their keyCodes (Player.SetKeyCodes())
	 */
	public void Start () {
		//reset lists
		if(players==null) {
			players = new List<Player>();	
		}
		
		//create/get player instances
		if(numPlayers != players.Count) {
			players.Clear();
			for(int i=0;i<numPlayers;i++) {
				Player p = (Player)Instantiate (playerPrefab);
				p.manager = this;
				players.Add (p);	
			}
		}
		
		//TODO reset positions
		
		//TODO start overlay with countdown
		
		//TODO freeze time until UnPause()
		
	}
	
	/**
	 * Unpauses the game
	 */
	void UnPause() {
		//TODO
	}
	
	// Update is called once per frame
	void Update () {
		DoInput();
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
