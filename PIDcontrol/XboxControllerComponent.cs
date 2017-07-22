using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using PIDcontrol.Properties;
using PIDcontrol.Xbox360;
using Rhino.Geometry;

namespace PIDcontrol
{
    public class XboxControllerComponent : GH_Component
    {
        private XboxController currentController;
        public int index;
        private bool connected = true;
        public bool autoupdate;

        public double LeftXAxis;
        public double LeftYAxis;
        public double RightXAxis;
        public double RightYAxis;
        public double LeftTrigger;
        public double RightTrigger;
        /// <summary>
        /// Initializes a new instance of the XboxController class.
        /// </summary>
        public XboxControllerComponent()
          : base("XboxController", "XboxController",
              "Get XboxController States",
              "ViveTrack", "Xbox")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("ControllerIndex", "ControllerIndex","The index of your Xbox 360 controller, 0,1,2 or 3. Default 0.", GH_ParamAccess.item, 0);
            pManager.AddBooleanParameter("AutoUpdate", "AutoUpdate","Determine if this component is autoupdating or you want to use your own timer.", GH_ParamAccess.item,false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Left X Axis", "Left X Axis", "Left X Axis, from -1.0 to 1.0",GH_ParamAccess.item);
            pManager.AddNumberParameter("Left Y Axis", "Left Y Axis", "Left Y Axis, from -1.0 to 1.0", GH_ParamAccess.item);
            pManager.AddNumberParameter("Right X Axis", "Right X Axis", "Right X Axis, from -1.0 to 1.0", GH_ParamAccess.item);
            pManager.AddNumberParameter("Right Y Axis", "Right Y Axis", "Right Y Axis, from -1.0 to 1.0", GH_ParamAccess.item);
            pManager.AddNumberParameter("Left Trigger", "Left Trigger", "Left Trigger, from 0.0 to 1.0", GH_ParamAccess.item);
            pManager.AddNumberParameter("Right Trigger", "Right Trigger", "Right Trigger, from 0.0 to 1.0", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Button A", "Button A", "True if Button A is pressed", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Button B", "Button B", "True if Button B is pressed", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Button X", "Button X", "True if Button X is pressed", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Button Y", "Button Y", "True if Button Y is pressed", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Button Back", "Button Back", "True if Button Back is pressed", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Button Start", "Button Start", "True if Button Start is pressed", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Button Left Shoulder", "Button Left Shoulder", "True if Button Left Shoulder is pressed", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Button Right Shoulder", "Button Right Shoulder", "True if Button Right Shoulder is pressed", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Button Left Stick", "Button Left Stick", "True if Button Left Stick is pressed", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Button Right Stick", "Button Right Stick", "True if Button Right Stick is pressed", GH_ParamAccess.item);
            pManager.AddBooleanParameter("DownPad Left", "DownPad Left", "True if DownPad Left is pressed", GH_ParamAccess.item);
            pManager.AddBooleanParameter("DownPad Right", "DownPad Right", "True if DownPad Right is pressed", GH_ParamAccess.item);
            pManager.AddBooleanParameter("DownPad Up", "DownPad Up", "True if DownPad Up is pressed", GH_ParamAccess.item);
            pManager.AddBooleanParameter("DownPad Down", "DownPad Down", "True if DownPad Down is pressed", GH_ParamAccess.item);
            pManager.AddTextParameter("Battery Info", "Battery Info", "Return if controller is wired or wireless. If wireless then the battery level.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.GetData(0, ref index);
            DA.GetData(1, ref autoupdate);

            try
            {
                currentController = XboxController.RetrieveController(index);
            }
            catch (Exception e)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
                return;
            }

            
            CheckConnection();
            if(!connected) return;

            XboxController.StartPolling();
            LeftXAxis = RemapValue(currentController.LeftThumbStick.X, -32768.0, 32767.0, -1.0, 1.0);
            LeftYAxis = RemapValue(currentController.LeftThumbStick.Y, -32768.0, 32767.0, -1.0, 1.0);
            RightXAxis = RemapValue(currentController.RightThumbStick.X, -32768.0, 32767.0, -1.0, 1.0);
            RightYAxis = RemapValue(currentController.RightThumbStick.Y, -32768.0, 32767.0, -1.0, 1.0);
            LeftTrigger = RemapValue(currentController.LeftTrigger, 0.0, 255.0, 0.0, 1.0);
            RightTrigger = RemapValue(currentController.RightTrigger, 0.0, 255.0, 0.0, 1.0);

            DA.SetData("Left X Axis", LeftXAxis);
            DA.SetData("Left Y Axis", LeftYAxis);
            DA.SetData("Right X Axis", RightXAxis);
            DA.SetData("Right Y Axis", RightYAxis);
            DA.SetData("Left Trigger", LeftTrigger);
            DA.SetData("Right Trigger", RightTrigger);
            DA.SetData("Button A", currentController.IsAPressed);
            DA.SetData("Button B", currentController.IsBPressed);
            DA.SetData("Button X", currentController.IsXPressed);
            DA.SetData("Button Y", currentController.IsYPressed);
            DA.SetData("Button Back", currentController.IsBackPressed);
            DA.SetData("Button Start", currentController.IsStartPressed);
            DA.SetData("Button Left Shoulder", currentController.IsLeftShoulderPressed);
            DA.SetData("Button Right Shoulder", currentController.IsRightShoulderPressed);
            DA.SetData("Button Left Stick", currentController.IsLeftStickPressed);
            DA.SetData("Button Right Stick", currentController.IsRightStickPressed);
            DA.SetData("DownPad Left", currentController.IsDPadLeftPressed);
            DA.SetData("DownPad Right", currentController.IsDPadRightPressed);
            DA.SetData("DownPad Up", currentController.IsDPadUpPressed);
            DA.SetData("DownPad Down", currentController.IsDPadDownPressed);
            DA.SetData("Battery Info", currentController.BatteryInformationGamepad);

            if (autoupdate)
            {
                this.OnPingDocument().ScheduleSolution(50, doc => {
                    this.ExpireSolution(false);
                });
            }
        }

        private async void CheckConnection()
        {
            await Task.Delay(500);
            if (!currentController.IsConnected)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Xbox 360 Controller " + index + " is not connected.");
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
                return Resources.xbox;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("52cf6a82-150e-4a78-bf9e-26305221b871"); }
        }

        private double RemapValue(double value, double oldmin, double oldmax, double newmin, double newmax)
        {
            double vPercentage = (value - oldmin) / (oldmax - oldmin);
            return newmin + vPercentage * (newmax - newmin);
        }

    }
}