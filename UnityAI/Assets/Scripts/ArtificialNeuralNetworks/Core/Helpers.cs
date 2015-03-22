using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.ArtificialNeuralNetworks.Core
{
    public class Helpers
    {
        public static double[][] MakeMatrix(int rows, int cols)
        {
            double[][] result = new double[rows][];
            for (int i = 0; i < rows; ++i)
                result[i] = new double[cols];
            return result;
        }

        public static void ShowVector(double[] vector, int decimals, int valsPerLine, bool blankLine)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < vector.Length; ++i)
            {
                sb.Append(vector[i].ToString("F" + decimals) + ", "); // n decimals
                if ((i != 0 && i % valsPerLine == 0) || valsPerLine == 1)
                {
                    Debug.Log(sb.ToString());
                    sb.Remove(0, sb.Length);
                }
                
            }

            if (sb.Length > 0)
                Debug.Log(sb.ToString());

            if (blankLine) Debug.Log("\n");
        }

        public static void ShowMatrix(double[][] matrix, int numRows, int decimals)
        {
            int ct = 0;
            if (numRows == -1) numRows = int.MaxValue; // if numRows == -1, show all rows
            for (int i = 0; i < matrix.Length && ct < numRows; ++i)
            {
                for (int j = 0; j < matrix[0].Length; ++j)
                {
                    if (matrix[i][j] >= 0.0) Debug.Log(" "); // blank space instead of '+' sign
                    Debug.Log(matrix[i][j].ToString("F" + decimals) + " ");
                }
                Debug.Log("");
                ++ct;
            }
            Debug.Log("");
        }

        public static double Error(double[] tValues, double[] yValues)
        {
            double sum = 0.0;
            for (int i = 0; i < tValues.Length; ++i)
                sum += (tValues[i] - yValues[i]) * (tValues[i] - yValues[i]);
            return Math.Sqrt(sum);
        }
    }
}
