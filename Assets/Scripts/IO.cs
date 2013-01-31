using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;

public class IO {
	public Player player;
	
	//accelerometer
	Vector3 dir = Vector3.zero;
	
	public bool leftDown;
	public bool rightDown;
	public float leftDownLast;
	public float rightDownLast;
	public float jumpLast;
	public static float buttonTime = 1/10.0f;
	
	public IO(){
	}
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	public void Update () {
		//get accelerometer data
		dir.x = Input.acceleration.x;
		dir.y = Input.acceleration.y;
		dir.z = Input.acceleration.z;
		
		player.accel = 1.0f;
		
		//input evens to player
		if (dir.y >= 0.6f) {
			if(!leftDown && leftDownLast+buttonTime < Time.time) {
				leftDownLast = Time.time;
				player.inputEvents.AddLast(new InputEvent(InputEvent.Axis.Direction, -1));
				Debug.Log("left.");
			}
			leftDown = true;
		} else {
			leftDown = false;
		}
		if(dir.y <= -0.6f) {
			if(!rightDown && rightDownLast+buttonTime < Time.time) {
				rightDownLast = Time.time;
				player.inputEvents.AddLast(new InputEvent(InputEvent.Axis.Direction, 1));
				Debug.Log("right.");
			}
			rightDown = true;
		} else {
			rightDown = false;
		}
		//jump
		if(dir.z >= 0.5f && jumpLast + buttonTime < Time.time) {
			jumpLast = Time.time;
			player.inputEvents.AddLast(new InputEvent(InputEvent.Axis.Powerup, 1));
			Debug.Log("jump");
		}
		
	}
}
