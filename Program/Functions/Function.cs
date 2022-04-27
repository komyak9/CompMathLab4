using System;

namespace Lab_4
{
    internal abstract class Function
    {
        protected Point[] points;
        private int a, b;

        public void SetRange(int a, int b)
        {
            this.a = a;
            this.b = b;
        }

        public void GeneratePoints(int count)
        {
            Random random = new Random();
            points = new Point[count];
            double h = (double)(b - a) / (count - 1);

            for (int i = 0; i < points.Length; i++)
            {
                points[i] = new Point
                {
                    X = (a + h * i)
                };
                points[i].Y = CalculateY(points[i].X) + (random.NextDouble() - 1) / 100;
            }
        }

        public abstract double CalculateY(double x);

        public Point[] Points
        {
            get { return points; }
            set { points = value; }
        }
        public double A
        {
            get { return a; }
        }
        public double B
        {
            get { return b; }
        }
    }
}