using UnityEngine;
using System.Collections;

public class Collison : MonoBehaviour {
	public GameObject player;

	void OnTriggerEnter(Collider otherObject) 
	{	if(otherObject.tag == "collider")
		{
			player.SendMessage("Collide",otherObject);	
		}
	}
}
