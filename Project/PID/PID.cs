using System;

namespace PID
{
    public static class PID
    {
        public static double It = 0;
        public static double next(double SetPoint, double ProcessValue, double kP, double kI, double kD, double Maximum, int Scale, double timeSinceLastUpdate)
        {           //Desired Value, Current Value, constant P,I,D , Maximum allowed return, value used to reduce size of output (returns 1:scale), time since last called PID in seconds
            double Error = 0;
            // Difference between Desired and Current
            Error = SetPoint - ProcessValue;

            // P
            double P = kP * Error;

            // I
            double I = kI * Error * timeSinceLastUpdate;
            It += I;


            // D
            double D = 0;


            double Output = (P + It - D)/Scale;

            if (Output > Maximum)
            {
                return Maximum;
            }
            else if (Output < -Maximum)
            {
                return -Maximum;
            }
            else
            {
                return Output;
            }
        }
    }
}
