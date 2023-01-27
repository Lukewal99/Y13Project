using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using NavMenuNew;

namespace NavMenuNew
{
    public class GenericModel : UserControl
    {
        public DispatcherTimer timer;

        public double desiredD = 0;
        public double desiredTheta = Math.PI / 2;
        public double currentD = 0;
        public double currentTheta = 0;
        public double DVel = 0;
        public double DAcc = 0;
        public double TVel = 0;
        public double TAcc = 0;



        public bool pidActive = false;
        public int Period = 10; //The loop will run every x milliseconds
        public double kP = 0;
        public double kI = 0;
        public double kD = 0;
        public double pidTiming = 0;
    }
}
