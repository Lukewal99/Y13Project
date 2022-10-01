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
        private int rotationalVel = 0; // 10ths of a degree per Ms
        private bool rotLeft = false;
        private bool rotRight = false;
        private int angle = 0; // 10ths of a degree

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
            if (rotLeft)
            {
                if (rotRight)
                {
                    rotationalVel = 0; // If both held
                }
                else
                {
                    rotationalVel -= 1; // If just left
                }
            }
            else if (rotRight)
            {
                rotationalVel += 1; // If just right
            }
            else // If neither
            {
                if (rotationalVel < 0)
                {
                    rotationalVel += 1;
                }
                else if (rotationalVel > 0)
                {
                    rotationalVel -= 1;
                }
            }
            
            angle += rotationalVel;
            rotateTransform.Angle = angle/10;



            Velocity.Text = "Rot Vel: " + rotationalVel;
            Angle.Text = "Ang: " + angle/10;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            rotLeft = true;

        }

        private void Grid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            rotRight = true;
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            rotLeft = false;
        }

        private void Grid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            rotRight = false;
        }
    }
}
