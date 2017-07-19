using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PIDcontrol
{
    public class PathFollowing : GH_Component
    {
        public List<Point3d> PathNodes;
        public int Index;
        public Point3d CurrentPos;
        public double ReachRange;
        public bool Pause;
        public bool Loop;
        public bool Reset;
        /// <summary>
        /// Initializes a new instance of the PathFollowing class.
        /// </summary>
        public PathFollowing()
          : base("PathFollowing", "PathFollowing",
              "PathFollowing",
              "ViveTrack", "PID")
        {
            PathNodes = new List<Point3d>();
            CurrentPos = Point3d.Unset;
            Index = 0;
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("PathNodes", "PathNodes", "", GH_ParamAccess.list);
            pManager.AddPointParameter("CurrentPos", "CurrentPos", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("ReachRange", "Reachrange", "", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Pause", "Pause", "", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Loop", "Loop", "", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Reset", "Reset", "", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("TargetPos", "TargetPos", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            PathNodes = new List<Point3d>();
            if(!DA.GetDataList("PathNodes", PathNodes))return;
            if(!DA.GetData("CurrentPos", ref CurrentPos))return;
            DA.GetData("ReachRange", ref ReachRange);
            DA.GetData("Pause", ref Pause);
            DA.GetData("Loop", ref Loop);
            DA.GetData("Reset", ref Reset);

            if (Reset) Index = 0;
            int count = PathNodes.Count;
            DA.SetData("TargetPos", PathNodes[Index]);
            double dist = CurrentPos.DistanceTo(PathNodes[Index]);
            if (!(dist < ReachRange)) return;
            if (Pause) return;
            if (Index < count - 1)
            {
                Index += 1;
            } 
            else
            {
                if (Loop) Index = 0;
                else return;
            }

            DA.SetData("TargetPos", PathNodes[Index]);
            this.ExpireSolution(true);
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
            get { return new Guid("52c7d081-23bf-47e6-bb8e-4b66d8d96e90"); }
        }
    }
}