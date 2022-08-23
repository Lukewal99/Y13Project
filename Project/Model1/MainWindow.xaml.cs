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

        private double P = 0;
        private double I = 0;
        private double D = 0;
        public MainWindow()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1); // every milliseconds
            timer.Tick += TimerEvent;
            timer.Start();
        }
        private void TimerEvent(object sender, EventArgs e)
        {
            
            if (pidActive)
            {
                pidRotAcc = PID.PID.next(0,currentRotation,P,I,D,4,30); 
                // 0.1 slowly meets point
                // 5 flicks and meets
                // 10 flicks and meets
                // 100 overshoots.
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
                currentRotVel -= 0.1;
            }
            else if(currentRotVel < -0.1)
            {
                currentRotVel += 0.1;
            }
            else
            {
                currentRotVel = 0;
            }
            AngleDisplay.Text = "Angle: " + currentRotation;
            RotVelDisplay.Text = "RotVel: " + currentRotVel;
            PidAccDisplay.Text = "PidAcc: " + pidRotAcc;
            AppliedAccDisplay.Text = "AppliedAcc: " + appliedRotAcc;

        }

        private void PidActive_Click(object sender, RoutedEventArgs e)
        {
            pidActive = !pidActive;
            PidActiveDisplay.Text = Convert.ToString(pidActive);

            if (pidActive)
            {
                PidActiveDisplay.Foreground = new SolidColorBrush(Color.FromRgb(0,185,0)) ;
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
                appliedRotAcc = -1;
            }
            else if (e.Key == Key.Right)
            {
                appliedRotAcc = 1;
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
