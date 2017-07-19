using System;
using System.Threading;


namespace PIDcontrol
{

    public class PID1
    {
        //Gains
        public double kp;
        public double ki;
        public double kd;
        public double feedforward;

        //Running Values
        public DateTime lastUpdate;
        public double preError;
        public double errSum;
        public double pOut;
        public double iOut;
        public double dOut;

        //Reading/Writing Values
        public double error;
        public double output;

        //Max/Min output value
        public double errMin;
        public double errMax;
        public double outMax;
        public double outMin;

        //Threading and Timing
        public double computeHz;
        public Thread runThread;

        private PIDcontrolComponent pidControlComponent;



        // Constructor
        public PID1(double err, double eMin, double eMax, double oMin, double oMax, double pG, double iG, double dG, 
             double feed, double hz, PIDcontrolComponent pidControlComponent)
        {
            kp = pG;
            ki = iG;
            kd = dG;
            errMin = eMin;
            errMax = eMax;
            outMax = oMax;
            outMin = oMin;
            error = err;
            feedforward = feed;
            computeHz = hz;
            this.pidControlComponent = pidControlComponent;
        }

        public PID1()
        {
            
        }
        // Deconstructor
        ~PID1()
        {
            Disable();
        }

        // Methods
        public void Enable()
        {
            if (runThread != null)
                return;

            Reset();

            runThread = new Thread(new ThreadStart(Run));
            runThread.IsBackground = true;
            runThread.Name = "PID_Processor";
            runThread.Start();
        }

        public void Disable()
        {
            if (runThread == null)
                return;

            runThread.Abort();
            runThread = null;
        }

        public void Reset()
        {
            errSum = 0.0;
            preError = 0.0;
            output = feedforward + outMin;
            lastUpdate = DateTime.Now;
        }


        private double RemapValue(double value, double oldmin, double oldmax, double newmin, double newmax)
        {
            double vPercentage = (value - oldmin) / (oldmax - oldmin);
            return newmin + vPercentage * (newmax - newmin);
        }

        //limit the value between minimum and maximum.
        private double ClampValue(double value, double min, double max)
        {
            if (value > max)
                return max;
            if (value < min)
                return min;
            return value;
        }

        private void Compute()
        {

            error = ClampValue(error, errMin, errMax);
            error = RemapValue(error, errMin, errMax, -1.0, 1.0);

            //Now the error is in percent
            //PROPORTION
            double pTerm = error * kp;
            double iTerm = 0.0;
            double dTerm = 0.0;

            double tempSum = 0.0;
            DateTime nowTime = DateTime.Now;


            //time difference
            double dT = (nowTime - lastUpdate).TotalSeconds;

            //INTEGRAL
            tempSum = errSum + dT * error;
            iTerm = ki * tempSum;

            //DERIVATIVE
            if (Math.Abs(dT) > 0.0001) dTerm = kd * (error - preError) / dT;

            //update time and lastPV
            lastUpdate = nowTime;
            errSum = tempSum;
            preError = error;

            //Now we have to scale the output value to match the requested scale
            output = pTerm + iTerm + dTerm;
            output = ClampValue(output, -1.0, 1.0);
            output = feedforward + RemapValue(output, -1.0, 1.0, outMin, outMax);

            //just show how much pid outputs in real scale
            pOut = RemapValue(pTerm, -1.0, 1.0, outMin, outMax);
            iOut = RemapValue(iTerm, -1.0, 1.0, outMin, outMax);
            dOut = RemapValue(dTerm, -1.0, 1.0, outMin, outMax);

        }


        //Threading
        private void Run()
        {

            while (true)
            {
                try
                {
                    int sleepTime = (int)(1000 / computeHz);
                    Thread.Sleep(sleepTime);
                    Compute();
                }
                catch (Exception e)
                {
                }
            }

        }

        public String bOut
        {
            get
            {
                int v = Convert.ToInt32(this.output);

                char a = (char) ((v & (0xff << 8)) >> 8);
                char b = (char)(v & 0xff);
                return Convert.ToString(a) + Convert.ToString(b);
            }
            
        }
    }
}
