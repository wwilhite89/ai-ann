using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.ArtificialNeuralNetworks.Core;
using System.IO;

public class PathfindingNetwork : MonoBehaviour
{

    #region Public Fields
    public bool LoadTrainingFiles = false;
    public bool VerboseLogging = true;
    public double LearnRate = 0.2;
    public double Momentum = 1.0;
    public int MaxTrainingEpochs = 10000;
    public double ErrorThreshold = 0.25;
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
			///* tial 1 ( ok results )*/-1.04231, 2.05177, 2.26751, -1.42857, -0.67405, 1.82190, 1.54200, 1.23433, 2.91517, 2.63713, 2.16703, 0.23292, 8.63558, -1.90439, 10.52001, -5.70827, 1.48727, -5.07518, -0.01214, -5.26346, -0.15831, 0.47426, 2.98813, -0.00756, -0.54269, -1.06489, 19.78878, 0.78513, 5.34836, -0.71407, 7.38371, 1.40219, -2.03905, 0.19372
			-3.73566, 30.75219, 2.26751, -22.80343, -3.36740, 23.68950, 1.54200, 6.79838, 16.77281, 2.90665, 2.16703, 5.85691, 20.39941, 8.96655, 10.52001, -36.92163, 5.58798, -20.00860, -0.01214, -13.67515, -0.42764, 3.17704, 2.98813, 0.54885, -0.59417, -0.17844, 15.88109, -0.98014, 5.29688, 0.10749, 7.38371, 6.56844, -2.09053, 1.01528, 

		};


        this.createNetwork();
        this.bnn.SetWeights(weights);

            // Train First
            if (this.LoadTrainingFiles)
                this.trainNetwork();
                
	}
	
	// Update is called once per frame
	void Update () {
	
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
    public bool GetNextMovement(float leftWall, float centerWall, float rightWall, float distance, float relativeAngle, out Rotate rotate)
    {
        if (this.bnn != null)
        {
            var movementThreshold = .96f;
            var vals = this.bnn.ComputeOutputs(new double[] { leftWall, centerWall, rightWall, distance, relativeAngle });
            
            if (this.VerboseLogging)
                Debug.Log(string.Format("[{0}][{1}]", vals[0], vals[1]));

            rotate = vals[1] < 0.3 ? Rotate.Left
                : vals[1] > 0.7 ? Rotate.Right
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
        this.bnn = new BackPropNeuralNet(5, 4, 2, true)
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
        int totalFiles = 0, filesProcessed = 0;

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
                filesProcessed = 0;

                // Read data
                while (filesProcessed < count)
                {
                    input[filesProcessed] = new double[5];
                    output[filesProcessed] = new double[2];

                    var values = reader.ReadLine().Split(' ');
                    // Debug.Log(values);
                    double.TryParse(values[0], out input[filesProcessed][0]);
                    double.TryParse(values[1], out input[filesProcessed][1]);
                    double.TryParse(values[2], out input[filesProcessed][2]);
                    double.TryParse(values[3], out input[filesProcessed][3]);
                    double.TryParse(values[4], out input[filesProcessed][4]);
                    double.TryParse(values[5], out output[filesProcessed][0]);
                    double.TryParse(values[6], out output[filesProcessed][1]);
                    filesProcessed++; // Next line
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