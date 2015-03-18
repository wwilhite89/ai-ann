using UnityEngine;
using System.Collections;
using System.IO;

public class TrainingScript : MonoBehaviour {

	public float frontSensor;
	public float rightSensor;
	public float leftSensor;
	public float agentDist;
	public float agentAngle;
	public int [] actLevels;
	private StreamWriter writer;

	// Use this for initialization
	void Start () {
		writer = new StreamWriter("C:/Users/Adam/Documents/UCF/AI for game programming/trainingData.txt");
	}
	
	// Update is called once per frame
	void Update () {
		printTrainingData ();
	}

	void FixedUpdate () {
		// call this each update to print the training data
		//printTrainingData ();
	}

	void printTrainingData() {
		writer.WriteLine ("right sensor " + rightSensor.ToString ());

	}
}
