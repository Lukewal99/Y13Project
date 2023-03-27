using System;

namespace PID
{
    public class PID
    {
        // Define Variables
        public double It = 0;
        private double PreviousError = 0;
        private double Error = 0;
        double ScaleValue = 0;
        double ClampValue = 0;

        public PID(double ScaleValueIn, double ClampValueIn)
        {
            // Set upon creation
            ScaleValue = ScaleValueIn;
            ClampValue = ClampValueIn;
        }

        public double next(double SetPoint, double ProcessValue, double kP, double kI, double kD, double timeSinceLastUpdate)
        {                       //Desired Value,  Current Value, constant P,        I,         D,        time since last update
            
            // Difference between Desired and Current
            Error = SetPoint - ProcessValue;

            // P
            double P = kP * Error;

            // I
            double I = kI * Error * timeSinceLastUpdate;
            this.It += I;

            // D
            double D = kD * (Error - PreviousError) / timeSinceLastUpdate;

            //Scale and Clamp D
            D = Scale(D, ScaleValue);
            D = Clamp(D, ClampValue/3);
            this.PreviousError = Error;
            
            // calculate output
            double output = P+It+D;

            // Scale and Clamp Output
            output = Scale(output, ScaleValue);
            output = Clamp(output, ClampValue);

            return output;
        }

        private double Scale(double input, double ScaleValue)
        {
            // Scale
            double output = input / ScaleValue;

            return output;
            
        }

        private double Clamp(double input, double ClampValue)
        {
            // Clamp
            if (input > ClampValue)
            {
                return ClampValue;
            }
            else if (input < -ClampValue)
            {
                return -ClampValue;
            }
            else if (0.001/ScaleValue < input && input < 0.001/ScaleValue)
            {
                // Approaching Zero
                return 0;
            }
            else
            {
                return input;
            }
        }

    }
}
