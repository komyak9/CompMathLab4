using System.Collections.Generic;

namespace Lab_4
{
    internal class Calculator
    {
        readonly SplineInterpolation interpolation;
        readonly Function function;
        readonly double a, b, pointsCount;
        double[] originalX, originalY;
        Point[] points;

        public Calculator(Function function, double a, double b, uint pointsCount)
        {
            this.function = function;
            this.a = a;
            this.b = b;
            this.pointsCount = pointsCount;
            points = new Point[pointsCount];

            FillPoints();
            interpolation = new SplineInterpolation(points);
        }

        public void PerformCalculation()
        {
            interpolation.Calculate();
        }

        public double CalculateInterpolatedY(double x) => interpolation.CalculateInterpolatedY(x);

        public double CalculateOriginalY(double x) => function.CalculateY(x);

        private void FillPoints()
        {
            double h = (double)(b - a) / (pointsCount - 1);
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = new Point() { X = a + i * h };
                points[i].Y = function.CalculateY(points[i].X);
            }
        }

        public double[] PointsX
        {
            get
            {
                double[] pointsX = new double[points.Length];
                for (int i = 0; i < points.Length; i++)
                {
                    pointsX[i] = points[i].X;
                }
                return pointsX;
            }
        }

        public double[] PointsY
        {
            get
            {
                double[] pointsY = new double[points.Length];
                for (int i = 0; i < points.Length; i++)
                {
                    pointsY[i] = points[i].Y;
                }
                return pointsY;
            }
        }

        public double[] OrigninalX
        {
            get
            {
                List<double> origX = new List<double>();
                for (double i = a; i < b; i += 0.01)
                    origX.Add(i);

                originalX = origX.ToArray();
                return originalX;
            }
        }

        public double[] OrigninalY
        {
            get
            {
                originalY = new double[originalX.Length];
                for (int i = 0; i < originalY.Length; i++)
                    originalY[i] = function.CalculateY(originalX[i]);
                    
                return originalY;
            }
        }

        public double[] InterpolatedX
        {
            get { return interpolation.interpolatedX; }
        }

        public double[] InterpolatedY
        {
            get { return interpolation.interpolatedY; }
        }

        public double A => a;

        public double B => b;
    }
}
