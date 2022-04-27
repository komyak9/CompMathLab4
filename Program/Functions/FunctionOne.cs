using System;

namespace Lab_4
{
    internal class FunctionOne : Function
    {
        public override double CalculateY(double x) => 0.1 * x + Math.Pow(Math.E, 0.3 * x);

        public override string ToString() => "y = 0.1 * x + exp^(0.3 * x)";
    }
}