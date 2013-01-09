using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	
	//constants
	public const float playerHeight = 0.5f;
	public const float turnDeadZone = 0.05f;
	public const float playerSpeed = 1.0f;
	
	// Prefabs
	public GameObject tailPrefab;
	
	// Variables
	public LinkedList<InputEvent> inputEvents;
	public GameManager manager;
	public List<GameObject> tails;
	public Orientation orientation;
	public float speed;
	
	private Vector3 nextPosition;
	
	// Keyconfig
	public KeyCode[] keyCodes;
	
	//Camera
	public float cameraDampTime = 0.15f;
	private Vector3 cameraVelocity = Vector3.zero;
	public Camera playerCamera;
	
	/**
	 * Use this to set the KeyCodes for a player
	 * 
	 */
	public void SetKeyCodes(KeyCode left, KeyCode right, KeyCode brake, KeyCode speed, KeyCode powerup) {
		keyCodes = new KeyCode[(int)keyCodeIndex.Size];
		keyCodes[(int)keyCodeIndex.Brake] = brake;
		keyCodes[(int)keyCodeIndex.Speed] = speed;
		keyCodes[(int)keyCodeIndex.Powerup] = powerup;
		keyCodes[(int)keyCodeIndex.Left] = left;
		keyCodes[(int)keyCodeIndex.Right] = right;
	}
	
	// Use this for initialization
	void Start () {
		// Init Lists
		tails = new List<GameObject>();
		inputEvents = new LinkedList<InputEvent>();
		speed = 1.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if(manager.pause) {
			return;
		}
		
		DoInput();
		UpdateCamera();
		//TODO accelerate to normal speed
		//TODO accelerate faster if next to wall
		
		nextPosition = transform.position
			+ OrientationToVector(orientation) * speed * Time.deltaTime;
		
		//TODO check if collided
		
		//TODO add tails
		
		transform.position = nextPosition;
	}
	
	void DoInput() {
		while(inputEvents.Count>0) {
			InputEvent e = inputEvents.First.Value;
			inputEvents.RemoveFirst();
			
			//apply event
			switch (e.axis) {
			case InputEvent.Axis.Direction:
				Turn(e.val);
				break;
			case InputEvent.Axis.Throttle:
				Throttle(e.val);
				break;
			case InputEvent.Axis.Powerup:
				Powerup(e.val);
				break;
			}
		}
	}
	
	private void Turn(float val) {
		if(val <= turnDeadZone && val >= -turnDeadZone) {
			return;
		}
		Orientation newOrientation = Orientation.North;
		switch(orientation) {
		case Orientation.North:
			if(val>0) {
				newOrientation = Orientation.East;
			} else {
				newOrientation = Orientation.West;
			}
			break;
		case Orientation.East:
			if(val>0) {
				newOrientation = Orientation.South;
			} else {
				newOrientation = Orientation.North;
			}
			break;
		case Orientation.South:
			if(val>0) {
				newOrientation = Orientation.West;
			} else {
				newOrientation = Orientation.East;
			}
			break;
		case Orientation.West:
			if(val>0) {
				newOrientation = Orientation.North;
			} else {
				newOrientation = Orientation.South;
			}
			break;
		}
		orientation = newOrientation;
		SetOrientation();
	}
	
	private void Throttle(float val) {
		//TODO brake and speed
	}
	
	private void Powerup(float val) {
		//TODO powerup
	}
	
	public void SetOrientation() {
		switch(orientation) {
		case Orientation.North:
			transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
			break;
		case Orientation.South:
			transform.rotation = Quaternion.Euler(new Vector3(0,180,0));
			break;
		case Orientation.East:
			transform.rotation = Quaternion.Euler(new Vector3(0,90,0));
			break;
		case Orientation.West:
			transform.rotation = Quaternion.Euler(new Vector3(0,270,0));
			break;
		}
	}
	
	public static Vector3 OrientationToVector(Orientation or) {
		switch(or) {
			case Orientation.North:
			return Vector3.forward;
		case Orientation.South:
			return Vector3.back;
		case Orientation.East:
			return Vector3.right;
		case Orientation.West:
			return Vector3.left;
		}
		return Vector3.zero;
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
	public enum Orientation {
		North, East, South, West
	}
	
	private void UpdateCamera() 
	{
			Vector3 point = playerCamera.WorldToViewportPoint(transform.position);
			Vector3 delta = transform.position - playerCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
			Vector3 destination = playerCamera.transform.position + delta;
			playerCamera.transform.position = Vector3.SmoothDamp(playerCamera.transform.position, destination, ref cameraVelocity, cameraDampTime);
		
	}
}
