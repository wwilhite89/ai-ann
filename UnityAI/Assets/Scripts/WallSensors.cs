using UnityEngine;
using System.Collections;

public class WallSensors : MonoBehaviour {

	// variables
	public float playerOffset;
	public float sensorLength;
	RaycastHit2D hitFront;
	RaycastHit2D hitRight;
	RaycastHit2D hitLeft;
    RaycastHit2D hitDirectRight;
    RaycastHit2D hitDirectLeft;
    private float rightWallDist;
    private float leftWallDist;
    private float fwdWallDist;
    private float rightDirectWallDist;
    private float leftDirectWallDist;
	public GameObject trainingObject;
    private TrainingScript trainer;
	
	// Use this for initialization
	void Start () {
		rightWallDist = sensorLength;
		leftWallDist = sensorLength;
		fwdWallDist = sensorLength;
        rightDirectWallDist = sensorLength;
        leftDirectWallDist = sensorLength;
	}
	
	// Update is called once per frame
	void Update () {
        if (this.trainingObject != null && this.trainer == null)
            this.trainer = trainingObject.GetComponent<TrainingScript>();
		updateSensors ();
	}

	private void updateSensors() {
	
		// vectors to hold sensor right offset and left offset
		Vector2 fwdRight = transform.position;
		Vector2 fwdLeft = transform.position;
		
		// set the sensor offsets
		fwdRight.Set (this.transform.up.x / 2  + this.transform.right.x / 2, this.transform.up.y / 2 + this.transform.right.y / 2);
		fwdLeft.Set (this.transform.up.x / 2 + -this.transform.right.x / 2, this.transform.up.y / 2 + -this.transform.right.y / 2);
		
		
		// Debug rays drawn in scene view  ***********  May want to remove  ************
		Debug.DrawRay (transform.position, this.transform.up * sensorLength, Color.cyan);
		Debug.DrawRay (transform.position, fwdRight * sensorLength, Color.cyan);
		Debug.DrawRay (transform.position, fwdLeft * sensorLength, Color.cyan);
        Debug.DrawRay (transform.position, this.transform.right * sensorLength, Color.cyan);
        Debug.DrawRay (transform.position, -this.transform.right * sensorLength, Color.cyan);
		
		// three raycasts fwd, left and right.
		// theses rays will only sense objects in layer 8, that is where the walls live
		hitFront = Physics2D.Raycast (transform.position, this.transform.up, sensorLength, 1 << 8);
		hitRight = Physics2D.Raycast (transform.position, fwdRight, sensorLength, 1 << 8);
		hitLeft = Physics2D.Raycast (transform.position, fwdLeft, sensorLength, 1 << 8);
        hitDirectRight = Physics2D.Raycast(transform.position, transform.right, sensorLength, 1 << 8);
        hitDirectLeft = Physics2D.Raycast(transform.position, -transform.right, sensorLength, 1 << 8);
		
		// for accuracy and readability, player offset is the radious of its collider
		hitFront.distance -= playerOffset;
		hitLeft.distance -= playerOffset;
		hitRight.distance -= playerOffset;
        hitDirectRight.distance -= playerOffset;
        hitDirectLeft.distance -= playerOffset;

		// return the sensor length if the sensor does not sense a wall/ other collider
		if (hitFront.collider == null)
			hitFront.distance = this.sensorLength;
		if (hitLeft.collider == null)
			hitLeft.distance = this.sensorLength;
		if (hitRight.collider == null) {
			hitRight.distance = this.sensorLength;
		}
        if (hitDirectLeft.collider == null)
        {
            hitDirectLeft.distance = this.sensorLength;
        }
        if (hitDirectRight.collider == null)
        {
            hitDirectRight.distance = this.sensorLength;
        }

        if (this.trainer != null)
            this.trainer.setSensors(
                hitRight.distance / this.sensorLength, 
                hitLeft.distance / this.sensorLength,
                hitFront.distance / this.sensorLength,
                hitDirectRight.distance / this.sensorLength,
                hitDirectLeft.distance / this.sensorLength);
		
		// format the distances 
        this.rightWallDist = hitRight.distance;
		this.leftWallDist = hitLeft.distance;
		this.fwdWallDist = hitFront.distance;
        this.leftDirectWallDist = hitDirectLeft.distance;
        this.rightDirectWallDist = hitDirectRight.distance;
		// print the distances found to the console
		/*Debug.Log ("FrontSensor " + rightDist + " " +
		           "RightSensor " + leftDist +  " " +
		           "LeftSensor " + fwdDist); */
	}

    public float GetRightWall(bool normalized)
    {
        return !normalized ? this.rightWallDist:
            this.rightWallDist / this.sensorLength;
    }

    public float GetDirectRightWall(bool normalized)
    {
        return !normalized ? this.rightDirectWallDist :
            this.rightDirectWallDist / this.sensorLength;
    }

    public float GetLeftWall(bool normalized)
    {
        return !normalized ? this.leftWallDist:
            this.leftWallDist / this.sensorLength;
    }

    public float GetDirectLeftWall(bool normalized)
    {
        return !normalized ? this.leftDirectWallDist :
            this.leftDirectWallDist / this.sensorLength;
    }

    public float GetFwdWall(bool normalized)
    {
        return !normalized ? this.fwdWallDist:
            this.fwdWallDist / this.sensorLength;
    }

	// print the distances of each sensor to the game screen
	/*void OnGUI() {
		
		GUI.Label (new Rect (10,10,150,20), "Left Wall Sensor: " + leftDist);
		GUI.Label (new Rect (10,25,150,20), "Front Wall Sensor: " + fwdDist);
		GUI.Label (new Rect (10,40,150,20), "Right Wall Sensor: " + rightDist);
		
	}*/

}
