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


namespace Rotational_Experiment
{
    /// Interaction logic for MainWindow.xaml
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private double windVelocity = 0;
        private double finAngle = 0; // degrees, 0deg is straight
        private double rocketAngle = 0; // degrees, 0deg is up
        private double rotVel = 0;
        private double kp = 10;
        private double ki = 0;
        private double kd = 0.1;
        private DateTime lastUpdate = DateTime.Now;
        private double lastPV;
        private double errSum;
        private double outMin = -0.3;
        private double outMax = 0.3;


        public MainWindow()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1); // every 100 miliseconds
            timer.Tick += TimerEvent;
            timer.Start();
        }

        private void TimerEvent(object sender, EventArgs e) // every Timer event
        {
            

            // Apply windVelocity to Rocket
            // windVelocity increases or decreases rocketAngle
            //if ((rocketAngle + windVelocity) > -90 && (rocketAngle + windVelocity) < 90)
            //{
            //    rocketAngle += windVelocity;
            //}

            finAngle = Compute(rocketAngle, 0, -90, 90);

            // Apply finAngle effect to Rocket
            // finAngle increases or decreases rocketAngle
            //if ((rocketAngle + finAngle) > -90 && (rocketAngle + finAngle) < 90)
            //{
            //    rocketAngle += finAngle;
            //}

            // Calculate Rotational velocity
            rotVel += windVelocity + finAngle;

            // Apply Rotational velocity
            if ((rocketAngle + rotVel) > -90 && (rocketAngle + rotVel) < 90)
            {
                rocketAngle += rotVel;
            }

            // Adjust Rocket
            rocketBodyAngle.Angle = rocketAngle;  // rocketAngle in degrees, with 0 up.


            // Apply Air Resistance
            rotVel /= 1.005;
            if (Math.Abs(rotVel) <= 0.0005)
            {
                rotVel = 0;
            }

            // Adjust Text accordingly
            rocketAngleText.Text = "Roc Ang: " + rocketAngle;
            windVelocityText.Text = "Win Vel: " + windVelocity;
            finAngleText.Text = "Fin Ang: " + finAngle;
            rotVelText.Text = "Rot Vel: " + rotVel;
        }

        // PID Code
        private double ScaleValue(double value, double valuemin,
                    double valuemax, double scalemin, double scalemax)
        {
            double vPerc = (value - valuemin) / (valuemax - valuemin);
            double bigSpan = vPerc * (scalemax - scalemin);

            double retVal = scalemin + bigSpan;

            return retVal;
        }

        private double Clamp(double value, double min, double max)
        {
            if (value > max)
                return max;
            if (value < min)
                return min;
            return value;
        }

        private double Compute(double pv, double sp, double pvMin, double pvMax)
        {

            //We need to scale the pv to +/- 100%, but first clamp it
            pv = Clamp(pv, pvMin, pvMax);
            pv = ScaleValue(pv, pvMin, pvMax, -1.0f, 1.0f);

            //We also need to scale the setpoint
            sp = Clamp(sp, pvMin, pvMax);
            sp = ScaleValue(sp, pvMin, pvMax, -1.0f, 1.0f);

            //Now the error is in percent...
            double err = sp - pv;

            double pTerm = err * kp;
            double iTerm = 0.0f;
            double dTerm = 0.0f;

            double partialSum = 0.0f;
            DateTime nowTime = DateTime.Now;

            
            
            double dT = (nowTime - lastUpdate).TotalSeconds;

            //Compute the integral if we have to...
            if (pv >= pvMin && pv <= pvMax)
            {
                partialSum = errSum + dT * err;
                iTerm = ki * partialSum;
            }

            if (dT != 0.0f)
                dTerm = kd * (pv - lastPV) / dT;
            

            lastUpdate = nowTime;
            errSum = partialSum;
            lastPV = pv;

            //Now we have to scale the output value to match the requested scale
            double outReal = pTerm + iTerm + dTerm;

            outReal = Clamp(outReal, -1.0f, 1.0f);
            outReal = ScaleValue(outReal, -1.0f, 1.0f, outMin, outMax);

            //Write it out to the world
            return(outReal);
        }


        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            windVelocity = -0.5;
        }

        private void Grid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            windVelocity = 0.5;
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            windVelocity = 0;
        }

        private void Grid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            windVelocity = 0;
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (Convert.ToString(e.SystemKey) == "p")
            {
                kp += 1;
            }

            if (Convert.ToString(e.Key) == "P")
            {
                kp -= 1;
            }
        }
    }
}
