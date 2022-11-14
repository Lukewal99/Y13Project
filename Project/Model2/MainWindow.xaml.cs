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
using PID;

namespace Model2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;

        private bool pidActive = false;
        private int Period = 10; //The loop will run every x milliseconds

        PID.PID anglePID = new PID.PID(1,10);
        PID.PID distancePID = new PID.PID(1,10000);

        private double kP = 0;
        private double kI = 0;
        private double kD = 0;

        private double desiredD = 100;
        private double desiredTheta = Math.PI/2;

        private double currentD = 100;
        private double currentTheta,
        currentDVel,
        currentDAcc,
        currentThetaVel,
        currentThetaAcc = 0;

        public MainWindow()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, Period);
            timer.Tick += TimerEvent;
            timer.Start();
        }

        private void TimerEvent(object sender, EventArgs e)
        {
            currentD = desiredD;


            //changes arms to reflect currentD
            sideOnArmOne.X2 = sideOnArmOne.X1 + desiredD/2;
            sideOnArmOne.Y2 = sideOnArmOne.Y1 - Math.Sqrt((100*100)-(desiredD*desiredD/4));

            sideOnArmTwo.X1 = sideOnArmOne.X2;
            sideOnArmTwo.Y1 = sideOnArmOne.Y2;

            sideOnArmTwo.X2 = sideOnArmOne.X1 + currentD;
            //sideOnArmTwo.Y2 Doesnt change
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

        private void Range_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePos = e.GetPosition(Range);
            double x = mousePos.X - 200;
            double y = mousePos.Y - 200;

            desiredD = Math.Min( Math.Sqrt(x * x + y * y),200);

            if (x >= 0)
            {
                if (y >= 0)
                {
                    desiredTheta = Math.Atan(y / x);
                }
                else if(y < 0)
                {
                    desiredTheta = Math.Atan(y / x) + 2*Math.PI;
                }
            }
            else if (x < 0)
            {
                desiredTheta = Math.Atan(y / x) + Math.PI;
            }

            DesiredDDisplay.Text = "Desired Distance: " + Convert.ToString(Math.Round(desiredD,2));
            DesiredThetaDisplay.Text = "Desired Angle: " + Convert.ToString(Math.Round(desiredTheta,2));
        }
    }
}
