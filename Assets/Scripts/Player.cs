using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	
	//constants
	public static float playerHeight = 0.0625f;
	public const float turnDeadZone = 0.05f;
	public static float playerSpeed = 2.0f;
	public const float headTurnSpeed = 1.0f;
	public const float tailFallSpeed = 10.0f;
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
	
	private Vector3 nextPosition;
	
	public bool alive = true;
	public int points = 0;
	
	// Keyconfig
	public KeyCode[] keyCodes;
	
	//Camera
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
		headStartRotation = head.transform.localRotation;
		SetOrientation();
		SetColor();
		AddTail();
		name = "Player "+number.ToString();
	}
	public void newRound() {
		transform.Find("Head").renderer.enabled = true;
		collider.enabled = true;
		collider.isTrigger = true;
	}
	
	// Update is called once per frame
	void Update () {
		if(manager.pause || !alive) {
			return;
		}
		
		DoInput();
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
		
		if(waitingToTurn!=0) {
			Turn();
		}
		
		//TODO check if collided
		
		UpdateTail(1);
		
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
	}
	
	public void SetOrientation() {
		transform.rotation = Quaternion.Euler(new Vector3(0,OrientationToAngle(newOrientation),0));
	}
	
	/**
	 * Destroy all tails this player has
	 * if instantly is true, the walls won't fall down but vanish now
	 */
	public void Kill(bool instantly) {
		alive = false;
		points--;
		transform.Find("Head").renderer.enabled = false;
		collider.enabled = false;
		collider.isTrigger = false;
		if(instantly) {
			foreach (GameObject g in tails) {
				Destroy (g);
			}
		} else {
			StartCoroutine(Fall(tails));
		}
	}
	
	/**
	 * Make this players tails fall down
	 * called when he dies through Destroy()
	 */
	System.Collections.IEnumerator Fall(List<GameObject> myTails) {
		//TODO convert this to an Update() in the Tails
		float y = -5.0f+playerHeight;
		while(y > (-5.0f-playerHeight)) {
			yield return new WaitForSeconds(1/30.0f);
			y -= tailFallSpeed * Time.deltaTime;
			foreach (GameObject g in myTails) {
				g.transform.position = new Vector3(g.transform.position.x, y, g.transform.position.z);
			}
		}
	}
	
	private void AddTail() {
		UpdateTail(1);
		
		lastTail = (GameObject) Instantiate(tailPrefab);
		lastTail.name = "Tail "+number+" - "+tails.Count;
		lastTail.renderer.material.color = Color.Lerp(Color.grey,color,0.8f);
		lastTailStartPos = transform.position;
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
				Kill (true);
			}
			//otherobject is a player
			if(otherObject.name.Contains("Player")) {
				otherObject.SendMessage("Kill", false);
				Debug.Log(otherObject.name + " destroyed himself and " + name);
			}
			//otherobject is a tail
			if(otherObject.name.Contains("Tail")) {
				Tail tail = (Tail) otherObject.GetComponent("Tail");
				tail.player.SendMessage("addPoint");
				Debug.Log(name + " got destroyed by tail from " + tail.player.name);
			}
			//otherobject is a wall
			if(otherObject.tag == "wall") {
				Debug.Log(name + " got destroyed by a wall.");	
			}
		}
	}
	
	public void addPoint() {
		points++;
	}
	
}
