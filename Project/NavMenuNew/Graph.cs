using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NavMenu
{
    public class Graph
    {

        List<double> SetPointLine = new List<double>();
        List<double> ProcessValueLine = new List<double>();
        List<double> appliedAccLine = new List<double>();

        double scaleX = 0;
        double scaleY = 0;
        double scaleYAcc = 0;

        public void addPoint(double SP, double PV, double AA)
        {
            SetPointLine.Add(SP);
            ProcessValueLine.Add(PV);
            appliedAccLine.Add( Math.Abs(AA));

            if(SetPointLine.Count > 500 ) 
            { 
                SetPointLine.RemoveRange(0, SetPointLine.Count()-500);
                ProcessValueLine.RemoveRange(0, ProcessValueLine.Count() - 500);
                appliedAccLine.RemoveRange(0, appliedAccLine.Count() - 500);
            }
        }

        public void updateGraph(Canvas theCanvas, int period)
        { //period is how frequent the points get passed in, ms
            theCanvas.Children.Clear();
            scaleX = 715.0 / (double)(SetPointLine.Count + 1); //The period, but in pixels.
            if (SetPointLine.Max() > 0)
            {
                scaleY = 365.0 / (2 * SetPointLine.Max()); // 2* The maximum, but in pixels.
            }
            else
            {
                scaleY = 1;
            }

            if (appliedAccLine.Max() > 0)
            {
                scaleYAcc = 365.0 / (2 * appliedAccLine.Max()); // 2* The maximum acc, but in pixels.
            }
            else
            {
                scaleYAcc = 1;
            }
            
           

            for (int i = 0; i < SetPointLine.Count - 2; i++) // Redraw SetPointLine
            {
                Line line = new Line();
                line.Y1 = 390 - SetPointLine[i] * scaleY;
                line.Y2 = 390 - SetPointLine[i + 1] * scaleY;
                line.X1 = i * scaleX;
                line.X2 = (i + 1) * scaleX;
                line.Stroke = new SolidColorBrush(Colors.Black);
                line.StrokeThickness = 2;
                theCanvas.Children.Add(line);
            }

            for (int i = 0; i < ProcessValueLine.Count - 2; i++) // Redraw ProcessValueLine
            {
                Line line = new Line();
                line.Y1 = 390 - ProcessValueLine[i] * scaleY;
                line.Y2 = 390 - ProcessValueLine[i + 1] * scaleY;
                line.X1 = i * scaleX;
                line.X2 = (i + 1) * scaleX;
                line.Stroke = new SolidColorBrush(Colors.Red);
                line.StrokeThickness = 2;
                theCanvas.Children.Add(line);
            }

            for (int i = 0; i < appliedAccLine.Count - 2; i++) // Redraw AppliedAccLine
            {
                Line line = new Line();
                line.Y1 = 390 - appliedAccLine[i] * scaleYAcc;
                line.Y2 = 390 - appliedAccLine[i + 1] * scaleYAcc;
                line.X1 = i * scaleX;
                line.X2 = (i + 1) * scaleX;
                line.Stroke = new SolidColorBrush(Colors.Green);
                line.StrokeThickness = 2;
                theCanvas.Children.Add(line);
            }
        }
    }
}
