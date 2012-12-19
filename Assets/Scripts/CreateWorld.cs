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
		
		planeFloor.name = "planeFloor";
		planeLeft.name = "planeLeft";
		planeRight.name = "planeRight";
		planeFront.name = "planeFront";
		planeBack.name = "planeBack";
		
		planeBack.transform.rotation = Quaternion.Euler(90,0,0);
		planeFront.transform.rotation = Quaternion.Euler(-90,0,0);
		planeLeft.transform.rotation = Quaternion.Euler(-90,0,90);
		planeRight.transform.rotation = Quaternion.Euler(90,0,-90);
		
		planeBack.transform.position = new Vector3(0,0,-5f);
		planeFront.transform.position = new Vector3(0,0,5f);
		planeLeft.transform.position = new Vector3(5,0,0);
		planeRight.transform.position = new Vector3(-5,0,0);
		planeFloor.transform.position = new Vector3(0,-5,0);
	}
}
