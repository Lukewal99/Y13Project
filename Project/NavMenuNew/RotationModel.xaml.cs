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
        // Define variables
        public double appliedRotAcc = 0;
        public double pidRotAcc = 0;
        PID.PID PID = new PID.PID(250, 0.5);

        public Model1()
        {
            InitializeComponent();

            // Set up timer event
            timer = new DispatcherTimer();
            pidTiming = Period / 1000F;
            timer.Interval = new TimeSpan(0, 0, 0, 0, Period);
            timer.Tick += TimerEvent;
            timer.Start();
        }
        private void TimerEvent(object sender, EventArgs e)
        {
            loopCount++;
            // Calculate PID
            // set TAcc to 0 if !pidActive
            if (pidActive)
            {

                pidRotAcc = PID.next(0, currentTheta, kP, kI, kD, pidTiming);

            }
            else
            {
                pidRotAcc = 0;
            }

            // Update graph
            graph.addPoint(0, currentTheta, pidRotAcc);
            if (loopCount == 3)
            {
                loopCount = 0;
                if (graphBool)
                {
                    graph.updateGraph(GraphCanvas, Period);
                }
            }

            // Update Angle
            TVel = TVel + appliedRotAcc + pidRotAcc;
            currentTheta = (currentTheta + TVel) % 180;

            // Update rotation on the UI
            RotateTransform rotateTransform = new RotateTransform(currentTheta);
            Rectangle.RenderTransform = rotateTransform;
    
            // Apply resistance
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

            ProcessVariable.Text = Convert.ToString(Math.Round(currentTheta, 2));
            AppliecAcceleration.Text = Convert.ToString(Math.Round(pidRotAcc, 2));
        }

        private void PidActive_Click(object sender, RoutedEventArgs e)
        {
            // Flip pidActive
            pidActive = !pidActive;
            PidActiveDisplay.Text = Convert.ToString(pidActive);

            if (pidActive) // Update UI
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
            // P Changed
            kP = Math.Round(Convert.ToDouble(PSlider.Value), 2);
            PValue.Text = Convert.ToString(kP);
        }

        private void ISlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // I Changed
            kI = Math.Round(Convert.ToDouble(ISlider.Value), 3);
            IValue.Text = Convert.ToString(kI);
        }

        private void DSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // D Changed
            kD = Math.Round(Convert.ToDouble(DSlider.Value), 4);
            DValue.Text = Convert.ToString(kD);
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            // Go back to NavMenu
            UserControl Model = new NavMenu();
            Canvas.SetLeft(Model, 0);
            Canvas.SetTop(Model, 0);

            Canvas MainCanvas = (Canvas)this.Parent;
            MainCanvas.Children.Clear();
            MainCanvas.Children.Add(Model);
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            // detect key down
            // check what key and apply the required effect
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
            // undo effects of keyDown
            if (e.Key == Key.Left || e.Key == Key.Right)
            {
                appliedRotAcc = 0;
            }
        }

        private void Graph_Click(object sender, RoutedEventArgs e)
        {
            graphBool = !graphBool;

            if (graphBool)
            {
                GraphButton.Content = "Close Graph";
                GraphCanvas.Visibility = Visibility.Visible;
                KeyCanvas.Visibility = Visibility.Visible;
            }
            else
            {
                GraphButton.Content = "Open Graph";
                GraphCanvas.Visibility = Visibility.Hidden;
                KeyCanvas.Visibility = Visibility.Hidden;
            }
        }
    }
}
