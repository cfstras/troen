using UnityEngine;
using System.Collections;

public class Tail : MonoBehaviour {
	
	public GameObject player;
	
	public static readonly float fallingSpeed = 0.1f;
	
	private bool falling = false;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(GameManager.instance.pause) {
			return;
		}
		if(falling) {
			transform.Translate(Vector3.down*fallingSpeed*Time.deltaTime);
			if(transform.position.y < (-5.0f-Player.playerHeight)) {
				Destroy(gameObject);
			}
		}
	}
	
	void FallDown() {
		falling = true;
	}
}
