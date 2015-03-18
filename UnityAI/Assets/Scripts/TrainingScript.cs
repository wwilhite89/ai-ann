using UnityEngine;
using System.Collections;

public class TrainingScript : MonoBehaviour {

	public float frontSensor;
	public float rightSensor;
	public float leftSensor;
	public float agentDist;
	public float agentAngle;
	public int [] actLevels;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		// call this each update to print the training data
		printTrainingData ();
	}

	void printTrainingData() {

			using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"..\sensorData.txt"))
			{
				file.WriteLine(leftSensor);
				file.WriteLine(frontSensor);
				file.WriteLine(rightSensor);
			}

	}
}
