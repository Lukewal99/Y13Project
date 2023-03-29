using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GraphControl
{
    /// <summary>
    /// Interaction logic for Graph.xaml
    /// </summary>
    public partial class Graph : UserControl
    {
        List<double> SetPointLine = new List<double>();
        List<double> ProcessValueLine = new List<double>();
        List<double> appliedAccLine = new List<double>();

        List<Line> lines = new List<Line>();

        public Graph()
        {
            InitializeComponent();
        }

        public void addPoint(double SP, double PV, double AA)
        {
            SetPointLine.Add(SP);
            ProcessValueLine.Add(PV);
            appliedAccLine.Add(AA);
        }

        public void updateGraph(Canvas theCanvas, int period)
        { //period is how frequent the points get passed in, ms
            theCanvas.Children.Clear();
            double totalTime = SetPointLine.Count * period / 1000; //Total time covered by the graph, in seconds
            double scaleX = 715.0/(double)(SetPointLine.Count+1); //The period, but in pixels.
            double scaleY = 365.0/SetPointLine.Max(); //The maximum, but in pixels.

            theCanvas.Children.Add( new Line() { X1 = 100, Y1 = 100, X2 = 200, Y2 = 200, Stroke = new SolidColorBrush(Colors.Blue) , StrokeThickness = 5});

            for (int i = 0; i < SetPointLine.Count -2; i++)
            {
                Line line = new Line();
                line.Y1 = SetPointLine[i] * scaleY;
                line.Y2 = SetPointLine[i+1] * scaleY;
                line.X1 = i*scaleX;
                line.X2 = (i+1)*scaleX;
                line.Stroke = new SolidColorBrush(Colors.Black);
                line.StrokeThickness = 2;
                theCanvas.Children.Add(line);
                lines.Add(line);
            }

            for (int i = 0; i < ProcessValueLine.Count - 2; i++)
            {
                Line line = new Line();
                line.Y1 = 370 - ProcessValueLine[i]*scaleY;
                line.Y2 = 370 - ProcessValueLine[i + 1]*scaleY;
                line.X1 = i * scaleX;
                line.X2 = (i+1) * scaleX;
                line.Stroke = new SolidColorBrush(Colors.Red);
                line.StrokeThickness = 2;
                theCanvas.Children.Add(line);
            }

            for (int i = 0; i < appliedAccLine.Count - 2; i++)
            {
                Line line = new Line();
                line.Y1 = 370 - appliedAccLine[i]*scaleY;
                line.Y2 = 370 - appliedAccLine[i + 1]*scaleY;
                line.X1 = i * scaleX;
                line.X2 = (i + 1) * scaleX;
                line.Stroke = new SolidColorBrush(Colors.Green);
                line.StrokeThickness = 2;
                theCanvas.Children.Add(line);
            }
        }
    }
}
