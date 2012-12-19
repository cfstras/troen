using UnityEngine;
using System.Collections;

public class CreateWorld : MonoBehaviour {
	public GameObject planeFloor, planeLeft, planeRight, planeFront, planeBack;
	public GameObject prefabPlane;
	// Use this for initialization
	void Start () {
		createArena();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void createArena() {
		planeFloor  = (GameObject) Instantiate(prefabPlane);
		planeLeft 	= (GameObject) Instantiate(prefabPlane);
		planeRight 	= (GameObject) Instantiate(prefabPlane);
		planeFront 	= (GameObject) Instantiate(prefabPlane);
		planeBack  	= (GameObject) Instantiate(prefabPlane);
		
		planeBack.transform.rotation = Quaternion.Euler(90,0,0);
		
	}
}
