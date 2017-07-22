using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino;
using Rhino.Geometry;

namespace PIDcontrol
{
    public class Robot
    {
        public Plane CurrentPos { set; get; }
        public List<Curve> PathCurves { set; get; }
        public double Tolerance { set; get; }
        public List<Point3d> PathPoints { set; get; }
        public Point3d TargetPos { set; get; }
        public bool Pausing { set; get; }
        public string Message { set; get; }
        public List<Object> Visualization;

        private double divideLength = 0.2;
        private double turnTolerance = 10.0 * Math.PI / 180;
        private int index = 0;
        private int pointsCount;
        private Vector3d CurrentVec;
        private Vector3d TargetVec;
        private bool Running;
        private bool Ending = false;


        public double angle;



        public Robot(Plane currentPos, List<Curve> paths, double tolerance)
        {
            CurrentPos = currentPos;
            PathCurves = paths;
            Tolerance = tolerance;
            PathPoints = GetPathPoints();
            pointsCount = PathPoints.Count;
            TargetPos = PathPoints[index];
            Running = true;
        }

        private List<Point3d> GetPathPoints()
        {
            List<Point3d> pathPoints = new List<Point3d>();
            foreach (var crv in PathCurves)
            {
                Point3d[] points;
                crv.DivideByLength(divideLength, true, out points);
                pathPoints.AddRange(points.ToList());
                pathPoints.Add(crv.PointAtEnd);
            }
            return pathPoints;
        }

        public void Update()
        {
            Running = !Pausing && !Ending;
            if (Running)
            {
                CheckDestination();
                Move();
            }
            else Pause();
            Visualize();
        }

        private void CheckDestination()
        {
            if (CurrentPos.Origin.DistanceTo(TargetPos) < Tolerance && index < pointsCount - 1)
            {
                index += 1;
                TargetPos = PathPoints[index];
            }
            if (CurrentPos.Origin.DistanceTo(TargetPos) < Tolerance && index == pointsCount - 1) Ending = true;

        }

        private void Move()
        {
            CurrentVec = CurrentPos.YAxis;
            TargetVec = Vector3d.Subtract((Vector3d)TargetPos,(Vector3d)CurrentPos.Origin);
            angle = Vector3d.VectorAngle(CurrentVec, TargetVec, Plane.WorldXY);
            if(angle > turnTolerance && angle < Math.PI) TurnLeft();
            else if (angle > Math.PI && angle < Math.PI * 2 - turnTolerance) TurnRight();
            else MoveForward();
        }

        private void Visualize()
        {
             Visualization = new List<object>(){TargetPos,Tolerance,CurrentPos,CurrentVec,TargetVec};
        }

        private void MoveForward()
        {
            Message = "W";
        }

        private void TurnRight()
        {
            Message = "D";
        }

        private void TurnLeft()
        {
            Message = "A";
        }

        private void Pause()
        {
            Message = "";
        }

    }
}
