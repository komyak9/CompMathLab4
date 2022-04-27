using System;

namespace Lab_4
{
    internal class FunctionTwo : Function
    {
        public override double CalculateY(double x) => 1 / (1 + 4 * Math.Pow(x, 2));

        public override string ToString() => "y = 1 / (1 + 4 * x^2)";
    }
}
