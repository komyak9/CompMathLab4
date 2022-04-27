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
            Graph.Plot.Clear();
            Function function;
            int a, b, pointsCount;
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
            function.SetRange(a, b);
            function.GeneratePoints(pointsCount);

            double[] dataX = new double[function.Points.Length];
            double[] dataY = new double[function.Points.Length];
            for (int i = 0; i < function.Points.Length; i++)
            {
                dataX[i] = function.Points[i].X;
                dataY[i] = function.Points[i].Y;
            }
            DrawPoints(dataX, dataY, System.Drawing.Color.CornflowerBlue, "Original", 10);

            SplineInterpolation interpolation = new SplineInterpolation(function);
            interpolation.Calculate();
            DrawPoints(interpolation.interpolatedX, interpolation.interpolatedY, System.Drawing.Color.Red, "Interpolated", 1);
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

        private int GetCount()
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

            int pointsCount;
            try
            {
                pointsCount = int.Parse(count.Text);
            }
            catch
            {
                throw new Exception("Check input for count of points.");
            }

            if (pointsCount < 1)
                throw new Exception("Points count must be 1 or greater.");

            return pointsCount;
        }

        private void DrawPoints(double[] dataX, double[] dataY, System.Drawing.Color color, string lbl, int markSize)
        {
            Graph.Plot.AddScatter(dataX, dataY, color, markerSize: markSize, lineWidth: 1, label: lbl);
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
    }
}
