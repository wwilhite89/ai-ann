using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

	public float speed;
	public float rotationSpeed;

	void FixedUpdate()
	{
		
		// movement and rotation
		float translation = Input.GetAxis("Vertical") * speed;
		float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
		translation *= Time.deltaTime/5;
		transform.Translate(0, translation, 0);
		transform.Rotate(0, 0, -rotation);

	}
	
}