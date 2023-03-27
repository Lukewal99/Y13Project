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

        public void updateGraph(int period)
        { //period is how frequent the points get passed in, ms
            double totalTime = SetPointLine.Count * period / 1000; //Total time covered by the graph, in seconds
            int pixelsBetweenPoints = 715/(SetPointLine.Count+1); //The period, but in pixels.

            for (int i = 0; i < SetPointLine.Count -2; i++)
            {
                Line line = new Line();
                line.Y1 = SetPointLine[i];
                line.Y2 = SetPointLine[i+1];
                line.X1 = i*pixelsBetweenPoints;
                line.X2 = i*pixelsBetweenPoints;
                line.Fill = new SolidColorBrush(Color.FromRgb(Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0)));
            }

            for (int i = 0; i < ProcessValueLine.Count - 2; i++)
            {
                Line line = new Line();
                line.Y1 = ProcessValueLine[i];
                line.Y2 = ProcessValueLine[i + 1];
                line.X1 = i * pixelsBetweenPoints;
                line.X2 = i * pixelsBetweenPoints;
                line.Fill = new SolidColorBrush(Color.FromRgb(Convert.ToByte(255), Convert.ToByte(0), Convert.ToByte(0)));
            }

            for (int i = 0; i < appliedAccLine.Count - 2; i++)
            {
                Line line = new Line();
                line.Y1 = appliedAccLine[i];
                line.Y2 = appliedAccLine[i + 1];
                line.X1 = i * pixelsBetweenPoints;
                line.X2 = i * pixelsBetweenPoints;
                line.Fill = new SolidColorBrush(Color.FromRgb(Convert.ToByte(0), Convert.ToByte(255), Convert.ToByte(0)));
            }
        }
    }
}
