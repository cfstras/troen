using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	
	//constants
	public const float playerHeight = 0.5f;
	public const float turnDeadZone = 0.05f;
	public const float playerSpeed = 2.0f;
	public const float headTurnSpeed = 1.0f;
	public const float tailFallSpeed = 0.5f;
	
	// Prefabs
	public GameObject tailPrefab;
	
	// Variables
	public LinkedList<InputEvent> inputEvents;
	public GameManager manager;
	
	public GameObject head;
	private Quaternion headFromRotation;
	private Quaternion headStartRotation;
	private float headTurnValue;
	
	public List<GameObject> tails;
	public GameObject lastTail;
	private Vector3 lastTailStartPos;
	
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
		Transform headTrans = transform.Find("Head");
		if(headTrans == null) {
			Debug.LogError("Error: player head not found!");
		} else {
			head = headTrans.gameObject;
		}
		headStartRotation = head.transform.localRotation;
		AddTail();
	}
	
	// Update is called once per frame
	void Update () {
		if(manager.pause) {
			return;
		}
		
		DoInput();
		FollowCamera();
		//TODO accelerate to normal speed
		//TODO accelerate faster if next to wall
		
		
		//turn head
		
		//if(headTurnValue <= 0) {
		//	headTurnValue = 0;
		//} else {
		//	headTurnValue -= headTurnSpeed * Time.deltaTime;
		//}
		//head.transform.localRotation = Quaternion.Slerp(headFromRotation,headStartRotation,headTurnValue);
		//update position
		nextPosition = transform.position + OrientationToVector(orientation) * speed * Time.deltaTime;
		
		//TODO check if collided
		
		//update tail
		lastTail.transform.position = Vector3.Lerp(lastTailStartPos,nextPosition,0.5f);
		
		
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
		//turn head in opposite directon, to make him turn smooth afterwards
		//float deltaOrientation = OrientationToAngle(newOrientation) - OrientationToAngle(orientation);
		//headFromRotation = Quaternion.Euler(0, -deltaOrientation, 0);
		//headTurnValue = 1;
		//head.transform.localRotation = headFromRotation;
		
		orientation = newOrientation;
		SetOrientation();
		AddTail();
	}
	
	private void Throttle(float val) {
		//TODO brake and speed
	}
	
	private void Powerup(float val) {
		//TODO powerup
	}
	
	public void SetOrientation() {
		transform.rotation = Quaternion.Euler(new Vector3(0,OrientationToAngle(orientation),0));
	}
	
	/**
	 * Destroy all tails this player has
	 * if instantly is true, the walls won't fall down but vanish now
	 */
	public void Destroy(bool instantly) {
		if(instantly) {
			foreach (GameObject g in tails) {
				Destroy (g);
			}
		} else {
			StartCoroutine(Fall());
		}
	}
	/**
	 * Make this players tails fall down
	 * called when he dies through Destroy()
	 */
	System.Collections.IEnumerator Fall() {
		//TODO convert this to an Update() in the Tails
		List<GameObject> myTails = tails;
		float y = -5.0f+playerHeight;
		while(y > (-5.0f-playerHeight)) {
			yield return new WaitForSeconds(1/30.0f);
			y -= tailFallSpeed * Time.deltaTime;
			foreach (GameObject g in tails) {
				g.transform.position = new Vector3(g.transform.position.x, y, g.transform.position.z);
			}
		}
	}
	
	private void AddTail() {
		lastTail = (GameObject) Instantiate(tailPrefab);
		lastTailStartPos = transform.position;
		tails.Add(lastTail);
		lastTail.transform.rotation = transform.rotation;
	}
	
	public static float OrientationToAngle(Orientation or) {
		switch(or) {
		case Orientation.North:
			return 0;
		case Orientation.South:
			return 180;
		case Orientation.East:
			return 90;
		case Orientation.West:
			return 270;
		}
		return 0;
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
	
	private void FollowCamera() 
	{
		SmoothFollow follow =  (SmoothFollow) playerCamera.GetComponent("SmoothFollow");
		follow.target = transform;
	}
}
