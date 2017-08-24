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
        public Plane MovingPlane { set; get; }
        public List<Object> Visualization;

        private double divideLength = 0.15;
        private double turnTolerance = 20.0 * Math.PI / 180;
        private int index = 0;
        private int pointsCount;
        private Point3d Location;
        private Point3d PrevLocation;
        private Vector3d CurrentVel;
        private Vector3d DesiredVel;
        private Vector3d Acceleration;
        private DateTime LastUpdate;
        private bool Running;
        private bool Ending = false;


        public double angle;



        public Robot(Plane currentPos, List<Curve> paths, double tolerance, Plane movingPlane)
        {
            CurrentPos = currentPos;
            Location = CurrentPos.Origin;
            PrevLocation = Location;
            PathCurves = paths;
            Tolerance = tolerance;
            PathPoints = GetPathPoints();
            pointsCount = PathPoints.Count;
            TargetPos = PathPoints[index];
            MovingPlane = movingPlane;
            LastUpdate = DateTime.Now;
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

        public void Update(Plane curPln)
        {
            Running = !Pausing && !Ending;
            if (Running)
            {
                
                UpdateState(curPln);
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

        private void UpdateState(Plane curPln)
        {
            // current location
            CurrentPos = curPln;
            Location = CurrentPos.Origin;
            // target location
            CheckDestination();
            // current velocity
            GetCurrentVel();
            // desired velocity
            DesiredVel = Vector3d.Subtract((Vector3d)TargetPos, (Vector3d)Location);
            // acceleration
            Acceleration = DesiredVel - CurrentVel;
        }

        private void Move()
        {
            
            angle = Vector3d.VectorAngle(CurrentVel, DesiredVel, MovingPlane);
            if(angle > turnTolerance && angle < Math.PI) TurnLeft();
            else if (angle > Math.PI && angle < Math.PI * 2 - turnTolerance) TurnRight();
            else MoveForward();
        }

        private void GetCurrentVel()
        {
            DateTime nowTime = DateTime.Now;
            double dT = (nowTime - LastUpdate).TotalSeconds;
            CurrentVel = (Location - PrevLocation) / dT;
            LastUpdate = nowTime;
            PrevLocation = Location;
        }

        private void Visualize()
        {
             Visualization = new List<object>(){TargetPos,Tolerance,CurrentPos, CurrentVel, DesiredVel};
        }

        private void MoveForward()
        {
            Message = string.Format(">{0},{1}<", System.Math.Round(Acceleration.X, 2), System.Math.Round(Acceleration.Y, 2));
        }

        private void TurnRight()
        {
            Message = ">1.0,0.0<";
        }

        private void TurnLeft()
        {
            Message = ">-1.0,0.0<";
        }

        private void Pause()
        {
            Message = ">0.0,0.0<";
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

    }
}
