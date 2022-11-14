using System;

namespace PID
{
    public class PID
    {
        public double It = 0;
        static double PreviousError = 0;
        static double Error = 0;
        double ScaleValue = 0;
        double ClampValue = 0;

        public PID(double ScaleValueIn, double ClampValueIn)
        {
            ScaleValue = ScaleValueIn;
            ClampValue = ClampValueIn;
        }

        public double next(double SetPoint, double ProcessValue, double kP, double kI, double kD, double timeSinceLastUpdate)
        {                           //Desired Value, Current Value, constant P,I,D
            
            // Difference between Desired and Current
            Error = SetPoint - ProcessValue;

            // P
            double P = kP * Error;

            // I
            double I = kI * Error * timeSinceLastUpdate;
            It += I;

            // D
            double D = kD * (Error - PreviousError) / timeSinceLastUpdate;
            

            PreviousError = Error;

            P = Clamp(P);


            double output = P+It+D;

            return output;
        }

        private double Scale(double input)
        {
            double output = input / ScaleValue;

            if (output < 0.05 && output > 0)
            {
                return 0.06;
            }
            else if (output > -0.05 && output < 0)
            {
                return -0.06;
            }

            return output;
            
        }

        private double Clamp(double input)
        {
            if (input > ClampValue)
            {
                return ClampValue;
            }
            else if (input < -ClampValue)
            {
                return -ClampValue;
            }
            else
            {
                return input;
            }
        }

    }
}
