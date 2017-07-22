using System;
using System.Threading;


namespace PIDcontrol
{

    public class PID
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
        public double dT;
        public double outDefault;

        private PIDcontrolComponent pidControlComponent;



        // Constructor
        public PID(double err, double eMin, double eMax, double oMin, double oMax, double pG, double iG, double dG, 
             double feed, double outdefault, double hz, PIDcontrolComponent pidControlComponent)
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
            outDefault = outdefault;
            computeHz = hz;
            this.pidControlComponent = pidControlComponent;
        }

        public PID()
        {
            
        }
        // Deconstructor
        ~PID()
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
            output = outDefault;
            pOut = 0;
            iOut = 0;
            dOut = 0;
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
           

            //PROPORTION
            pOut = error * kp;
   
            DateTime nowTime = DateTime.Now;


            //time difference
            dT = (nowTime - lastUpdate).TotalSeconds;

            //INTEGRAL
            if(Math.Abs(dT - 1 / computeHz) < 0.01)
                iOut += (ki * error);
  

            //DERIVATIVE
            if (Math.Abs(dT) > 0.0001)
                dOut = kd * (error - preError);

            //update time and lastPV
            lastUpdate = nowTime;
            preError = error;

            //Now we have to scale the output value to match the requested scale
            output = pOut + iOut + dOut + feedforward;
            output = ClampValue(output, outMin, outMax);

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
