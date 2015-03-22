using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

	public float speed;
	public float rotationSpeed;
    public bool AiControlled = false;
	public GameObject trainingObject;
    private TrainingScript trainer;
    private PathfindingNetwork ai;
    private WallSensors wallSensors;
    private AdjacentAgentSensor adjSensors;

	void FixedUpdate()
    {
        #region Get scripts

        if (trainingObject && trainer == null)
            trainer = trainingObject.GetComponent<TrainingScript>();

        // Load AI Components
        if (AiControlled)
        {
            if (this.ai == null)
                ai = this.GetComponent<PathfindingNetwork>();
            if (this.wallSensors == null)
                this.wallSensors = this.GetComponent<WallSensors>();
            if (this.adjSensors == null)
                this.adjSensors = this.GetComponent<AdjacentAgentSensor>();
        }

        #endregion

        // movement and rotation
		float translation, rotation;

        if (!this.AiControlled || ai == null)
        {
            translation = Input.GetAxis("Vertical") * speed;
            rotation = Input.GetAxis("Horizontal") * rotationSpeed;
        }
        else
        {
            PathfindingNetwork.Rotate rotate;
            var movement = ai.GetNextMovement(
                wallSensors.GetLeftWall(), 
                wallSensors.GetFwdWall(), 
                wallSensors.GetRightWall(), 
                adjSensors.GetAgentDistances()[0], // Only care about the closest agent
                adjSensors.GetAgentRelativeAngles()[0], // Only care about the closest agent
                out rotate);

            translation = movement ? speed : 0;
            Debug.Log(string.Format("AI decided to {0}move to the {1}", 
                !movement ? "not " : "", 
                (rotate == PathfindingNetwork.Rotate.None ? "Center" : rotate.ToString()).ToLower()));
            rotation = (float)rotate * rotationSpeed;
        }

        if (trainer != null)
        {
            setTrainingMove(translation != 0);

            if (rotation > 0)
            {
                setTrainingRot(1.0f);
            }
            else if (rotation < 0)
            {
                setTrainingRot(0.0f);
            }
            else if (rotation == 0)
            {
                setTrainingRot(0.5f);
            }
        }
		translation *= Time.deltaTime/5;
		transform.Translate(0, translation, 0);
		transform.Rotate(0, 0, -rotation);


	}

	private void setTrainingMove(bool moving) {

        this.trainer.setIsMoving(moving);

	}
	private void setTrainingRot(float rot) {

        this.trainer.setRotation(rot);
		
	}
}