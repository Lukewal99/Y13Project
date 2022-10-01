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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private int rotVel = 0; // 10ths of a degree per ms
        private int windVelocity = 0;
        private int correctiveAction = 0;
        private int disturbance = 0;
        private int airResistance = 0;
        private int rocketAngle = 1800; // 10ths of a degree

        public MainWindow()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1); // every milisecond
            timer.Tick += timerEvent;
            timer.Start();
        }

        private void timerEvent(object sender, EventArgs e)
        {
            // Calculate Correction 
            correctiveAction = 0; // temp

            // Calculate Air Resistance
            if(rotVel%2 == 1)
            {
                airResistance = -(rotVel/2)-1;
            }
            else if(rotVel % 2 == -1)
            {
                airResistance = -(rotVel / 2)+1;
            }
            else
            {
                airResistance = -rotVel/2;
            }

            // Calculate Disturbacne
            disturbance = (900 - Math.Abs(rocketAngle - 1800)) / 600 * windVelocity; //currently stops at 140/220 not 90/270

            // Apply Accelerations to Velocity 
            rotVel +=  disturbance + correctiveAction + airResistance;

            // Apply Velocity to Angle
            rocketAngle += rotVel;
            rotateTransform.Angle = (rocketAngle/10)-180;

            // Adjust Text accordingly
            Velocity.Text = "Rot Vel: " + rotVel;
            Angle.Text = "Ang: " + rocketAngle/10;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            windVelocity = -10;

        }

        private void Grid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            windVelocity = 10;
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            windVelocity = 0;
        }

        private void Grid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            windVelocity = 0;
        }
    }
}
