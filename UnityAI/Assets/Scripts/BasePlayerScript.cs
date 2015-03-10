using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BasePlayerScript : MonoBehaviour {

    public float speed;
    public float rotationSpeed;

    // Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    void FixedUpdate() { 

		// movement and rotation
		float translation = Input.GetAxis("Vertical") * speed;
		float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
		translation *= Time.deltaTime/5;
		transform.Translate(0, translation, 0);
		transform.Rotate(0, 0, -rotation);
    }

    public Vector3 GetPosition(out float rotationAngle) {
        rotationAngle = transform.eulerAngles.z;
        return transform.position;
    }

}