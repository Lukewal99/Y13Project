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
        private DispatcherTimer timer;
        private double currentRotation = 0;
        private double currentRotVel = 0;
        private double appliedRotAcc = 0;
        private double pidRotAcc = 0;
        private bool pidActive = false;
        //private int Period = 10; //The loop will run every x milliseconds
        // FOR SOME REASON USING PERIOD DELETES THE RECTANGLE!?!?!?!?!?!?


        private double P = 0;
        private double I = 0;
        private double D = 0;

        public MainWindow()
        {
            InitializeComponent();


            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            //timer.Interval = new TimeSpan(0, 0, 0, 0, Period);
            timer.Tick += TimerEvent;
            timer.Start();
        }
        private void TimerEvent(object sender, EventArgs e)
        {
            
            if (pidActive)
            {
                pidRotAcc = PID.PID.next(0,currentRotation,P,I,D,200,10,0.01);
                //pidRotAcc = PID.PID.next(0, currentRotation, P, I, D, 200, 10, Period/1000);

            }
            else
            {
                pidRotAcc = 0;
            }

            currentRotVel = currentRotVel + appliedRotAcc + pidRotAcc;

            currentRotation = (currentRotation + currentRotVel) % 180;

            RotateTransform rotateTransform = new RotateTransform(currentRotation);
            Rectangle.RenderTransform = rotateTransform;

            if(currentRotVel > 0.05)
            {
                currentRotVel -= 0.05;
            }
            else if(currentRotVel < -0.05)
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
                PID.PID.It = 0;
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
                PID.PID.It = 0;
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
            P = Math.Round(Convert.ToDouble(PSlider.Value),2);
            PValue.Text = Convert.ToString(P);
        }

        private void ISlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            I = Math.Round(Convert.ToDouble(ISlider.Value),3);
            IValue.Text = Convert.ToString(I);
        }

        private void DSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            D = Math.Round(Convert.ToDouble(DSlider.Value),4);
            DValue.Text = Convert.ToString(D);
        }
    }
}
