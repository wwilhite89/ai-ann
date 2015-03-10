using UnityEngine;
using System.Collections;

public class CreateAgent : MonoBehaviour {

	public float minSpawnTime = 0.75f;
	public float maxSpawnTime = 2f;
	public GameObject Agent;

	private Vector3 agentLocation;

	// Use this for initialization
	void Start () {
		Invoke ("SpawnAgent", minSpawnTime);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void SpawnAgent()
	{
		// cameras current position and bounds
		Camera camera = Camera.main;
		Vector3 cameraPos = camera.transform.position;
		
		float xMax = camera.aspect * camera.orthographicSize;
		float xRange = camera.aspect * camera.orthographicSize * 1.75f;
		float yMax = camera.orthographicSize - 0.5f;
		
		// create a new position at random location at same z position
		Vector3 catPos = new Vector3 (cameraPos.x + 
		                              Random.Range (xMax - xRange, xMax),
		                              Random.Range (-yMax, yMax),
		                              Agent.transform.position.z);
		
		// create an instance of the prefab at location catPos
		Instantiate (Agent, catPos, Quaternion.identity);
		Invoke ("SpawnAgent", Random.Range (minSpawnTime, maxSpawnTime));
	}
}
