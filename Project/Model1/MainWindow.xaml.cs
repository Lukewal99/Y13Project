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


namespace Model1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public int TYPEP = 0;
        public int TYPEI = 1;
        public int TYPED = 1;

        private DispatcherTimer timer;
        private double currentRotation = 0;
        private double currentRotVel = 0;
        private double appliedRotAcc = 0;
        private double pidRotAcc = 0;
        private bool pidActive = false;
        private int Period = 10; //The loop will run every x milliseconds
        // FOR SOME REASON USING PERIOD DELETES THE RECTANGLE!?!?!?!?!?!?

        PID.PID PID = new PID.PID();

        private double kP = 0;
        private double kI = 0;
        private double kD = 0;

        private double[] pidOutput = new double[3];

        public MainWindow()
        {
            InitializeComponent();


            timer = new DispatcherTimer();
            //timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            timer.Interval = new TimeSpan(0, 0, 0, 0, Period);
            timer.Tick += TimerEvent;
            timer.Start();
        }
        private void TimerEvent(object sender, EventArgs e)
        {
            
            if (pidActive)
            {

                //pidOutput = PID.next(0,currentRotation,kP,kI,kD,0.01);
                double pidTiming = Period / 1000F;
                pidOutput = PID.next(0, currentRotation, kP, kI, kD, pidTiming);

                pidOutput[0] = PID.Scale(pidOutput[0], 200, TYPEP);
                pidOutput[1] = PID.Scale(pidOutput[1], 500, TYPEI); // TRY SET UP ENUM FOR TYPE
                pidOutput[2] = PID.Scale(pidOutput[2], 1000, TYPED);
                pidOutput[0] = PID.Clamp(pidOutput[0], 3);
                pidOutput[1] = PID.Clamp(pidOutput[1], 2);
                pidOutput[2] = PID.Clamp(pidOutput[2], 0.1);

                pidRotAcc = pidOutput[0] + pidOutput[1] + pidOutput[2];
                

            }
            else
            {
                pidRotAcc = 0;
            }

            currentRotVel = currentRotVel + appliedRotAcc + pidRotAcc;

            currentRotation = (currentRotation + currentRotVel) % 180;

            RotateTransform rotateTransform = new RotateTransform(currentRotation);
            Rectangle.RenderTransform = rotateTransform;

            if(currentRotVel > 0.1)
            {
                currentRotVel -= 0.05;
            }
            else if(currentRotVel < -0.1)
            {
                currentRotVel += 0.05;
            }
            else
            {
                currentRotVel = 0;
            }
            AngleDisplay.Text = "Angle: " + Math.Round(currentRotation,3);
            RotVelDisplay.Text = "RotVel: " + Math.Round(currentRotVel,3);
            PidAccDisplay.Text = "PidAcc: " + Math.Round(pidRotAcc,3);
            AppliedAccDisplay.Text = "AppliedAcc: " + Math.Round(appliedRotAcc,3);
        }

        private void PidActive_Click(object sender, RoutedEventArgs e)
        {
            pidActive = !pidActive;
            PidActiveDisplay.Text = Convert.ToString(pidActive);

            if (pidActive)
            {
                PidActiveDisplay.Foreground = new SolidColorBrush(Color.FromRgb(0,185,0)) ;
                PID.It = 0;
            }
            else if (!pidActive)
            {
                
                PidActiveDisplay.Foreground = new SolidColorBrush(Color.FromRgb(255,0,0));
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                appliedRotAcc = -0.2;
            }
            else if (e.Key == Key.Right)
            {
                appliedRotAcc = 0.2;
            }
            else if (e.Key == Key.D0)
            {
                currentRotation = 0;
                currentRotVel = 0;
                pidRotAcc = 0;
                PID.It = 0;
                pidActive = false;
                PidActiveDisplay.Text = "False";
                PidActiveDisplay.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left || e.Key == Key.Right)
            {
                appliedRotAcc = 0;
            }
        }


        private void PSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            kP = Math.Round(Convert.ToDouble(PSlider.Value),2);
            PValue.Text = Convert.ToString(kP);
        }

        private void ISlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            kI = Math.Round(Convert.ToDouble(ISlider.Value),3);
            IValue.Text = Convert.ToString(kI);
        }

        private void DSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            kD = Math.Round(Convert.ToDouble(DSlider.Value),4);
            DValue.Text = Convert.ToString(kD);
        }
    }
}
