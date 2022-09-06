using System;

namespace PID
{
    public class PID
    {
        public static double It = 0;
        public static double PreviousError = 0;
        public static double next(double SetPoint, double ProcessValue, double kP, double kI, double kD, double ClampValue, int ScaleValue, double timeSinceLastUpdate)
        {           //Desired Value, Current Value, constant P,I,D , Maximum allowed return, value used to reduce size of output (returns 1:scale), time since last called PID in seconds
            double Error = 0;
            // Difference between Desired and Current
            Error = SetPoint - ProcessValue;

            // P
            double P = kP * Error;
            
            //P = Scale(P, ScaleValue);
            //P = Clamp(P, ClampValue);

            // I
            double I = kI * Error * timeSinceLastUpdate;
            It += I;
            
            //I = Scale(I, ScaleValue);
            //I = Clamp(I, ClampValue);

            // D
            double D = kD * (Error - PreviousError) / timeSinceLastUpdate;
            
            //D = Scale(D, ScaleValue);
            //D = Clamp(D, ClampValue);


            double Output = P + It + D;
            Output = Scale(Output, ScaleValue);
            Output = Clamp(Output, ClampValue);
            
            PreviousError = Error;

            return Output;
        }

        static double Scale(double input, double ScaleValue)
        {
            return(input / ScaleValue);
        }

        static double Clamp(double input, double ClampValue)
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
