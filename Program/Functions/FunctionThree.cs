using System;

namespace Lab_4
{
    internal class FunctionThree : Function
    {
        public override double CalculateY(double x) => Math.Sin(x);

        public override string ToString() => "y = sin(x)";
    }
}
