using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

	public float speed;
	public float rotationSpeed;
	public GameObject trainingObject;



	void FixedUpdate()
	{

		// movement and rotation
		float translation = Input.GetAxis("Vertical") * speed;
		if (translation != 0) {
			setTrainingMove(true);
		} else {
			setTrainingMove(false);
		}
		float rotation = Input.GetAxis("Horizontal") * rotationSpeed;

		translation *= Time.deltaTime/5;
		transform.Translate(0, translation, 0);
		transform.Rotate(0, 0, -rotation);
		setTrainingRot (transform.rotation.eulerAngles.z.ToString());

	}
	private void setTrainingMove(bool moving) {

		trainingObject.GetComponent<TrainingScript>().moving = moving;

	}
	private void setTrainingRot(string rot) {
		
		trainingObject.GetComponent<TrainingScript>().rotation = rot;
		
	}
}