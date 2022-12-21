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

        PID.PID anglePID = new PID.PID(1000, 0.1);
        PID.PID distancePID = new PID.PID(400, 0.5);

        private double kP = 0;
        private double kI = 0;
        private double kD = 0;

        private double currentX = 100;
        private double currentY = 100;
        private double currentTheta = 0;
        private double currentD = 0;

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
            desiredD = Math.Sqrt((desiredX-currentX)*(desiredX-currentX) + (desiredY-currentY)*(desiredY-currentY));
            if (desiredX-currentX >= 0)
            {
                if (desiredY-currentY >= 0)
                {
                    desiredTheta = Math.Atan((desiredY-currentY) / (desiredX-currentX));
                }
                else if (desiredY - currentY < 0)
                {
                    desiredTheta = Math.Atan((desiredY -currentY) / (desiredX - currentX)) + 2 * Math.PI;
                }
            }
            else if (desiredX - currentX < 0)
            {
                desiredTheta = Math.Atan((desiredY -currentY) / (desiredX - currentX)) + Math.PI;
            }

            // Calculate PID
            // Logic not quite right? Something going wrong.

            if (pidActive)
            {
                DAcc = distancePID.next(desiredD, 0, kP, kI, kD, pidTiming);

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

            // Apply TAcc
            TVel = Math.Min(TVel + TAcc, Math.PI / 10);

            // Apply DAcc
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
            if(currentTheta > (3/2) * Math.PI)
            {
                currentX -= DVel*Math.Cos(currentTheta-(3/2)*Math.PI);
                currentY += DVel*Math.Sin(currentTheta-(3/2)*Math.PI);
            }
            else if (currentTheta > Math.PI)
            {
                currentX -= DVel * Math.Cos(currentTheta -  Math.PI);
                currentY -= DVel * Math.Sin(currentTheta -  Math.PI);
            }
            else if (currentTheta > Math.PI/2)
            {
                currentX += DVel * Math.Cos(currentTheta - Math.PI/2);
                currentY -= DVel * Math.Sin(currentTheta - Math.PI/2);
            }
            else
            {
                currentX += DVel * Math.Cos(currentTheta);
                currentY += DVel * Math.Sin(currentTheta);
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

            // update Visuals
            Canvas.SetLeft(Car, currentX);
            Canvas.SetTop(Car, currentY);
            Canvas.SetLeft(Pointer, desiredX);
            Canvas.SetTop(Pointer, desiredY);

            DesiredDDisplay.Text = "Desired Distance: " + Math.Round(desiredD,2);
            DesiredThetaDisplay.Text = "Desired Angle: " + Math.Round(desiredTheta,2);
            CurrentDDisplay.Text = "Current Distance: " + Math.Round(currentD,2);
            CurrentThetaDisplay.Text = "Current Angle: " + Math.Round(currentTheta,2);
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
        }
    }
}
