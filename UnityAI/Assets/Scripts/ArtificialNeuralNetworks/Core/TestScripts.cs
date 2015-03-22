using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.ArtificialNeuralNetworks.Core
{
    public static class TestScripts
    {

        public static void RunXORTest()
        {
            double[][] inputs = 
            {
                new double[]{ 0, 0},
                new double[]{ 0, 1},
                new double[]{ 1, 0},
                new double[]{ 1, 1},
            };

            double[][] goals = 
            { 
                new double[] { 0 }, 
                new double[] { 1 }, 
                new double[] { 1 }, 
                new double[] { 0 },
            
            };
            
            BackPropNeuralNet bnn = new BackPropNeuralNet(2, 2, 1, true)
            {
                LearnRate = 0.38, // 0.38
                Momentum = 1.0, // 1.0
                MaxEpochs = 10000, // 10000
                ErrorThreshold = 0.1,
                HiddenLayerActivation = BackPropNeuralNet.ActivationMethod.Sigmoid,
                OutputLayerActivation = BackPropNeuralNet.ActivationMethod.Sigmoid,
            };
            
            bnn.Train(inputs, goals);

            // Debug.Log("Final neural network weights and bias values are:");
            // Helpers.ShowVector(bnn.GetWeights(), 5, 8, true);

            Debug.Log("The final output values are:");
            for (int i = 0; i < inputs.Length; i++)
            {
                Debug.Log(string.Format("\n{0} XOR {1} = {2}", inputs[i][0], inputs[i][1], bnn.ComputeOutputs(inputs[i])[0]));
                //Helpers.ShowVector(, 5, 8, false);
            }

            // Console.ReadLine();
        }

        public static void RunGenericTest()
        {
            try
            {
                Debug.Log("\nBegin Neural Network training using Back-Propagation demo\n");

                BackPropNeuralNet bnn = new BackPropNeuralNet(3, 4, 2, true)
                {
                    LearnRate = 0.5,
                    Momentum = 0.5,
                    MaxEpochs = 10000,
                    ErrorThreshold = 0.00001,
                    HiddenLayerActivation = BackPropNeuralNet.ActivationMethod.HyperbolicTangent,
                    OutputLayerActivation = BackPropNeuralNet.ActivationMethod.Sigmoid,
                };

                double[] inputs = new double[3] { 1.0, -2.0, 3.0 };
                double[] goal = new double[2] { 0.1234, 0.8766 };

                Debug.Log("The fixed input xValues are:");
                Helpers.ShowVector(inputs, 1, 8, true);

                Debug.Log("The fixed target tValues are:");
                Helpers.ShowVector(goal, 4, 8, true);

                #region BNN information

                Debug.Log("Creating a " + bnn.InputNodeCount + "-input, " + bnn.HiddenNodeCount + "-hidden, " + bnn.OutputNodeCount + "-output neural network");
                Debug.Log("Using " + bnn.HiddenLayerActivation.ToString() + " function for hidden layer activation");
                Debug.Log("Using " + bnn.OutputLayerActivation.ToString() + " function for output layer activation");
                Debug.Log("\nGenerating random initial weights and bias values");
                Debug.Log("\nInitial weights and biases are:");
                Helpers.ShowVector(bnn.GetWeights(), 3, 8, true);
                Debug.Log("Loading weights and biases into neural network");
                Debug.Log(string.Format("Setting learning rate={0} and momentum={1}", bnn.LearnRate.ToString("F2"), bnn.Momentum.ToString("F2")));
                Debug.Log(string.Format("\nSetting max epochs = {0} and error threshold={1}", bnn.MaxEpochs, bnn.ErrorThreshold.ToString("F6")));

                #endregion

                bnn.TrainSingle(inputs, goal);

                Debug.Log("");
                Debug.Log("Final neural network weights and bias values are:");
                Helpers.ShowVector(bnn.GetWeights(), 5, 8, true);

                var finalOutputs = bnn.ComputeOutputs(inputs);
                Debug.Log("The final output values are:");
                Helpers.ShowVector(finalOutputs, 5, 8, true);
                var finalError = Helpers.Error(goal, finalOutputs);
                Debug.Log("The final error is: " + finalError.ToString("F8"));

                Debug.Log("\nEnd Neural Network Back-Propagation demo\n");
                // Console.ReadLine();
            }
            catch (Exception ex)
            {
                Debug.Log("Fatal: " + ex.Message);
                // Console.ReadLine();
            }

        }
        
    }
}
