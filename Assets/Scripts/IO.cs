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
	
	public IO(string port){
		stream = new SerialPort(port);
		stream.Open(); //Open the Serial Stream.	
		Debug.Log("Serial Port opened.");	
	}
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
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
		//input evens to player
		if ((buttonVal & leftButtonMask) != 0) {
			player.inputEvents.AddLast(new InputEvent(InputEvent.Axis.Direction, -1));
		} 
		if((buttonVal & rightButtonMask) != 0) {
			player.inputEvents.AddLast(new InputEvent(InputEvent.Axis.Direction, 1));
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
