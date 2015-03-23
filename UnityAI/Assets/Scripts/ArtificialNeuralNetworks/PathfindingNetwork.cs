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
			-0.00022, -0.09342, 1.81210, -0.03253, 0.05824, -0.01413, 0.89718, 0.03252, 0.03007, -0.05662, 1.54228, 0.00393, 0.03873, -0.53799, 9.94697, -0.22154, -1.00195, -1.19059, -4.20571, -0.71062, 0.02685, -0.08348, 2.82695, -0.03127, 7.41734, 0.59613, -0.55521, 0.65568, 1.83933, -1.19487, -0.98381, 0.81873, -5.70799, 1.38290
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
            var movementThreshold = 0.9f;
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