using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	
	// Prefabs
	public GameObject tailPrefab;
	
	// Variables
	public LinkedList<InputEvent> inputEvents;
	public GameManager manager;
	public List<Tail> tails;
	
	// Keyconfig
	public KeyCode[] keyCodes;
	
	/**
	 * Use this to set the KeyCodes for a player
	 * 
	 */
	public void SetKeyCodes(KeyCode left, KeyCode right, KeyCode brake, KeyCode speed, KeyCode powerup) {
		keyCodes = new KeyCode[keyCodeIndex.Size];
		keyCodes[keyCodeIndex.Brake] = brake;
		keyCodes[keyCodeIndex.Speed] = speed;
		keyCodes[keyCodeIndex.Powerup] = powerup;
		keyCodes[keyCodeIndex.Left] = left;
		keyCodes[keyCodeIndex.Right] = right;
	}
	
	// Use this for initialization
	void Start () {
		// Init Lists
		tails = new List<Tail>();
		inputEvents = new LinkedList<InputEvent>();
		
	}
	
	// Update is called once per frame
	void Update () {
		DoInput();
		//TODO check if game paused
		//TODO check if collided
		//TODO add tails
	}
	
	void DoInput() {
		while(inputEvents.Count>0) {
			InputEvent e = inputEvents.First;
			inputEvents.RemoveFirst();
			
			//TODO apply event
		}
	}
	
	public void AddInputEvent(keyCodeIndex index) {
		InputEvent.Axis a = InputEvent.Axis.Null;
		float val=0;
		if(index == keyCodeIndex.Brake) {
			a = InputEvent.Axis.Throttle;
			val = -1;
		} else if(index == keyCodeIndex.Speed) {
			a = InputEvent.Axis.Throttle;
			val = 1;
		} else if(index == keyCodeIndex.Powerup) {
			a = InputEvent.Axis.Powerup;
			val = 1;
		} else if(index == keyCodeIndex.Left) {
			a = InputEvent.Axis.Direction;
			val = -1;
		} else if(index == keyCodeIndex.Right) {
			a = InputEvent.Axis.Direction;
			val = 1;
		}
		if(a == InputEvent.Axis.Null) {
			return;
		}
		inputEvents.AddLast(new InputEvent(a,val));
	}
	
	public enum keyCodeIndex {
		Left = 0,
		Right = 1,
		Speed = 2,
		Brake = 3,
		Powerup = 4,
		
		//... be sure to update this
		Size = 5
	}
}
