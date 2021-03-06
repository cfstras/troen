using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	
	//constants
	public static float playerHeight = 0.0625f;
	public const float turnDeadZone = 0.05f;
	public static float playerSpeed = 0.8f;
	public const float headTurnSpeed = 1.0f;
	public static float tailBeginOffset = 0.5f;
	
	// Prefabs
	public GameObject tailPrefab;
	
	// Variables
	public LinkedList<InputEvent> inputEvents;
	public GameManager manager;
	
	public Color color;
	public int number;
	
	private GameObject head;
	private Quaternion headFromRotation;
	private Quaternion headStartRotation;
	private float headTurnValue;
	private float waitingToTurn;
	
	public List<GameObject> tails;
	public GameObject lastTail;
	private Vector3 lastTailStartPos;
	
	public Orientation orientation;
	public Orientation newOrientation;
	float speed;
	static float maxPlayerSpeed = 4;
	public float accel = 1;
	private float jumpingAccel;
	private float jumpingYPos;
	private float jumpingBaseY;
	
	private Vector3 nextPosition;
	
	public bool alive;
	public bool winner = false;
	public int points = 0;
	
	// Keyconfig
	public KeyCode[] keyCodes;
	
	private string comPort;
	private IO io;
	
	//Camera
	public Camera playerCamera;
	
	//text notification positons
	public float textPosX, textPosY;
	
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
	public void InitializePlayer () {
		// Init Lists
		tails = new List<GameObject>();
		inputEvents = new LinkedList<InputEvent>();
		FollowCamera();

		speed = playerSpeed;
		Transform headTrans = transform.Find("Head");
		if(headTrans == null) {
			Debug.LogError("Error: player head not found!");
		} else {
			head = headTrans.gameObject;
		}
		name = "Player "+(number+1).ToString();
	}
	
	/**
	 * Does everything a player has to do to be in the next round
	 * position and rotation must be set before calling this!
	 */
	public void newRound() {
		nextPosition = transform.position;
		headStartRotation = head.transform.localRotation;
		SetOrientation();
		SetColor();
		AddTail();
		
		transform.Find("Head").renderer.enabled = true;
		collider.enabled = true;
		collider.isTrigger = true;
		alive = true;
		winner = false;
		speed = playerSpeed;
		jumpingYPos = transform.position.y;
		jumpingAccel = 0;
		jumpingBaseY = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		if(manager.pause || !alive) {
			return;
		}
		if(io != null) {
			io.Update();
		}
		DoInput();
		
		//accelerate to normal speed
		float speedOld = speed;
		speed += accel * Time.deltaTime;
		if(speed >= maxPlayerSpeed) {
			speed = maxPlayerSpeed;
		}
		
		//TODO accelerate faster if next to wall
		
		//turn head
		
		//if(headTurnValue <= 0) {
		//	headTurnValue = 0;
		//} else {
		//	headTurnValue -= headTurnSpeed * Time.deltaTime;
		//}
		//head.transform.localRotation = Quaternion.Slerp(headFromRotation,headStartRotation,headTurnValue);
		
		//update position
		
		nextPosition = transform.position + OrientationToVector(orientation) *
			((speed + speedOld ) * 0.5f * Time.deltaTime);
		nextPosition.y = jumpingBaseY;
		
		//jump
		float oldAcc = jumpingAccel;
		jumpingAccel -= 9.81f*Time.deltaTime;
		if(jumpingAccel < -3.5f) {
			jumpingAccel = -3.5f;
		}
		jumpingYPos += ((jumpingAccel + oldAcc ) * 0.5f * Time.deltaTime);
		if(jumpingYPos < jumpingBaseY) {
			jumpingAccel = 0;
			jumpingYPos = jumpingBaseY;
		}
		
		if(waitingToTurn!=0) {
			Turn();
		}
		
		//TODO check if collided
		
		UpdateTail(7);
		nextPosition.y = jumpingYPos;
		orientation = newOrientation;
		transform.position = nextPosition;
	}
	
	void UpdateTail(float offset) {
		if(lastTail == null) {
			return;	
		}
		Vector3 or = OrientationToVector(orientation);
		or.Scale(transform.localScale);
		Vector3 tailEndPos = nextPosition - or * tailBeginOffset * offset;
		
		Vector3 tailStartPos = lastTailStartPos;
		//update tail
		lastTail.transform.position = Vector3.Lerp(
			lastTailStartPos,
			tailEndPos,0.5f);
		//OrientationToVector(orientation)*0.5f*lastTail.transform.localScale.z;
		lastTail.transform.localScale = new Vector3(
			Vector3.Distance(lastTailStartPos,tailEndPos),
			lastTail.transform.localScale.y,
			lastTail.transform.localScale.z);
		
	}
	
	void DoInput() {
		while(inputEvents.Count>0) {
			InputEvent e = inputEvents.First.Value;
			inputEvents.RemoveFirst();
			
			//apply event
			switch (e.axis) {
			case InputEvent.Axis.Direction:
				waitingToTurn = e.val;
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
	
	private void Turn() {
		float val = waitingToTurn;
		waitingToTurn = 0;
		if(val <= turnDeadZone && val >= -turnDeadZone) {
			return;
		}
		newOrientation = Orientation.North;
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
		SetOrientation();
		AddTail();
	}
	
	private void Throttle(float val) {
		//TODO brake and speed
	}
	
	private void Powerup(float val) {
		//TODO powerup
		if(val >= 0.9f) {
			jumpingAccel = 4.0f;
			Debug.Log("Player "+number+" jumps high.");
		}
	}
	
	public void SetOrientation() {
		transform.rotation = Quaternion.Euler(new Vector3(0,OrientationToAngle(newOrientation),0));
	}
	
	/**
	 * Destroy all tails this player has
	 * if instantly is true, the walls won't fall down but vanish now
	 */
	public void Kill(bool instantly) {
		if(!alive) {
			return;
		}
		alive = false;
		transform.Find("Head").renderer.enabled = false;
		collider.enabled = false;
		collider.isTrigger = false;
		lastTail = null;
		if(instantly) {
			foreach (GameObject g in tails) {
				Destroy (g);
			}
			tails.Clear();
		} else {
			foreach(GameObject g in tails) {
				if(g.active) {
					g.SendMessage("FallDown");
				}
			}
			tails.Clear();
		}
	}
	
	/**
	 * Delete this player
	 */
	public void Delete() {
		Destroy(gameObject);
		io = null;
	}
	
	private void AddTail() {
		UpdateTail(1);
		
		lastTail = (GameObject) Instantiate(tailPrefab);
		lastTail.name = "Tail "+number+" - "+tails.Count+" - ID: "+Random.value;
		lastTail.renderer.material.color = Color.Lerp(Color.grey,color,0.8f);
		lastTailStartPos = new Vector3(nextPosition.x,jumpingBaseY,nextPosition.z);
		Tail tail = (Tail)lastTail.GetComponent("Tail");
		tail.player = gameObject;
		tails.Add(lastTail);
		lastTail.transform.rotation = Quaternion.Euler(
			lastTail.transform.eulerAngles.x+transform.eulerAngles.x,
			lastTail.transform.eulerAngles.y+transform.eulerAngles.y,
			lastTail.transform.eulerAngles.z+transform.eulerAngles.z);
		UpdateTail(1);
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
	
	private void SetColor() {
		switch(number) {
		case 0:
			color = Color.red;
			break;
		case 1:
			color = Color.green;
			break;
		case 2:
			color = Color.blue;
			break;
		case 3:
			color = Color.yellow;
			break;
		case 4:
			color = Color.magenta;
			break;
		case 5:
			color = Color.cyan;
			break;
			//TODO add more colors
		default:
			color = new Color(Random.Range (0,1),Random.Range (0,1),Random.Range (0,1));
			break;
		}
		head.renderer.material.color = color;
	}
	
	public enum keyCodeIndex 
	{
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
	
	private void FollowCamera() {
		SmoothFollow follow =  (SmoothFollow) playerCamera.GetComponent("SmoothFollow");
		follow.target = transform;
	}
	
	void Collide(Collider otherObject) {
		if(alive) {
			if(otherObject.gameObject != lastTail) {
				Kill (false);
			}
			
			if(otherObject.name.Contains("Player")) {
				//otherobject is a player
				otherObject.SendMessage("Kill", false);
				otherObject.SendMessage("score",-1);
				score(-1);
				Debug.Log(otherObject.name + " and " + name + " destroyed each other");
			} else if(otherObject.name.Contains("Tail")) {
				//otherobject is a tail
				Tail tail = (Tail) otherObject.GetComponent("Tail");	
				if(name == tail.player.name) {
					score(-1);
					Debug.Log(name + " got destroyed by own tail");
				} else {
					tail.player.SendMessage("score",1);
					score(-1);
					Debug.Log(name + " got destroyed by tail from " + tail.player.name);
				}
			} else if(otherObject.tag == "wall") {
				//otherobject is a wall
				score(-1);
				Debug.Log(name + " got destroyed by a wall.");	
			}
		}
	}
	
	public void SetCOM(string port) {
		if(port != null && !port.Equals("")) {
			this.comPort = port;
			io = new IO(comPort);
			io.player = this;
		}
	}
	
	public void score(int p) {
		points += p;
	}
}
