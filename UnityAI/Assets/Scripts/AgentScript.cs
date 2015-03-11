using UnityEngine;
using System.Collections;

public class AgentScript : MonoBehaviour {

	public GameObject player;
	public GameObject deadBunny;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerExit2D(Collider2D coll) {
		if (coll.gameObject.name.Contains("Mower")) {
			Destroy(this.gameObject);
			Instantiate(deadBunny, transform.position, transform.rotation);
		}
	}
}
