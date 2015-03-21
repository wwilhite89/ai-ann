using UnityEngine;
using System.Collections;
using Assets.Scripts.ArtificialNeuralNetworks.Core;

public class PathfindingNetwork : MonoBehaviour
{

    #region Public Fields
    public bool LoadTrainingFiles = false;
    public bool VerboseLogging = true;
    public double LearnRate = 0.3;
    public double Momentum = 1.0;
    public int MaxTrainingEpochs = 10000;
    public double ErrorThreshold = 0.1;
    #endregion

    #region Private Fields
    private string rootTrainingFilePath = @"..\sensorData";
    private BackPropNeuralNet bnn;
    #endregion

    // Use this for initialization
	void Start () {
        // Train First
        if (this.LoadTrainingFiles)
            this.trainNetwork();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    #region Private Methods
    private void createNetwork()
    {
    
    }

    private void trainNetwork()
    {

    }
    #endregion

}