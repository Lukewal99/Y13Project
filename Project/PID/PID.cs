using System;

namespace PID
{
    public class PID
    {
        public double It = 0;
        static double PreviousError = 0;
        static double Error = 0;

        public double[] next(double SetPoint, double ProcessValue, double kP, double kI, double kD, double timeSinceLastUpdate)
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

            double[] output = { P, It, D };

            return output;
        }

        public double Scale(double input, double ScaleValue, int Type)
        {
            double output = input / ScaleValue;
            if (Type == 0)
            {
                if (output < 0.05 && output > 0)
                {
                    return (0.06);
                }
                else if (output > -0.05 && output < 0)
                {
                    return (-0.06);
                }
            }

            return (input / ScaleValue);
            
        }

        public double Clamp(double input, double ClampValue)
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
