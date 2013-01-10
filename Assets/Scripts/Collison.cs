using UnityEngine;
using System.Collections;

public class Collison : MonoBehaviour {
	public GameObject player;

	void OnTriggerEnter(Collider otherObject) 
	{	
		if(otherObject.tag == "wall" || otherObject.tag == "player" || otherObject.tag == "tail") {
			player.SendMessage("Collide",otherObject);			
		}
	}
}
