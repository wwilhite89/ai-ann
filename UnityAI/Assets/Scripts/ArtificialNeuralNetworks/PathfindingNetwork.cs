﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.ArtificialNeuralNetworks.Core;
using System.IO;
using System.Threading;

public class PathfindingNetwork : MonoBehaviour
{

    #region Public Fields
    public bool LoadTrainingFiles = false;
    public bool VerboseLogging = true;
    public double LearnRate = 0.2;
    public double Momentum = 1.0;
    public int MaxTrainingEpochs = 10000;
    public double ErrorThreshold = 0.25;
    public KeyCode PrintWeightsKey = KeyCode.W;
    public enum Rotate
    { 
        Left = -1,
        Right = 1,
        None = 0,
    }
    #endregion

    #region Private Fields
    private string trainingFileFormat = "..\\sensorData_{0}.txt";
    private BackPropNeuralNet bnn;
    #endregion

    // Use this for initialization
	void Start () {
        double[] weights = new double[]
        {
        //-91.45560, -9.55997, -88.75825, -7.69307, 40.79388, -92.81184, -15.15363, -1.62691, -78.89581, -8.32583, 5.19362, 4.43045, 20.27612, 13.03869, -6.95389, -1.48190, 27.30377, -8.15084, 1.73598, 6.73617, -63.33648, 48.06354, -2.30955, 0.86448, 4.73056, 5.91697, 1.31404, 3.70318, 27.65855, 12.02901, 94.04744, 1.96860, 68.66748, 1.55772, -3.94027, 28.45790, 54.58911, -1.89976, -53.66986, 3.57036, 51.70758, -2.98710
			-97.34611, -3.90911, -149.04366, 2.78079, 49.12497, -138.73610, -93.60766, -0.39073, -93.84984, -16.60178, 10.71182, 15.86199, 28.46233, 23.76207, -8.87522, 3.36728, 1.31377, -6.84122, -4.08323, 24.93612, -128.52738, 33.37120, 123.29416, 15.08536, 27.90609, -0.73410, 122.43132, 38.34337, 36.38239, 22.75239, 121.95462, -48.13423, 69.72272, 1.22996, 1.42393, 37.02866, 3.32031, 0.85693, -68.54849, 1.51724, 69.52844, -0.60007
			// tial 1 ( ok results )-1.04231, 2.05177, 2.26751, -1.42857, -0.67405, 1.82190, 1.54200, 1.23433, 2.91517, 2.63713, 2.16703, 0.23292, 8.63558, -1.90439, 10.52001, -5.70827, 1.48727, -5.07518, -0.01214, -5.26346, -0.15831, 0.47426, 2.98813, -0.00756, -0.54269, -1.06489, 19.78878, 0.78513, 5.34836, -0.71407, 7.38371, 1.40219, -2.03905, 0.19372
			//-3.73566, 30.75219, 2.26751, -22.80343, -3.36740, 23.68950, 1.54200, 6.79838, 16.77281, 2.90665, 2.16703, 5.85691, 20.39941, 8.96655, 10.52001, -36.92163, 5.58798, -20.00860, -0.01214, -13.67515, -0.42764, 3.17704, 2.98813, 0.54885, -0.59417, -0.17844, 15.88109, -0.98014, 5.29688, 0.10749, 7.38371, 6.56844, -2.09053, 1.01528, 
           // 0.67216, -9.68895, -14.69239, 3.85634, -5.75940, 21.02394, 31.82843, -5.03168, -12.54429, -2.97053, 82.39918, 20.69674, -21.83945, -31.98080, 9.72366, -22.67755, 6.92105, -3.17255, -30.57761, 51.01984, 9.38309, -8.59458, -3.67414, -14.00542, -52.04559, 12.82188, 24.61726, 12.76068, 40.67682, 15.22335, 37.11664, 12.41045, 15.79968, -27.86470
           // Jason test results
           // 1.57129, -4.27995, -3.36299, 17.75179, 1.06521, -82.70220, 6.06337, -55.51868, 3.81671, 11.64703, 5.43716, -4.00157, 16.61993, 37.70362, 22.09976, -50.99300, 72.49254, 23.71892, 8.03141, 20.97513, -6.12398, -8.00372, -15.67902, -1.07754, -88.28119, -3.56209, -51.61088, 6.44667, 6.51731, 3.60930, 43.53153, 0.15261, 84.67302, 0.02948
           // 11.53091, 0.34143, -9.17652, -14.99076, 7.04148, -84.10723, 4.38802, -41.36451, 29.03649, 3.43519, -2.36067, 4.75183, 8.99726, 51.28721, 16.83561, -59.23322, 112.72394, 16.85909, 58.09328, 52.68874, -54.52602, -4.16884, -19.44719, -5.20606, -78.39688, -33.70512, -44.82414, 14.99464, -10.19908, 39.18803, 51.65474, -3.76920, 81.88299, 0.38024
        };

        this.createNetwork();
        this.bnn.SetWeights(weights);

         // Train First
        if (this.LoadTrainingFiles)
            ThreadPool.QueueUserWorkItem(x => 
            { 
                this.trainNetwork(); 
            });
                
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(this.PrintWeightsKey))
        {
            Debug.Log("Current weights:");
            Helpers.ShowVector(this.bnn.GetWeights(), 5, 50, false);
        }
	}

    #region Public Methods

    /// <summary>
    /// Based on training determines if the agent should move forward.
    /// </summary>
    /// <param name="leftWall"></param>
    /// <param name="centerWall"></param>
    /// <param name="rightWall"></param>
    /// <param name="distance"></param>
    /// <param name="relativeAngle"></param>
    /// <param name="rotation">Whether the agent should rotate.</param>
    /// <returns>Whether the agent should move or not.</returns>
    public bool GetNextMovement(float leftWall, float centerWall, float rightWall, float rightDirectWall, float leftDirectWall,
        float distance, float relativeAngle, out Rotate rotate)
    {
        if (this.bnn != null)
        {
            var movementThreshold = .95f;
            var vals = this.bnn.ComputeOutputs(new double[] { leftWall, centerWall, rightWall, rightDirectWall, leftDirectWall, distance, relativeAngle });
            
            rotate = vals[1] < 0.4 ? Rotate.Left
                : vals[1] > 0.66 ? Rotate.Right
                    : Rotate.None;
            return vals[0] >= movementThreshold;
        }

        // Wait until the network is ready
        rotate = Rotate.None;
        return false;
    }

    #endregion

    #region Private Methods
    private void createNetwork()
    {
        this.bnn = new BackPropNeuralNet(7, 4, 2, true)
        {
            LearnRate = this.LearnRate,
            Momentum = this.Momentum,
            MaxEpochs = this.MaxTrainingEpochs,
            ErrorThreshold = this.ErrorThreshold,
            HiddenLayerActivation = BackPropNeuralNet.ActivationMethod.Sigmoid,
            OutputLayerActivation = BackPropNeuralNet.ActivationMethod.Sigmoid,
        };

        if (this.VerboseLogging)
            Debug.Log(string.Format("Pathfinding {0}-{1}-{2} neural network created.",
                bnn.InputNodeCount,
                bnn.HiddenNodeCount,
                bnn.OutputNodeCount));
    }

    private void trainNetwork()
    {
        int totalFiles = 0, linesProcessed = 0;

        if (this.VerboseLogging)
            Debug.Log("Training network from files.");

        while (File.Exists(string.Format(this.trainingFileFormat, totalFiles)))
        {
            var fileName = string.Format(this.trainingFileFormat, totalFiles);
            var count = File.ReadAllLines(fileName).Length;
            var input = new double[count][];
            var output = new double[count][];
            
            if (this.VerboseLogging)
                Debug.Log(string.Format("Reading training file '{0}'", fileName));

            using (var reader = new StreamReader(fileName))
            {
                linesProcessed = 0;

                // Read data
                while (linesProcessed < count)
                {
                    input[linesProcessed] = new double[7];
                    output[linesProcessed] = new double[2];

                    var values = reader.ReadLine().Split(' ');
                    // Debug.Log(values);
                    double.TryParse(values[0], out input[linesProcessed][0]);
                    double.TryParse(values[1], out input[linesProcessed][1]);
                    double.TryParse(values[2], out input[linesProcessed][2]);
                    double.TryParse(values[3], out input[linesProcessed][3]);
                    double.TryParse(values[4], out input[linesProcessed][4]);
                    double.TryParse(values[5], out input[linesProcessed][5]);
                    double.TryParse(values[6], out input[linesProcessed][6]);
                    double.TryParse(values[7], out output[linesProcessed][0]);
                    double.TryParse(values[8], out output[linesProcessed][1]);
                    linesProcessed++; // Next line
                }

                if (this.VerboseLogging)
                    Debug.Log(string.Format("Computing values for training file '{0}'", fileName));

                // Train
                this.bnn.Train(input, output, this.VerboseLogging);
            }

            if (this.VerboseLogging)
                Debug.Log(string.Format("Training file '{0}' loaded into network. {1} lines processed.", fileName, count));

            totalFiles++; // Get next file
        }


        if (this.VerboseLogging)
        {
            Debug.Log(string.Format("Neural network trained on {0} file(s).", totalFiles));
            Debug.Log("Final weights after training.");
            Helpers.ShowVector(this.bnn.GetWeights(), 5, 50, false);
        }
    }
    #endregion

}