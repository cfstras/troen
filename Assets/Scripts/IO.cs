using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;

public class IO {
	SerialPort stream = null;
	public string receivedData = "EMPTY";
	public Player player;
	
	//buttons
	int buttonVal;
	int leftButtonMask 	= 0x100;
	int rightButtonMask = 0x200;
	int upButtonMask 	= 0x040;
	int downButtonMask 	= 0x080;
	int fireButtonMask 	= 0x400;
	//accelerometer
	public int x,y,z;
	public float fx,fy,fz;
	//sliders & valves
	public int slider1,slider2,valve1,valve2;
	public float s1,s2,v1,v2;
	
	public bool leftDown;
	public bool rightDown;
	public float leftDownLast;
	public float rightDownLast;
	public float jumpLast;
	public static float buttonTime = 1/10.0f;
	
	public IO(string port){
		stream = new SerialPort(port);
		stream.Open(); //Open the Serial Stream.	
		Debug.Log("Serial Port "+port+" opened.");	
	}
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	public void Update () {
		//buttons
		stream.Write("1");
		receivedData = stream.ReadLine();
		buttonVal = System.Convert.ToInt32(receivedData,16);
		//accelerometer
		stream.Write("a");
		receivedData = stream.ReadLine();
		string[] data = receivedData.Split(' ');
		
		x = convertInt(System.Convert.ToInt32(data[1],16));
		y = convertInt(System.Convert.ToInt32(data[2],16));
		z = convertInt(System.Convert.ToInt32(data[3],16));
		
		//normalize and convert to float
		fx = interpolate(x,-128,127,-1,1);
		fy = interpolate(y,-128,127,-1,1);
		fz = interpolate(z,-128,127,-1,1);
		//sliders & valves
		stream.Write("4");
		receivedData = stream.ReadLine();
		//A: 0000 0000 0000 0000
		//A: dreh dreh slid1 slid2
		data = receivedData.Split(' ');
		slider1 = System.Convert.ToInt32(data[3],16);
		slider2 = System.Convert.ToInt32(data[4],16);
		valve1 = System.Convert.ToInt32(data[1],16);
		valve2 = System.Convert.ToInt32(data[2],16);
		//convert to [0,1]
		float b = 4095;
		s1 = slider1/b;
		s2 = slider2/b;
		v1 = valve1/b;
		v2 = valve2/b;
		
		player.accel = interpolate(s1,0,1,-0.5f,1.5f);
		
		//input evens to player
		if ((buttonVal & leftButtonMask) != 0) {
			if(!leftDown && leftDownLast+buttonTime < Time.time) {
				leftDownLast = Time.time;
				player.inputEvents.AddLast(new InputEvent(InputEvent.Axis.Direction, -1));
				Debug.Log("left.");
			}
			leftDown = true;
		} else {
			leftDown = false;
		}
		if((buttonVal & rightButtonMask) != 0) {
			if(!rightDown && leftDownLast+buttonTime < Time.time) {
				rightDownLast = Time.time;
				player.inputEvents.AddLast(new InputEvent(InputEvent.Axis.Direction, 1));
				Debug.Log("right.");
			}
			rightDown = true;
		} else {
			rightDown = false;
		}
		if(fz <= -0.5f && jumpLast + buttonTime < Time.time) {
			jumpLast = Time.time;
			player.inputEvents.AddLast(new InputEvent(InputEvent.Axis.Powerup, 1));
			Debug.Log("jump");
		}
		
	}
	
	int convertInt(int i) {
		if(i > 127)
			return (i-256);
		return i;
	}
	
	float interpolate(float value, float froma, float fromb, float toa, float tob) {
		return 1 - (value-froma)*(tob-toa)/(fromb-froma);
	}
}
