using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Grasshopper.Kernel;
using PIDcontrol.Properties;
using Rhino.Geometry;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace PIDcontrol
{
    public class PIDcontrolComponent : GH_Component
    {
        public double error;
        public double kp;
        public double ki;
        public double kd;
        public double feedforward;
        public double errMin;
        public double errMax;
        public double outMax;
        public double outMin;
        public double computeHz;
        public bool enable;
        public bool reset;
        public PID iPID;

        public double Out;
        public double pOut;
        public double iOut;
        public double dOut;
        
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public PIDcontrolComponent()
          : base("PIDcontrol", "PIDcontrol",
              "PID controller",
              "ViveTrack", "PID")
        {
            iPID = null;
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Err", "Err", "The error as feedback", GH_ParamAccess.item);
            pManager.AddNumberParameter("ErrMin", "ErrMin", "The minimum error as limitation.", GH_ParamAccess.item);
            pManager.AddNumberParameter("ErrMax", "ErrMax", "The maximum error as limitation.", GH_ParamAccess.item);
            pManager.AddNumberParameter("OutMin", "OutMin", "The minimum output value.", GH_ParamAccess.item);
            pManager.AddNumberParameter("OutMax", "OutMax", "The maximum output value.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Kp", "Kp", "Proportional parameter", GH_ParamAccess.item);
            pManager.AddNumberParameter("Ki", "Ki", "Integral parameter", GH_ParamAccess.item);
            pManager.AddNumberParameter("Kd", "Kd", "Derivative parameter", GH_ParamAccess.item);
            pManager.AddNumberParameter("FeedFwd", "FeedFwd", "Feed forward value for output.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Hz", "Hz", "compute frequency", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Enable", "Enable", "Enable the controller", GH_ParamAccess.item,true);
            pManager.AddBooleanParameter("Reset", "Reset", "reset the controller", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Out", "Out", "Output in real scale after clamp", GH_ParamAccess.item);
            pManager.AddIntegerParameter("pOut", "pOut", "p Output in real scale", GH_ParamAccess.item);
            pManager.AddIntegerParameter("iOut", "iOut", "i Output in real scale", GH_ParamAccess.item);
            pManager.AddIntegerParameter("dOut", "dOut", "d Output in real scale", GH_ParamAccess.item);
            pManager.AddNumberParameter("dT", "dT", "dT", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.GetData("Err", ref error);
            DA.GetData("ErrMin", ref errMin);
            DA.GetData("ErrMax", ref errMax);
            DA.GetData("OutMin", ref outMin);
            DA.GetData("OutMax", ref outMax);
            DA.GetData("Kp", ref kp);
            DA.GetData("Ki", ref ki);
            DA.GetData("Kd", ref kd);
            DA.GetData("FeedFwd", ref feedforward);
            DA.GetData("Hz", ref computeHz);
            DA.GetData("Enable", ref enable);
            DA.GetData("Reset", ref reset);

            if (iPID == null) iPID = new PID(error,errMin,errMax,outMin,outMax,kp,ki,kd,feedforward,computeHz, this);
            iPID.error = error;
            iPID.errMin = errMin;
            iPID.errMax = errMax;
            iPID.outMin = outMin;
            iPID.outMax = outMax;
            iPID.kp = kp;
            iPID.ki = ki;
            iPID.kd = kd;
            iPID.feedforward = feedforward;
            iPID.computeHz = computeHz;

            if(enable) iPID.Enable();
            else iPID.Disable();
            if(reset) iPID.Reset();

            Out = iPID.output;
            pOut = iPID.pOut;
            iOut = iPID.iOut;
            dOut = iPID.dOut;

            DA.SetData("Out", Convert.ToInt32(Out));
            DA.SetData("pOut", Convert.ToInt32(pOut));
            DA.SetData("iOut", Convert.ToInt32(iOut));
            DA.SetData("dOut", Convert.ToInt32(dOut));
            DA.SetData("dT", iPID.dT);
            ExpireSolution(true);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Resources.icon;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("9a56991e-00e6-4926-9583-bd7245438aa8"); }
        }
    }
}
