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

namespace Model1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private int currentRotation = 0;
        private int appliedRotVel = 0;
        private int pidRotVel = 0;
        private bool pidActive = false;
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
            currentRotation = (currentRotation + appliedRotVel + pidRotVel)%360;

            if (pidActive)
            {
                //INSERT PID
            }


            RotateTransform rotateTransform = new RotateTransform(currentRotation);
            Rectangle.RenderTransform = rotateTransform;
        }

        private void PidActive_Click(object sender, RoutedEventArgs e)
        {
            pidActive = !pidActive;
            PidActiveDisplay.Text = Convert.ToString(pidActive);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                appliedRotVel = -2;
            }
            else if (e.Key == Key.Right)
            {
                appliedRotVel = 2;
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left || e.Key == Key.Right)
            {
                appliedRotVel = 0;
            }
        }
    }
}
