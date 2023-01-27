using NavMenuNew;
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

namespace NavMenuNew
{
    /// <summary>
    /// Interaction logic for Model1.xaml
    /// </summary>
    public partial class Model1 : GenericModel
    {
        public double appliedRotAcc = 0;
        public double pidRotAcc = 0;
        PID.PID PID = new PID.PID(250, 0.5);

        public Model1()
        {
            InitializeComponent();


            timer = new DispatcherTimer();
            pidTiming = Period / 1000F;
            //timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            timer.Interval = new TimeSpan(0, 0, 0, 0, Period);
            timer.Tick += TimerEvent;
            timer.Start();
        }
        private void TimerEvent(object sender, EventArgs e)
        {

            if (pidActive)
            {
              
                pidRotAcc = PID.next(0, currentTheta, kP, kI, kD, pidTiming);

            }
            else
            {
                pidRotAcc = 0;
            }

            TVel = TVel + appliedRotAcc + pidRotAcc;

            currentTheta = (currentTheta + TVel) % 180;

            RotateTransform rotateTransform = new RotateTransform(currentTheta);
            Rectangle.RenderTransform = rotateTransform;

            if (TVel > 0.1)
            {
                TVel -= 0.05;
            }
            else if (TVel < -0.1)
            {
                TVel += 0.05;
            }
            else
            {
                TVel = 0;
            }
            AngleDisplay.Text = "Angle: " + Math.Round(currentTheta, 3);
            RotVelDisplay.Text = "RotVel: " + Math.Round(TVel, 3);
            PidAccDisplay.Text = "PidAcc: " + Math.Round(pidRotAcc, 3);
            AppliedAccDisplay.Text = "AppliedAcc: " + Math.Round(appliedRotAcc, 3);
        }

        private void PidActive_Click(object sender, RoutedEventArgs e)
        {
            pidActive = !pidActive;
            PidActiveDisplay.Text = Convert.ToString(pidActive);

            if (pidActive)
            {
                PidActiveDisplay.Foreground = new SolidColorBrush(Color.FromRgb(0, 185, 0));
                PID.It = 0;
            }
            else if (!pidActive)
            {

                PidActiveDisplay.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            }
        }



        private void PSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            kP = Math.Round(Convert.ToDouble(PSlider.Value), 2);
            PValue.Text = Convert.ToString(kP);
        }

        private void ISlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            kI = Math.Round(Convert.ToDouble(ISlider.Value), 3);
            IValue.Text = Convert.ToString(kI);
        }

        private void DSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            kD = Math.Round(Convert.ToDouble(DSlider.Value), 4);
            DValue.Text = Convert.ToString(kD);
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

        private void Grid_KeyDown(object sender, KeyEventArgs e)
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
                currentTheta = 0;
                TVel = 0;
                pidRotAcc = 0;
                PID.It = 0;
                pidActive = false;
                PidActiveDisplay.Text = "False";
                PidActiveDisplay.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));

            }
        }

        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left || e.Key == Key.Right)
            {
                appliedRotAcc = 0;
            }
        }
    }
}
