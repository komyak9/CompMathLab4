using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Lab_4
{
    public partial class MainWindow : Window
    {
        private readonly Function[] functions = { new FunctionOne(), new FunctionTwo(), new FunctionThree() };
        private Calculator calculator;

        public MainWindow()
        {
            InitializeComponent();
            MinWidth = CalculateMinWidth();

            Graph.Plot.Title("Spline interpolation");
            Graph.Plot.XLabel("x");
            Graph.Plot.YLabel("y");
            Graph.Plot.Legend();
            Graph.Plot.AxisScaleLock(true);

            for (int i = 0; i < functions.Length; i++)
            {
                Functions.Children.Add(new RadioButton { GroupName = "FunctionRadioButton", Name = 'f' + (i + 1).ToString(), Content = functions[i] });
            }
        }

        private void CalcButtonClick(object sender, RoutedEventArgs e)
        {
            yOriginal.Text = string.Empty;
            yInterpolated.Text = string.Empty;
            x.Text = string.Empty;
            Graph.Plot.Clear();

            Function function;
            int a, b;
            uint pointsCount;
            try
            {
                function = GetFunction();
                a = GetA();
                b = GetB();
                CheckBounds(a, b);
                pointsCount = GetCount();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            calculator = new Calculator(function, a, b, pointsCount);
            calculator.PerformCalculation();
            DrawPoints(calculator.OrigninalX, calculator.OrigninalY, System.Drawing.Color.CornflowerBlue, "Original", 1, 1);
            DrawPoints(calculator.InterpolatedX, calculator.InterpolatedY, System.Drawing.Color.Red, "Interpolated", 1, 1);
            DrawPoints(calculator.PointsX, calculator.PointsY, System.Drawing.Color.Black, "Boundaries", 4, 0);


            pointCalculator.Visibility = Visibility.Visible;
        }

        private Function GetFunction()
        {
            List<RadioButton> radioButtons = Functions.Children.OfType<RadioButton>().Where(r => r.GroupName == "FunctionRadioButton").ToList();
            RadioButton radioButton = null;
            foreach (var r in radioButtons)
                if (r.IsChecked == true) radioButton = r;

            if (radioButton == null)
                throw new Exception("Function must be chosen!");
            else
            {
                foreach (Function f in functions)
                    if (f.ToString() == radioButton.Content.ToString()) return f;
            }
            return null;
        }

        private int GetA()
        {
            if (a.Text == string.Empty)
            {
                a.Background = new SolidColorBrush(Color.FromRgb(255, 161, 161));
                a.BorderBrush = Brushes.Red;
                throw new Exception("Lower border 'a' must be chosen!");
            }
            else
            {
                a.ClearValue(BorderBrushProperty);
                a.ClearValue(BackgroundProperty);
            }

            int numberA;
            try
            {
                numberA = int.Parse(a.Text);
            }
            catch
            {
                throw new Exception("Check input for lower border 'a'.");
            }
            return numberA;
        }

        private int GetB()
        {
            if (b.Text == string.Empty)
            {
                b.Background = new SolidColorBrush(Color.FromRgb(255, 161, 161));
                b.BorderBrush = Brushes.Red;
                throw new Exception("Upper border 'b' must be chosen!");
            }
            else
            {
                b.ClearValue(BorderBrushProperty);
                b.ClearValue(BackgroundProperty);
            }

            int numberB;
            try
            {
                numberB = int.Parse(b.Text);
            }
            catch
            {
                throw new Exception("Check input for upper border 'b'.");
            }
            return numberB;
        }

        private void CheckBounds(int aValue, int bValue)
        {
            if (aValue >= bValue)
            {
                a.Background = new SolidColorBrush(Color.FromRgb(255, 161, 161));
                a.BorderBrush = Brushes.Red;
                b.Background = new SolidColorBrush(Color.FromRgb(255, 161, 161));
                b.BorderBrush = Brushes.Red;
                throw new Exception("Lower bound can't be greater than upper one.");
            }
            else
            {
                a.ClearValue(BorderBrushProperty);
                a.ClearValue(BackgroundProperty);
                b.ClearValue(BorderBrushProperty);
                b.ClearValue(BackgroundProperty);
            }
        }

        private uint GetCount()
        {
            if (count.Text == string.Empty)
            {
                count.Background = new SolidColorBrush(Color.FromRgb(255, 161, 161));
                count.BorderBrush = Brushes.Red;
                throw new Exception("Count of points must be chosen!");
            }
            else
            {
                count.ClearValue(BorderBrushProperty);
                count.ClearValue(BackgroundProperty);
            }

            uint pointsCount;
            try
            {
                pointsCount = uint.Parse(count.Text);
            }
            catch
            {
                throw new Exception("Check input for count of points.");
            }

            if (pointsCount < 2)
                throw new Exception("Points count must be 2 or greater.");

            return pointsCount;
        }

        private void DrawPoints(double[] dataX, double[] dataY, System.Drawing.Color color, string lbl, int markSize, int lineSize)
        {
            Graph.Plot.AddScatter(dataX, dataY, lineWidth: lineSize, color: color, markerSize: markSize, label: lbl);
            Graph.Refresh();
        }

        private int CalculateMinWidth()
        {
            int maxFunctionName = 0;
            for (int i = 0; i < functions.Length; i++)
            {
                if (functions[i].ToString().Length > maxFunctionName)
                    maxFunctionName = functions[i].ToString().Length;
            }

            return maxFunctionName * 25;
        }

        private void CalculateY_TextChanged(object sender, RoutedEventArgs e)
        {
            ScottPlot.Plottable.MarkerPlot dot1 = null, dot2 = null;

            if (x.Text == "-" || x.Text == "\u2408" || x.Text == string.Empty)
                return;

            double xValue = 0, originalY, interpolatedY;
            try
            {
                xValue = double.Parse(x.Text);
                if (xValue < calculator.A || xValue > calculator.B)
                    throw new Exception("X must be in range [a, b].");
            } catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            originalY = calculator.CalculateOriginalY(xValue);
            interpolatedY = calculator.CalculateInterpolatedY(xValue);
            yOriginal.Text = originalY.ToString();
            yInterpolated.Text = interpolatedY.ToString();

            dot1 = Graph.Plot.AddPoint(xValue, originalY, color: System.Drawing.Color.CornflowerBlue, size: 7);
            dot2 = Graph.Plot.AddPoint(xValue, interpolatedY, color: System.Drawing.Color.Red, size: 7);
            Graph.Refresh();

            Graph.Plot.Remove(dot1);
            Graph.Plot.Remove(dot2);
        }
    }
}
