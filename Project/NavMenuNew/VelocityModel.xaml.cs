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
using static System.Net.Mime.MediaTypeNames;

namespace NavMenuNew
{
    /// <summary>
    /// Interaction logic for Model3.xaml
    /// </summary>
    public partial class Model3 : GenericModel
    {
        //define variables
        PID.PID anglePID = new PID.PID(500, 0.1);
        PID.PID distancePID = new PID.PID(4000, 0.5);

        public double currentX = 100;
        public double currentY = 100;
        public double desiredX = -10;
        public double desiredY = -10;



        public Model3()
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
            // Calculate desiredD and desiredTheta from desiredX and desiredY
            double deltaX = desiredX - currentX;
            double deltaY = desiredY - currentY;


            desiredD = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            if (deltaY >= 0)
            {
                desiredTheta = Math.PI - Math.Atan(deltaX / deltaY);
            }
            else
            {
                if (deltaX >= 0)
                {
                    desiredTheta = -Math.Atan(deltaX / deltaY);
                    
                }
                else
                {
                    desiredTheta = 2 * Math.PI - Math.Atan(deltaX / deltaY);
                }
            }

            // Calculate PID
            // set DAcc and TAcc to 0 if !pidActive
            if (pidActive) 
            {
                // DAcc
                DAcc = distancePID.next(desiredD, 0, kP, kI, kD, pidTiming);

                //TAcc
                if (desiredD < 1)
                {
                    currentX = desiredX;
                    currentY = desiredY;
                    DAcc = 0;
                    DVel = 0;
                    desiredTheta = 0.5*Math.PI;
                }

                // edge cases for crossing 0
                if (0 <= currentTheta && currentTheta <= 0.5 * Math.PI && 1.5 * Math.PI <= desiredTheta && desiredTheta <= 2 * Math.PI)
                {
                    TAcc = anglePID.next(desiredTheta, currentTheta + 2 * Math.PI, kP, kI, kD, pidTiming);
                }
                else if (0 <= desiredTheta && desiredTheta <= 0.5 * Math.PI && 1.5 * Math.PI <= currentTheta && currentTheta <= 2 * Math.PI)
                {
                    TAcc = anglePID.next(desiredTheta + 2 * Math.PI, currentTheta, kP, kI, kD, pidTiming);
                }
                else
                {
                    TAcc = anglePID.next(desiredTheta, currentTheta, kP, kI, kD, pidTiming);
                }
                
            }
            else
            {
                DAcc = 0;
                TAcc= 0;

            }

            // Update Graph
            graph.addPoint(Math.Sqrt(desiredX*desiredX + desiredY*desiredY), Math.Sqrt(currentX * currentX + currentY * currentY), DAcc);
            if (loopCount == 3)
            {
                loopCount = 0;
                if (graphBool)
                {
                    graph.updateGraph(GraphCanvas, Period);
                }
            }

            #region
            // Apply TAcc
            // Cap at 1/20 pi
            if (TVel >= 0)
            {
                TVel = Math.Min(TVel + TAcc, Math.PI / 20);
            }
            else
            {
                TVel = Math.Max(TVel + TAcc, -Math.PI / 10);
            }
            

            // Apply DAcc
            // always positive
            // Cap at 2
            DVel = Math.Min(DVel + DAcc, 2);


            // Apply TVel
            currentTheta = (currentTheta + TVel)%(2*Math.PI);
            while(currentTheta < 0)
            {
                currentTheta += 2*Math.PI;
            }
            
            // Apply DVel
            if (currentTheta >= 1.5*Math.PI) // Apply North - West
            {
                currentX -= DVel*Math.Cos(currentTheta-1.5*Math.PI);
                currentY -= DVel*Math.Sin(currentTheta-1.5*Math.PI);
            }
            else if(currentTheta >= Math.PI) // Apply South - West
            {
                currentX -= DVel * Math.Sin(currentTheta - Math.PI);
                currentY += DVel * Math.Cos(currentTheta - Math.PI);
            }
            else if (currentTheta >= 0.5 * Math.PI) // Apply South - East
            {
                currentX += DVel * Math.Cos(currentTheta - 0.5 * Math.PI);
                currentY += DVel * Math.Sin(currentTheta - 0.5 * Math.PI);
            }
            else // Apply North - East
            {
                currentX += DVel * Math.Cos(currentTheta);
                currentY -= DVel * Math.Sin(currentTheta);
            }

            // Apply Resistance
            TVel *= 0.98;

            if (TVel > 0.25)
            {
                TVel = 0.25; // Maximum
            }   
            else if (TVel < -0.25)
            {
                TVel = -0.25; // -Maximum
            }
            else if(-0.001 < TVel && TVel < 0.001)
            {
                TVel = 0; // Approaching Zero
            }

            if (DVel > 0.01)
            {
                DVel = 0.98 * DVel;
            }
            else
            {
                DVel = 0; // Approaching Zero
            }

            // update Visuals
            Canvas.SetLeft(Car, currentX-50);
            Canvas.SetTop(Car, currentY-31.25);
            // Rotate the Car
            RotateTransform carRotateTransform = new RotateTransform(360 * currentTheta / (2 * Math.PI) - 90);
            Car.RenderTransform = carRotateTransform;
            #endregion

            DesiredDDisplay.Text = "Desired Distance: " + Math.Round(desiredD,2);
            DesiredThetaDisplay.Text = "Desired Angle: " + Math.Round(desiredTheta/Math.PI,2) + " Pi";
            CurrentThetaDisplay.Text = "Current Angle: " + Math.Round(currentTheta/Math.PI,2) +" Pi";
            ThetaVelDisplay.Text = "Theta Velocity: " + Math.Round(TVel,7);
            ThetaAccelerationDisplay.Text = "Theta Acceleration: " + Math.Round(TAcc,7);
            DistanceVelocityDisplay.Text = "Distance Velocity: " + Math.Round(DVel,2);
            DistanceAccelerationDisplay.Text = "Distance Acceleration: " + Math.Round(DAcc,2);

            SetPoint.Text = Convert.ToString(Math.Round(Math.Sqrt(desiredX * desiredX + desiredY * desiredY), 2));
            ProcessVariable.Text = Convert.ToString(Math.Round(Math.Sqrt(currentX * currentX + currentY * currentY), 2));
            AppliecAcceleration.Text = Convert.ToString(Math.Round(DAcc, 2));
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

        private void largeCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Set Desired X/Y
            Point mousePos = e.GetPosition(largeCanvas);
            desiredX = mousePos.X + 5;
            desiredY = mousePos.Y + 5;

            Canvas.SetLeft(Pointer, desiredX);
            Canvas.SetTop(Pointer, desiredY);

        }

        private void Graph_Click(object sender, RoutedEventArgs e)
        {
            graphBool = !graphBool;

            if (graphBool)
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
