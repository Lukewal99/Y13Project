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
    public partial class Model3 : UserControl
    {
        private DispatcherTimer timer;

        private bool pidActive = false;
        static int Period = 10; //The loop will run every x milliseconds
        double pidTiming = Convert.ToDouble(Period) / 1000;

        PID.PID anglePID = new PID.PID(500, 0.1);
        PID.PID distancePID = new PID.PID(4000, 0.5);

        private double kP = 0;
        private double kI = 0;
        private double kD = 0;

        private double currentX = 100;
        private double currentY = 100;
        private double currentTheta = 0;

        private double desiredX = -10;
        private double desiredY = -10;

        private double DVel = 0;
        private double DAcc = 0;
        private double TVel = 0;
        private double TAcc = 0;

        private double desiredD = 0;
        private double desiredTheta = 0;


        public Model3()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, Period);
            timer.Tick += TimerEvent;
            timer.Start();
        }

        private void TimerEvent(object sender, EventArgs e)
        {
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
                DAcc = distancePID.next(desiredD, 0, kP, kI, kD, (double)Period/1000);

                //TAcc
                TAcc = anglePID.next(desiredTheta, currentTheta, kP, kI, kD, pidTiming);
            }
            else
            {
                DAcc = 0;
                TAcc= 0;

            }

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
            if(currentTheta < 0)
            {
                currentTheta = -currentTheta;
            }
            
            // Apply DVel
            if (currentTheta >= 1.5*Math.PI)
            {
                currentX -= DVel*Math.Cos(currentTheta-1.5*Math.PI);
                currentY -= DVel*Math.Sin(currentTheta-1.5*Math.PI);
            }
            else if(currentTheta >= Math.PI)
            {
                currentX -= DVel * Math.Sin(currentTheta - Math.PI);
                currentY += DVel * Math.Cos(currentTheta - Math.PI);
            }
            else if (currentTheta >= 0.5 * Math.PI)
            {
                currentX += DVel * Math.Cos(currentTheta - 0.5 * Math.PI);
                currentY += DVel * Math.Sin(currentTheta - 0.5 * Math.PI);
            }
            else
            {
                currentX += DVel * Math.Cos(currentTheta);
                currentY -= DVel * Math.Sin(currentTheta);
            }

            // Apply Resistance
            TVel *= 0.98;

            if (TVel > 0.25)
            {
                TVel = 0.25;
            }   
            else if (TVel < -0.25)
            {
                TVel = -0.25;
            }
            else if(-0.001 < TVel && TVel < 0.001)
            {
                TVel = 0;
            }

            if (DVel > 0.01)
            {
                DVel = 0.98 * DVel;
            }
            else
            {
                DVel = 0;
            }

            // update Visuals
            Canvas.SetLeft(Car, currentX);
            Canvas.SetTop(Car, currentY);

            RotateTransform rotateTransform = new RotateTransform(360*currentTheta/(2*Math.PI) -90);
            Car.RenderTransform = rotateTransform;

            DesiredDDisplay.Text = "Desired Distance: " + Math.Round(desiredD,2);
            DesiredThetaDisplay.Text = "Desired Angle: " + Math.Round(desiredTheta/Math.PI,2) + " Pi";
            CurrentThetaDisplay.Text = "Current Angle: " + Math.Round(currentTheta/Math.PI,2) +" Pi";
            ThetaVelDisplay.Text = "Theta Velocity: " + Math.Round(TVel,7);
            ThetaAccelerationDisplay.Text = "Theta Acceleration: " + Math.Round(TAcc,7);
            DistanceVelocityDisplay.Text = "Distance Velocity: " + Math.Round(DVel,2);
            DistanceAccelerationDisplay.Text = "Distance Acceleration: " + Math.Round(DAcc,2);
        }

        private void PSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            kP = Math.Round(Convert.ToDouble(PSlider.Value), 2);
            PValue.Text = Convert.ToString(kP);
        }

        private void ISlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            kI = Math.Round(Convert.ToDouble(ISlider.Value), 2);
            IValue.Text = Convert.ToString(kI);
        }

        private void DSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            kD = Math.Round(Convert.ToDouble(DSlider.Value), 2);
            DValue.Text = Convert.ToString(kD);
        }

        private void PidActive_Click(object sender, RoutedEventArgs e)
        {
            pidActive = !pidActive;
            PidActiveDisplay.Text = Convert.ToString(pidActive);

            if (pidActive)
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
            UserControl Model = new NavMenu();
            Canvas.SetLeft(Model, 0);
            Canvas.SetTop(Model, 0);

            Canvas MainCanvas = (Canvas)this.Parent;
            MainCanvas.Children.Clear();
            MainCanvas.Children.Add(Model);
        }

        private void largeCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePos = e.GetPosition(largeCanvas);
            desiredX = mousePos.X;
            desiredY = mousePos.Y;

            Canvas.SetLeft(Pointer, desiredX);
            Canvas.SetTop(Pointer, desiredY);

        }
    }
}
