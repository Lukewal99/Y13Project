using NavMenu;
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
using System.Windows.Threading;

namespace NavMenuNew
{
    /// <summary>
    /// Interaction logic for Model2.xaml
    /// </summary>
    public partial class Model2 : GenericModel
    {
        // Define variables
        PID.PID anglePID = new PID.PID(1000, 0.1);
        PID.PID distancePID = new PID.PID(400, 0.5);

        public Model2()
        {
            InitializeComponent();
            // Create the timer event
            timer = new DispatcherTimer();
            pidTiming = Period / 1000F;
            timer.Interval = new TimeSpan(0, 0, 0, 0, Period);
            timer.Tick += TimerEvent;
            timer.Start();
        }

        private void TimerEvent(object sender, EventArgs e)
        {
            loopCount++;
            // Calculate PID
            // set DAcc and TAcc to 0 if !pidActive
            if (pidActive)
            {
                //DAcc
                DAcc = distancePID.next(desiredD, currentD, kP, kI, kD, pidTiming);

                //TAcc
                if (currentTheta < desiredTheta - Math.PI)
                {
                    TAcc = anglePID.next(desiredTheta - 2 * Math.PI, currentTheta, kP, kI, kD, pidTiming);
                }
                else if (currentTheta - Math.PI > desiredTheta)
                {
                    TAcc = anglePID.next(desiredTheta, currentTheta - 2 * Math.PI, kP, kI, kD, pidTiming);
                }
                else
                {
                    TAcc = anglePID.next(desiredTheta, currentTheta, kP, kI, kD, pidTiming);
                }

            }
            else
            {
                DAcc = 0;
                TAcc = 0;
            }

            // Update graph
            graph.addPoint(desiredD, currentD, DAcc);
            if (loopCount == 3)
            {
                loopCount = 0;
                if (graphBool)
                {
                    graph.updateGraph(GraphCanvas, Period);
                }
            }
            

            // Apply TAcc
            // Cap at 1/20 pi
            TVel = Math.Min(TVel + TAcc, Math.PI / 10);

            // Apply DAcc
            // always positive
            // Cap at 2
            if (DVel + DAcc > 0)
            {
                DVel = Math.Min(DVel + DAcc, 2);
            }
            else
            {
                DVel = Math.Max(DVel + DAcc, -2);
            }

            // Apply TVel
            currentTheta = (currentTheta + TVel);
            while (currentTheta < 0)
            {
                currentTheta += 2 * Math.PI;
            }

            // Apply DVel
            currentD = Math.Min(Math.Abs(currentD + DVel), 200);
            if (Math.Abs(currentD) == 200)
            {
                DVel = 0;
            }



            // Apply Resistance
            if (TVel > 0)
            {
                TVel = 0.98 * TVel;
            }
            else if (TVel < 0)
            {
                TVel = 0.98 * TVel;
            }
            else if (TVel < 0.01 && TVel > -0.01)
            {
                TVel = 0;
            }

            if (TVel > 0.25)
            {
                TVel = 0.25;
            }
            else if (TVel < -0.25)
            {
                TVel = -0.25;
            }

            if (DVel > 0.1)
            {
                DVel = 0.98 * DVel;
            }
            else if (DVel < -0.1)
            {
                DVel = 0.98 * DVel;
            }
            else
            {
                DVel = 0;
            }

            //Update UI
            DesiredThetaDisplay.Text = "Desired Angle: " + Convert.ToString(Math.Round(desiredTheta, 2));
            DesiredDDisplay.Text = "Desired Distance: " + Convert.ToString(Math.Round(desiredD, 2));
            CurrentThetaDisplay.Text = "Current Angle: " + Convert.ToString(Math.Round(currentTheta, 2));
            CurrentDDisplay.Text = "Current Distance: " + Convert.ToString(Math.Round(currentD, 2));
            ThetaVelDisplay.Text = "Theta Velocity: " + Convert.ToString(Math.Round(TVel, 3));
            ThetaAccelerationDisplay.Text = "Theta Acceleration: " + Convert.ToString(Math.Round(TAcc, 4));
            DistanceVelocityDisplay.Text = "Distance Velocity: " + Convert.ToString(Math.Round(DVel, 3));
            DistanceAccelerationDisplay.Text = "Distance Acceleration: " + Convert.ToString(Math.Round(DAcc, 4));

            SetPoint.Text = Convert.ToString(Math.Round(desiredD, 2));
            ProcessVariable.Text = Convert.ToString(Math.Round(currentD, 2));
            AppliecAcceleration.Text = Convert.ToString(Math.Round(DAcc, 2));

            //Sets a pointer position to the current desired location
            Canvas.SetLeft(pointer, Canvas.GetLeft(topDownBase) + topDownBase.Width / 2 - pointer.Width / 2 + desiredD * Math.Cos(desiredTheta));
            Canvas.SetTop(pointer, Canvas.GetTop(topDownBase) + topDownBase.Height / 2 - pointer.Height / 2 + desiredD * Math.Sin(desiredTheta));

            //changes arm to reflect currentTheta
            topDownArmOne.X2 = topDownArmOne.X1 + (currentD / 2) * Math.Cos(currentTheta);
            topDownArmOne.Y2 = topDownArmOne.Y1 + (currentD / 2) * Math.Sin(currentTheta);

            topDownArmTwo.X1 = topDownArmOne.X2;
            topDownArmTwo.Y1 = topDownArmOne.Y2;
            topDownArmTwo.X2 = topDownArmOne.X1 + currentD * Math.Cos(currentTheta);
            topDownArmTwo.Y2 = topDownArmOne.Y1 + currentD * Math.Sin(currentTheta);

            //changes arms to reflect currentD
            sideOnArmOne.X2 = sideOnArmOne.X1 + currentD / 2;
            sideOnArmOne.Y2 = sideOnArmOne.Y1 - Math.Sqrt((100 * 100) - (currentD * currentD / 4));

            sideOnArmTwo.X1 = sideOnArmOne.X2;
            sideOnArmTwo.Y1 = sideOnArmOne.Y2;

            sideOnArmTwo.X2 = sideOnArmOne.X1 + currentD;
            //sideOnArmTwo.Y2 Doesnt change
        }

              private void PSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // P Changed
            kP = Math.Round(Convert.ToDouble(PSlider.Value), 2);
            PValue.Text = Convert.ToString(kP);
        }

        private void ISlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // I Changed
            kI = Math.Round(Convert.ToDouble(ISlider.Value), 2);
            IValue.Text = Convert.ToString(kI);
        }

        private void DSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // D Changed
            kD = Math.Round(Convert.ToDouble(DSlider.Value), 2);
            DValue.Text = Convert.ToString(kD);
        }

        private void PidActive_Click(object sender, RoutedEventArgs e)
        {
            // Flip pidActive
            pidActive = !pidActive;
            PidActiveDisplay.Text = Convert.ToString(pidActive);

            if (pidActive) // Update UI
            {
                PidActiveDisplay.Foreground = new SolidColorBrush(Color.FromRgb(0, 185, 0));
                anglePID.It = 0;
                distancePID.It = 0;
            }
            else if (!pidActive)
            {
                PidActiveDisplay.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            }
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            // Go back to NavMenu
            UserControl Model = new NavMenu();
            Canvas.SetLeft(Model, 0);
            Canvas.SetTop(Model, 0);

            Canvas MainCanvas = (Canvas)this.Parent;
            MainCanvas.Children.Clear();
            MainCanvas.Children.Add(Model);
        }

        private void Range_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Detect click in range
            Point mousePos = e.GetPosition(Range);
            double x = mousePos.X - 200;
            double y = mousePos.Y - 200;

            // Set desired D
            desiredD = Math.Min(Math.Sqrt(x * x + y * y), 200);

            // set desired Theta
            if (x >= 0)
            {
                if (y >= 0)
                {
                    desiredTheta = Math.Atan(y / x);
                }
                else if (y < 0)
                {
                    desiredTheta = Math.Atan(y / x) + 2 * Math.PI;
                }
            }
            else if (x < 0)
            {
                desiredTheta = Math.Atan(y / x) + Math.PI;
            }

            DesiredDDisplay.Text = "Desired Distance: " + Convert.ToString(Math.Round(desiredD, 2));
            DesiredThetaDisplay.Text = "Desired Angle: " + Convert.ToString(Math.Round(desiredTheta, 2));
        }

        private void Graph_Click(object sender, RoutedEventArgs e)
        {
            graphBool = !graphBool;

            if(graphBool)
            {
                GraphButton.Content = "Close Graph";
                GraphCanvas.Visibility = Visibility.Visible;
                KeyCanvas.Visibility = Visibility.Visible;
            }
            else
            {
                GraphButton.Content = "Open Graph";
                GraphCanvas.Visibility = Visibility.Hidden;
                KeyCanvas.Visibility = Visibility.Hidden;
            }
        }
    }
}
