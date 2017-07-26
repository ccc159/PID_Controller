using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using PIDcontrol.Xbox360;
using Rhino.Geometry;

namespace PIDcontrol
{
    public class XboxVibration : GH_Component
    {

        private XboxController currentController;
        private int _index;
        private double _leftspeed;
        private double _rightspeed;
        private int _timespan;
        private bool _send;
        private TimeSpan _Time;


        /// <summary>
        /// Initializes a new instance of the XboxVibration class.
        /// </summary>
        public XboxVibration()
          : base("XboxVibration", "XboxVibration",
              "Send Vibration to your Xbox controller",
              "ViveTrack", "Xbox")
        {
            _index = 0;
            _Time = TimeSpan.Zero;
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("ControllerIndex", "ControllerIndex", "The index of your Xbox 360 controller, 0,1,2 or 3. Default 0.", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("LeftMotorSpeed", "LeftMotorSpeed","Specify how strong the motor speed is from 0.0 to 1.0, where 1.0 is maximum and 0.0 is stop",GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("RightMotorSpeed", "RightMotorSpeed", "Specify how strong the motor speed is from 0.0 to 1.0, where 1.0 is maximum and 0.0 is stop", GH_ParamAccess.item, 0.0);
            pManager.AddIntegerParameter("VibrationTime", "VibrationTime", "Specify vibration time in miliseconds. If set <= 0 then it keeps vibrating.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Send", "Send", "If true, the vibration command will send to the controller",GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.GetData("ControllerIndex", ref _index);
            DA.GetData("LeftMotorSpeed", ref _leftspeed);
            DA.GetData("RightMotorSpeed", ref _rightspeed);
            DA.GetData("VibrationTime", ref _timespan);
            DA.GetData("Send", ref _send);

            

            _leftspeed = ClampValues(_leftspeed);
            _rightspeed = ClampValues(_rightspeed);


            try
            {
                currentController = XboxController.RetrieveController(_index);
            }
            catch (Exception e)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
                return;
            }
            

            if (!_send) return;
            if (_timespan <= 0.0)
            {
                currentController.Vibrate(_leftspeed, _rightspeed);
            }
            else
            {
                _Time = TimeSpan.FromMilliseconds(_timespan);
                currentController.Vibrate(_leftspeed, _rightspeed,_Time);
            }
           
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("a19fc0c7-ce81-444e-8e64-a8a1d2bdba5e"); }
        }

        private double ClampValues(double v)
        {
            if (v > 1.0d) return 1.0d;
            if (v < 0.0d) return 0.0d;
            return v;
        }
    }
}