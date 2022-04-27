using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab_4
{
    internal class SplineInterpolation
    {
        private readonly double[] a, b, d, h;
        private readonly double[] c;
        private readonly int intervalCount;
        private readonly Function function;
        public double[] interpolatedX, interpolatedY;

        public SplineInterpolation(Function function)
        {
            intervalCount = function.Points.Count() - 1;
            this.function = function;
            a = new double[intervalCount + 1];
            b = new double[intervalCount];
            c = new double[intervalCount + 1];
            d = new double[intervalCount];
            h = new double[intervalCount];

            FillA();
            FillH();
        }

        private void FillA()
        {
            for (int i = 0; i < a.Length; i++)
                a[i] = function.Points[i].Y;
        }

        private void FillH()
        {
            for (int i = 0; i < h.Length; i++)
                h[i] = function.Points[i + 1].X - function.Points[i].X;
        }

        public void Calculate()
        {
            CalculateCoefC();
            CalculateCoefD();
            CalculateCoefB();
            CalculateInterpolatedValues();
        }

        private void CalculateCoefB()
        {
            for (int i = 0; i < b.Length; i++)
                b[i] = (function.Points[i + 1].Y - function.Points[i].Y) / h[i] - h[i] * (c[i + 1] + 2 * c[i]) / 3;
        }

        private void CalculateCoefC()
        {
            double[][] matrixC = GetMatrixC();
            double[] vectorC = GetVectorC();

            IterationMethod iterationMethod = new IterationMethod(c.Length);
            iterationMethod.Iterate(matrixC, vectorC, 0.0001);


            for (int i = 0; i < c.Length; i++)
                c[i] = iterationMethod.ResultX[i] / 2;
        }

        private void CalculateCoefD()
        {
            for (int i = 0; i < d.Length; i++)
                d[i] = (c[i + 1] - c[i]) / (3 * h[i]);
        }

        private double[][] GetMatrixC()
        {
            double[][] matrixC = new double[c.Length][];
            for (int i = 0; i < matrixC.Length; i++)
                matrixC[i] = new double[c.Length];

            matrixC[0][0] = 1;
            matrixC[c.Length - 1][c.Length - 1] = 1;
            for (int i = 1; i < matrixC.Length - 1; i++)
            {
                matrixC[i][i - 1] = h[i - 1];
                matrixC[i][i] = 2 * (h[i - 1] + h[i]);
                matrixC[i][i + 1] = h[i];
            }

            return matrixC;
        }

        private double[] GetVectorC()
        {
            double[] vectorC = new double[c.Length];
            for (int i = 1; i < vectorC.Length - 1; i++)
                vectorC[i] = 6 * ((function.Points[i + 1].Y - function.Points[i].Y) / h[i] - (function.Points[i].Y - function.Points[i - 1].Y) / h[i - 1]);

            return vectorC;
        }

        private void CalculateInterpolatedValues()
        {
            List<double> interX = new List<double>();
            List<double> interY = new List<double>();

            int j = 0;
            for (double i = function.Points[0].X; i < function.Points[function.Points.Length - 1].X; i += 0.01)
            {
                if (j > h.Length) break;
                interX.Add(i);
                interY.Add(a[j] + b[j] * (i - function.Points[j].X) + c[j] * Math.Pow(i - function.Points[j].X, 2) + d[j] * Math.Pow(i - function.Points[j].X, 3));
                if (i >= function.Points[j + 1].X) j++;
            }

            interpolatedX = interX.ToArray();
            interpolatedY = interY.ToArray();
        }
    }
}