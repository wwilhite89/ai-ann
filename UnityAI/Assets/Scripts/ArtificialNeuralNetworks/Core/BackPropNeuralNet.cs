using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;

namespace Assets.Scripts.ArtificialNeuralNetworks.Core
{
    class BackPropNeuralNet
    {
        public enum ActivationMethod
        {
            Sigmoid,
            HyperbolicTangent
        }

        #region Fields
        public int InputNodeCount { get; private set; }
        public int HiddenNodeCount { get; private set; }
        public int OutputNodeCount { get; private set; }
        public int TotalWeightCount
        {
            get
            {
                return (InputNodeCount * HiddenNodeCount) + (HiddenNodeCount * OutputNodeCount) + (HiddenNodeCount + OutputNodeCount);
            }
        }
        public double LearnRate { get; set; }
        public double Momentum { get; set; }
        public int MaxEpochs { get; set; }
        public double ErrorThreshold { get; set; }
        public ActivationMethod HiddenLayerActivation { get; set; }
        public ActivationMethod OutputLayerActivation { get; set; }

        private double[] inputs;
        private double[][] ihWeights; // input-to-hidden
        private double[] hBiases;
        private double[] hSums;
        private double[] hOutputs;

        private double[][] hoWeights;  // hidden-to-output
        private double[] oBiases;
        private double[] oSums;
        private double[] outputs;

        private double[] oGrads; // output gradients for back-propagation
        private double[] hGrads; // hidden gradients for back-propagation

        private double[][] ihPrevWeightsDelta;  // for momentum with back-propagation
        private double[] hPrevBiasesDelta;
        private double[][] hoPrevWeightsDelta;
        private double[] oPrevBiasesDelta;
        #endregion

        #region Constructors

        public BackPropNeuralNet(int numInput, int numHidden, int numOutput, bool randomizeInitialWeights)
        {
            this.InputNodeCount = numInput;
            this.HiddenNodeCount = numHidden;
            this.OutputNodeCount = numOutput;

            inputs = new double[numInput];
            ihWeights = Helpers.MakeMatrix(numInput, numHidden);
            hBiases = new double[numHidden];
            hSums = new double[numHidden];

            hOutputs = new double[numHidden];
            hoWeights = Helpers.MakeMatrix(numHidden, numOutput);
            oBiases = new double[numOutput];
            oSums = new double[numOutput];
            outputs = new double[numOutput];

            oGrads = new double[numOutput];
            hGrads = new double[numHidden];

            ihPrevWeightsDelta = Helpers.MakeMatrix(numInput, numHidden);
            hPrevBiasesDelta = new double[numHidden];
            hoPrevWeightsDelta = Helpers.MakeMatrix(numHidden, numOutput);
            oPrevBiasesDelta = new double[numOutput];

            this.HiddenLayerActivation = ActivationMethod.HyperbolicTangent;
            this.OutputLayerActivation = ActivationMethod.Sigmoid;

            if (randomizeInitialWeights)
                this.randomizeWeights();
        }

        #endregion

        #region Public Methods

        public void Train(double[][] input, double[][] target, bool showLog)
        {
            int epoch = 0;
            double error = double.MaxValue;
            bool[] withinError = new bool[input.Length];
            bool found = false;
			StreamWriter outFile = new StreamWriter("..\\epochError.txt", true);

            Debug.Log("Beginning training using back-propagation");

            while (epoch < this.MaxEpochs) // train
            {
                if (epoch % 100 == 0) Debug.Log("epoch = " + epoch);

                for (int i = 0; i < input.Length; i++)
                {
                    outputs = this.ComputeOutputs(input[i]);
                    error = Helpers.Error(target[i], outputs);

                    withinError[i] = error < this.ErrorThreshold;

                    if (withinError.All(x => x))
                    {
                        found = true;
                        break;
                    }

                    this.UpdateWeights(target[i], this.LearnRate, this.Momentum);
                }

                if (found)
                    break;

				outFile.WriteLine("Error of epoch {0}: {1}", epoch, error);
                ++epoch;
            }

            if (!found)
                Debug.Log(string.Format("Could not train to all targets within {0} epochs.", this.MaxEpochs));
            else if (showLog)
                Debug.Log(string.Format("Trained to all targets within {0} epochs.", epoch));

			outFile.Close();
        }

        public void TrainSingle(double[] input, double[] target)
        {
            int epoch = 0;
            double error = double.MaxValue;
            bool found = false;
            Debug.Log("\nBeginning training using back-propagation\n");

            while (epoch < this.MaxEpochs) // train
            {
                if (epoch % 20 == 0) Debug.Log("epoch = " + epoch);

                outputs = this.ComputeOutputs(input);
                error = Helpers.Error(target, outputs);
                if (error < this.ErrorThreshold)
                {
                    found = true;
                    break;
                }
                this.UpdateWeights(target, this.LearnRate, this.Momentum);
                ++epoch;
            }

            if (!found)
                throw new Exception(string.Format("Could not train to target within {0} epochs.", this.MaxEpochs));
        }

        public void SetWeights(double[] weights)
        {
            // assumes weights[] has order: input-to-hidden wts, hidden biases, hidden-to-output wts, output biases
            if (weights.Length != this.TotalWeightCount)
                throw new Exception("The weights array length: " + weights.Length +
                    " does not match the total number of weights and biases: " + this.TotalWeightCount);

            int k = 0;

            // Input to Hidden
            for (int i = 0; i < this.InputNodeCount; ++i)
                for (int j = 0; j < this.HiddenNodeCount; ++j)
                    ihWeights[i][j] = weights[k++];

            // Hidden biases
            for (int i = 0; i < this.HiddenNodeCount; ++i)
                hBiases[i] = weights[k++];

            // Hidden to Output
            for (int i = 0; i < this.HiddenNodeCount; ++i)
                for (int j = 0; j < this.OutputNodeCount; ++j)
                    hoWeights[i][j] = weights[k++];

            // Output biases
            for (int i = 0; i < this.OutputNodeCount; ++i)
                oBiases[i] = weights[k++];
        }

        public double[] GetWeights()
        {
            double[] result = new double[this.TotalWeightCount];
            int k = 0;
            for (int i = 0; i < ihWeights.Length; ++i)
                for (int j = 0; j < ihWeights[0].Length; ++j)
                    result[k++] = ihWeights[i][j];
            for (int i = 0; i < hBiases.Length; ++i)
                result[k++] = hBiases[i];
            for (int i = 0; i < hoWeights.Length; ++i)
                for (int j = 0; j < hoWeights[0].Length; ++j)
                    result[k++] = hoWeights[i][j];
            for (int i = 0; i < oBiases.Length; ++i)
                result[k++] = oBiases[i];
            return result;
        }

        public double[] GetOutputs()
        {
            double[] result = new double[this.OutputNodeCount];
            this.outputs.CopyTo(result, 0);
            return result;
        }

        public double[] ComputeOutputs(double[] xValues)
        {
            if (xValues.Length != this.InputNodeCount)
                throw new Exception("Inputs array length " + inputs.Length + " does not match NN numInput value " + this.InputNodeCount);

            for (int i = 0; i < this.HiddenNodeCount; ++i)
                this.hSums[i] = 0.0;
            for (int i = 0; i < this.OutputNodeCount; ++i)
                this.oSums[i] = 0.0;

            for (int i = 0; i < xValues.Length; ++i) // copy x-values to inputs
                this.inputs[i] = xValues[i];

            for (int j = 0; j < this.HiddenNodeCount; ++j)  // compute hidden layer weighted sums
                for (int i = 0; i < this.InputNodeCount; ++i)
                    this.hSums[j] += this.inputs[i] * this.ihWeights[i][j];

            for (int i = 0; i < this.HiddenNodeCount; ++i)  // add biases to hidden sums
                this.hSums[i] += this.hBiases[i];

            for (int i = 0; i < this.HiddenNodeCount; ++i)
                this.hOutputs[i] = this.computeActivation(hSums[i], this.HiddenLayerActivation);

            for (int j = 0; j < this.OutputNodeCount; ++j)   // compute output layer weighted sums
                for (int i = 0; i < this.HiddenNodeCount; ++i)
                    this.oSums[j] += this.hOutputs[i] * this.hoWeights[i][j];

            for (int i = 0; i < this.OutputNodeCount; ++i)  // add biases to output sums
                this.oSums[i] += this.oBiases[i];

            for (int i = 0; i < this.OutputNodeCount; ++i)   // apply log-sigmoid activation
                this.outputs[i] = this.computeActivation(oSums[i], this.OutputLayerActivation);

            double[] result = new double[this.OutputNodeCount]; // for convenience when calling method
            this.outputs.CopyTo(result, 0);
            return result;
        }

        public void UpdateWeights(double[] tValues, double learn, double mom) // back-propagation
        {
            // assumes that SetWeights and ComputeOutputs have been called and so inputs and outputs have values
            if (tValues.Length != this.OutputNodeCount)
                throw new Exception("target values not same Length as output in UpdateWeights");

            // 1. compute output gradients.
            for (int i = 0; i < oGrads.Length; ++i)
            {
                double derivative = this.computeActivationDerivative(this.outputs[i], this.OutputLayerActivation);
                oGrads[i] = derivative * (tValues[i] - this.outputs[i]); // oGrad = (1 - O)(O) * (T-O)
            }

            // 2. compute hidden gradients. assumes tanh!
            for (int i = 0; i < hGrads.Length; ++i)
            {
                double derivative = this.computeActivationDerivative(hOutputs[i], this.HiddenLayerActivation);
                double sum = 0.0;
                for (int j = 0; j < this.OutputNodeCount; ++j) // each hidden delta is the sum of numOutput terms
                    sum += oGrads[j] * hoWeights[i][j]; // each downstream gradient * outgoing weight
                hGrads[i] = derivative * sum; // hGrad = (1-O)(1+O) * E(oGrads*oWts)
            }

            // 3. update input to hidden weights (gradients must be computed right-to-left but weights can be updated in any order)
            for (int i = 0; i < ihWeights.Length; ++i) // 0..2 (3)
            {
                for (int j = 0; j < ihWeights[0].Length; ++j) // 0..3 (4)
                {
                    double delta = learn * hGrads[j] * inputs[i]; // compute the new delta = "eta * hGrad * input"
                    ihWeights[i][j] += delta; // update
                    ihWeights[i][j] += mom * ihPrevWeightsDelta[i][j]; // add momentum using previous delta. on first pass old value will be 0.0 but that's OK.
                    ihPrevWeightsDelta[i][j] = delta; // save the delta for next time
                }
            }

            // 4. update hidden biases
            for (int i = 0; i < hBiases.Length; ++i)
            {
                double delta = learn * hGrads[i] * 1.0; // the 1.0 is the constant input for any bias; could leave out
                hBiases[i] += delta;
                hBiases[i] += mom * hPrevBiasesDelta[i];
                hPrevBiasesDelta[i] = delta; // save delta
            }

            // 5. update hidden to output weights
            for (int i = 0; i < hoWeights.Length; ++i)  // 0..3 (4)
            {
                for (int j = 0; j < hoWeights[0].Length; ++j) // 0..1 (2)
                {
                    double delta = learn * oGrads[j] * hOutputs[i];  // hOutputs are inputs to next layer
                    hoWeights[i][j] += delta;
                    hoWeights[i][j] += mom * hoPrevWeightsDelta[i][j];
                    hoPrevWeightsDelta[i][j] = delta;
                }
            }

            // 6. update hidden to output biases
            for (int i = 0; i < oBiases.Length; ++i)
            {
                double delta = learn * oGrads[i] * 1.0;
                oBiases[i] += delta;
                oBiases[i] += mom * oPrevBiasesDelta[i];
                oPrevBiasesDelta[i] = delta;
            }
        } // UpdateWeights

        #endregion

        #region Private Methods

        private double computeActivation(double x, ActivationMethod method)
        {
            switch (method)
            {
                case ActivationMethod.Sigmoid:
                    return sigmoid(x);
                case ActivationMethod.HyperbolicTangent:
                    return hyperTan(x);
                default:
                    return sigmoid(x);
            }
        }

        private double computeActivationDerivative(double x, ActivationMethod method)
        {
            switch (method)
            {
                case ActivationMethod.Sigmoid:
                    return sigmoidDerivative(x);
                case ActivationMethod.HyperbolicTangent:
                    return hyperTanDerivative(x);
                default:
                    return sigmoidDerivative(x);
            }
        }

        private static double sigmoid(double x)
        {
            if (x < -45.0) return 0.0;
            else if (x > 45.0) return 1.0;
            else return 1.0 / (1.0 + Math.Exp(-x));
        }

        private static double sigmoidDerivative(double x)
        {
            // derivative of log-sigmoid is y(1-y)
            return (1 - x) * x;
        }

        private static double hyperTan(double x)
        {
            if (x < -45.0) return -1.0;
            else if (x > 45.0) return 1.0;
            else return Math.Tanh(x);
        }

        private static double hyperTanDerivative(double x)
        {
            // derivative of tanh is (1-y)(1+y) 
            return (1 - x) * (1 + x);
        }
        
        private void randomizeWeights()
        {
            System.Random rnd = new System.Random(1);
            double[] randomWeights = new double[this.TotalWeightCount];

            for (int i = 0; i < randomWeights.Length; ++i)
                randomWeights[i] = (0.1 - 0.01) * rnd.NextDouble() + 0.01;

            this.SetWeights(randomWeights);
        }

        #endregion

    }
}
