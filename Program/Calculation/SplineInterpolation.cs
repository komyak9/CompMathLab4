using System;
using System.Collections.Generic;
using IterationLib;

namespace Lab_4
{
    internal class SplineInterpolation
    {
        public double[] interpolatedX, interpolatedY;
        readonly SubInterval[] intervals;

        public SplineInterpolation(Point[] points)
        {
            intervals = new SubInterval[points.Length - 1];

            for (int i = 0; i < points.Length - 1; i++)
                intervals[i] = new SubInterval(points[i], points[i + 1]);
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
            for (int i = 0; i < intervals.Length; i++)
            {
                double cLast;
                if (i == intervals.Length - 1) cLast = 0;
                else cLast = intervals[i + 1].C;

                intervals[i].B = (intervals[i].P2.Y - intervals[i].P1.Y) / intervals[i].H
                    - intervals[i].H * (cLast + 2 * intervals[i].C) / 3;
            }
        }

        private void CalculateCoefC()
        {
            double[][] matrixC = GetMatrixC();
            double[] vectorC = GetVectorC();

            IterationMethod iterationMethod = new IterationMethod(intervals.Length + 1);
            iterationMethod.Iterate(matrixC, vectorC, 0.00001);

            for (int i = 1; i < intervals.Length; i++)
            {
                intervals[i].C = iterationMethod.ResultX[i] / 2;
            };
        }

        private void CalculateCoefD()
        {
            for (int i = 0; i < intervals.Length; i++)
            {
                double cLast;
                if (i == intervals.Length - 1) cLast = 0;
                else cLast = intervals[i + 1].C;

                intervals[i].D = (cLast - intervals[i].C) / (3 * intervals[i].H);
            }
        }

        private double[][] GetMatrixC()
        {
            double[][] matrixC = new double[intervals.Length + 1][];
            for (int i = 0; i < matrixC.Length; i++)
                matrixC[i] = new double[intervals.Length + 1];

            matrixC[0][0] = 1;
            matrixC[intervals.Length][intervals.Length] = 1;
            for (int i = 1; i < matrixC.Length - 1; i++)
            {
                matrixC[i][i - 1] = intervals[i - 1].H;
                matrixC[i][i] = 2 * (intervals[i - 1].H + intervals[i].H);
                matrixC[i][i + 1] = intervals[i].H;
            }

            return matrixC;
        }

        private double[] GetVectorC()
        {
            double[] vectorC = new double[intervals.Length + 1];
            for (int i = 1; i < vectorC.Length - 1; i++)
                vectorC[i] = 6 * ((intervals[i].P2.Y - intervals[i - 1].P2.Y) / intervals[i].H
                    - (intervals[i - 1].P2.Y - intervals[i - 1].P1.Y) / intervals[i - 1].H);

            return vectorC;
        }

        private void CalculateInterpolatedValues()
        {
            List<double> interX = new List<double>();
            List<double> interY = new List<double>();

            int j = 0;
            for (double i = intervals[0].P1.X; i < intervals[intervals.Length - 1].P2.X; i += 0.01)
            {
                //if (j >= intervals.Length) break;
                interX.Add(i);
                interY.Add(CalculateInterpolatedY(intervals[j].A, intervals[j].B, intervals[j].C, intervals[j].D, i, intervals[j].P1.X));
                if (i >= intervals[j].P2.X) j++;
            }

            interpolatedX = interX.ToArray();
            interpolatedY = interY.ToArray();
        }

        private double CalculateInterpolatedY(double a, double b, double c, double d, double x, double x0)
        {
            return a + b * (x - x0) + c * Math.Pow(x - x0, 2) + d * Math.Pow(x - x0, 3);
        }

        public double CalculateInterpolatedY(double x)
        {
            for (int i = 0; i < intervals.Length; i++)
            {
                if (x >= intervals[i].P1.X && x <= intervals[i].P2.X)
                    return CalculateInterpolatedY(intervals[i].A, intervals[i].B, intervals[i].C, intervals[i].D, x, intervals[i].P1.X);
            }
            return 0;
        }

        class SubInterval
        {
            double a, b, c, d, h = 0;
            readonly Point p1;
            readonly Point p2;

            public SubInterval(Point p1, Point p2)
            {
                a = p1.Y;
                h = p2.X - p1.X;
                this.p1 = p1;
                this.p2 = p2;
            }

            public double A => a;
            public double H => h;
            public Point P1 => p1;
            public Point P2 => p2;

            public double B
            {
                get { return b; }
                set { b = value; }
            }

            public double C
            {
                get { return c; }
                set { c = value; }
            }

            public double D
            {
                get { return d; }
                set { d = value; }
            }
        }
    }
}