using UnityEngine;
using System.Collections;
using System.IO;

public class TrainingScript : MonoBehaviour
{

    #region Fields
    /// <summary>
    /// Stop recording after getting within this target's range.
    /// </summary>
    public float DistanceThreshold = 0.1f;
    public bool CreateNewFile = true;
    public bool AppendToExisting = false;
	public int trainingTrials = 1;
	private int count;
    // public int RecordIntervalMillis = 300;
    /// <summary>
    /// Number of updates in between logging to training file.
    /// </summary>
    public int RecordEach = 5;

    // Input values
    private float moving = 0.0f;
    private float rotation;

    // Sensors
	private float frontSensor;
	private float rightSensor;
	private float leftSensor;
    private float leftDirectSensor;
    private float rightDirectSensor;
    private float agentDist;
	private float agentAngle;

	private StreamWriter writer;
    private bool recordData;
    // private float timeElapsed = 0f;
    private int updatesElapsed = 0;
    #endregion

    // Use this for initialization
	void Start () {
        int i = 0;
		count = 0;

        this.recordData = true;
        
        if (this.CreateNewFile)
            while (File.Exists(string.Format("..\\sensorData_{0}.txt", i)))
                i++;

		this.writer = new StreamWriter(string.Format("..\\sensorData_{0}.txt", i), this.AppendToExisting);
        Debug.Log("Training started. Logging to file @ " + string.Format("..\\sensorData_{0}.txt", i));
	}
	
	// Update is called once per frame
	void Update () {
		if (recordData) {

            // Did we find the target/*
          /*  if (this.agentDist <= this.DistanceThreshold)
            {
                this.recordData = false;
                if (this.writer != null)
                    this.writer.Close();
                Debug.Log("Training run complete! Target found.");
                return;
            } */

            // Track our time
            // this.timeElapsed += Time.deltaTime * 1000;
            this.updatesElapsed++;

            // Time to record?
            //if (timeElapsed >= this.RecordIntervalMillis)
            if (this.updatesElapsed >= this.RecordEach)
            {
			    printTrainingData ();
                // this.timeElapsed = 0f;
                this.updatesElapsed = 0;
            }
		}
	}

	void OnTriggerEnter2D(Collider2D coll) {
		if (coll.gameObject.name.Contains ("Bunny")) {
			count++;

			if (count == trainingTrials) {
				this.recordData = false;
				if (this.writer != null)
					this.writer.Close ();
				Debug.Log ("Training run complete! Target found.");
				return;
			}
		}
	}

	private void printTrainingData() {
        // Normalize
	    this.writer.WriteLine (
            string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8}",
            leftSensor.ToString("F6"),
            frontSensor.ToString("F6"),
            rightSensor.ToString("F6"),
            rightDirectSensor.ToString("F6"),
            leftDirectSensor.ToString("F6"),
            agentDist.ToString("F6"),
            agentAngle.ToString("F3"),
            moving.ToString("F1"),
            rotation.ToString("F1")));
	}

    #region Setters
    public void setSensors(float right, float left, float fwd, float rightDirect, float leftDirect)
    {
        this.frontSensor = fwd;
        this.rightSensor = right;
        this.leftSensor = left;
        this.rightDirectSensor = rightDirect;
        this.leftDirectSensor = leftDirect;
    }

    public void setRotation(float rot)
    {
        this.rotation = rot;
    }

    public void setIsMoving(bool isMoving)
    {
        this.moving = isMoving ? 1f : 0f;
    }

    public void setAgentDistance(float distance)
    {
        this.agentDist = distance;
    }

    public void setAgentAngle(float angle)
    {
        this.agentAngle = angle;
    }

    #endregion
}
