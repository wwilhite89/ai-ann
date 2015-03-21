﻿using UnityEngine;
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
	private float timer;
	private int i;
	private int line;
	public float moving=0.0f;
	public float rotation;
	private bool record=true;

	// Use this for initialization
	void Start () {
		i = 0;
		line = 0;
		writer = new StreamWriter(@"..\sensorData.txt");
		timer=10.0f;

	}
	
	// Update is called once per frame
	void Update () {
		if (record) {
			printTrainingData ();
			if (timer > 0) {
				timer -= Time.deltaTime;
			}
			if (timer <= 0) {
				writer.Close ();
				record=false;
				Debug.Log("done "+i);
			}
		}
	}

	void FixedUpdate () {
		// call this each update to print the training data
		//printTrainingData ();
	}

	void printTrainingData() {
		if ((i % 5) == 0) {
			writer.WriteLine (leftSensor.ToString("F1")+" "+frontSensor.ToString("F1")+" " + rightSensor.ToString("F1")+" "+agentDist.ToString("F1")+" "+agentAngle+" "+moving.ToString("F1")+" "+rotation.ToString("F1"));
			line++;
		}
		i++;

//			using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"..\sensorData.txt"))
//			{
//				file.WriteLine(leftSensor);
//				file.WriteLine(frontSensor);
//				file.WriteLine(rightSensor);
//			}

	}
}
