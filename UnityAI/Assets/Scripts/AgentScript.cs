using UnityEngine;
using System.Collections;

public class AgentScript : MonoBehaviour {

	public GameObject player;
	public GameObject deadBunny;
	public GameObject bunny;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter2D(Collider2D coll) {
		if (coll.gameObject.name.Contains("Mower")) {
			FindLocation();
			
			Destroy(this.gameObject);
		}

		if (coll.gameObject.name.Contains ("Long Bush")) {
			FindLocation ();			
			Destroy (this.gameObject);
		}
	}

	void FindLocation()
	{
		// cameras current position and bounds
		Camera camera = Camera.main;
		Vector3 cameraPos = camera.transform.position;
		
		float xMax = camera.aspect * camera.orthographicSize;
		float xRange = camera.aspect * camera.orthographicSize * 1.75f;
		float yMax = camera.orthographicSize - 0.5f;
		
		// create a new position at random location at same z position
		Vector3 agentPosition = new Vector3 (cameraPos.x + 
		                                     Random.Range (xMax - xRange, xMax),
		                                     Random.Range (-yMax, yMax),
		                                     bunny.transform.position.z);

		// create an instance of the prefab at location catPos
		Instantiate (bunny, agentPosition, Quaternion.identity);
	}

}
