using System;

namespace PID
{
    public static class PID
    {
        public static double next(double SetPoint, double ProcessValue, double kP, double kI, double kD, double Maximum, int Scale)
        {           //Desired Value, Current Value, constant P,I,D , Maximum allowed return, value used to reduce size of output (returns 1:scale)
            double Error = 0;
            // Difference between Desired and Current
            Error = SetPoint - ProcessValue;

            // P
            double P = kP * Error;

            // I
            double I = 0;
            double It = 0;


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
