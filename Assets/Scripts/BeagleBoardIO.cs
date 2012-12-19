using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;

public class BeagleBoardIO {
	SerialPort stream = new SerialPort("COM3");
	public string receivedData = "EMPTY";
	static BeagleBoardIO instance;
	
	//DATA
	//buttons
	int buttonVal;
	//accelerometer
	public int x,y,z;
	public float fx,fy,fz;
	
	private BeagleBoardIO(){
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
	}
	int convertInt(int i) {
		if(i > 127)
			return (i-256);
		return i;
	}
	
	float interpolate(int value,int froma,int fromb, int toa,int tob) {
		return 1 - (value-froma)*(tob-toa)/(float)(fromb-froma);
	}
	//singleton
	public static BeagleBoardIO getInstance() {
		if(instance == null) {
			instance = new BeagleBoardIO();
		} 
		return instance;
	}
}
