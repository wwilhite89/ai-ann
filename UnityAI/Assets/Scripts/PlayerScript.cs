using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

	public float speed;
	public float rotationSpeed;
	public GameObject trainingObject;
    private TrainingScript script;


	void FixedUpdate()
	{
        if (script == null)
            script = trainingObject.GetComponent<TrainingScript>();

		// movement and rotation
		float translation = Input.GetAxis("Vertical") * speed;
		if (translation != 0) {
			setTrainingMove(true);
		} else {
			setTrainingMove(false);
		}
		float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
		if (rotation > 0) {
			setTrainingRot (1.0f);
		} 
		else if (rotation < 0) {
			setTrainingRot (0.0f);
		} 
		else if (rotation == 0) {
			setTrainingRot(0.5f);
		}
		translation *= Time.deltaTime/5;
		transform.Translate(0, translation, 0);
		transform.Rotate(0, 0, -rotation);


	}
	private void setTrainingMove(bool moving) {

		this.script.setIsMoving(moving);

	}
	private void setTrainingRot(float rot) {
		
		this.script.setRotation(rot);
		
	}
}