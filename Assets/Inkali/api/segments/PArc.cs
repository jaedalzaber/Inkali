using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PArc : Segment {
    public enum SweepDirection {
        CLOCKWISE, ANTI_CLOCKWISE
    }
    private double radiusX;
    private double radiusY;
    private double rotation;
    private bool isLargeArc;
    private SweepDirection direction;

    // don't use!
    private double centerX;
    private double centerY;
    private double angleStart;
    private double angleExtent;

    public PArc(Vector3d start, Vector3d end, double radiusX, double radiusY, double rotation, bool isLargeArc, SweepDirection direction) {
        startPoint = start;
        endPoint = end;
        this.radiusX = radiusX;
        this.radiusY = radiusY;
        this.rotation = rotation;
        this.isLargeArc = isLargeArc;
        this.direction = direction;
        computeArc();
    }

    public PArc(Vector3d end, double radiusX, double radiusY, double rotation, bool isLargeArc, SweepDirection direction) {
        endPoint = end;
        this.radiusX = radiusX;
        this.radiusY = radiusY;
        this.rotation = rotation;
        this.isLargeArc = isLargeArc;
        this.direction = direction;
        computeArc();
    }

    public double RadiusX {
        get {
            return radiusX;
        }
        set {
            radiusX = value;
            computeArc();
        }
    }
    public double RadiusY {
        get {
            return radiusY;
        }
        set {
            radiusY = value;
            computeArc();
        }
    }
    public double Rotation {
        get {
            return rotation;
        }
        set {
            rotation = value;
            computeArc();
        }
    }
    public bool IsLargeArc {
        get {
            return isLargeArc;
        }
        set {
            isLargeArc = value;
            computeArc();
        }
    }
    public SweepDirection Direction {
        get {
            return direction;
        }
        set {
            direction = value;
            computeArc();
        }
    }

    public double CenterX { get { return centerX; } }
    public double CenterY { get { return centerY; } }
    public double AngleStart { get { return angleStart; } }
    public double AngleExtent { get { return angleExtent; } }

    private Vector2d tmp = new Vector2d(), tmp2 = new Vector2d();
    public override double ApproxLength(int samples) {
        double tempLength = 0;
        for (int i = 0; i < samples; ++i) {
            tmp.Set(tmp2.x, tmp2.y);
            tmp2 = ValueAt((i) / ((double)samples - 1));
            if (i > 0) tempLength += Vector2d.Distance(tmp,tmp2);
        }
        return tempLength;
    }

    public override Vector2d DerivativeAt(double t) {
        t = Mathd.Clamp(t, 0.0, 1.0);
        double angle = angleStart + angleExtent * t;
        double dx = -Mathd.Cos(rotation) * radiusX * Mathd.Sin(angle) - Mathd.Sin(rotation) * radiusY * Mathd.Cos(angle);
        double dy = -Mathd.Sin(rotation) * radiusX * Mathd.Sin(angle) + Mathd.Cos(rotation) * radiusY * Mathd.Cos(angle);
        return new Vector2d(dx, dy);
    }

    public override Vector2d NormalAt(double t, int dir) {
        t = Mathd.Clamp(t, 0.0, 1.0);
        Vector2d n = DerivativeAt(t);
        if (dir < 0) n.Set(-n.y, n.x);
        if (dir >= 0) n.Set(n.y, -n.x);
        n.Normalize();
        return n;
    }

    public override Vector2d ValueAt(double t) {
        t = Mathd.Clamp(t, 0.0, 1.0);
        double angle = angleStart + angleExtent * t;
        //Fix me: convert from radians to degree
        double x = Mathd.Cos(rotation * Mathd.Deg2Rad) * radiusX * Mathd.Cos(angle * Mathd.Deg2Rad) - Mathd.Sin(rotation * Mathd.Deg2Rad) * radiusY * Mathd.Sin(angle * Mathd.Deg2Rad) + centerX;
        double y = Mathd.Sin(rotation * Mathd.Deg2Rad) * radiusX * Mathd.Cos(angle * Mathd.Deg2Rad) + Mathd.Cos(rotation * Mathd.Deg2Rad) * radiusY * Mathd.Sin(angle * Mathd.Deg2Rad) + centerY;
        return new Vector2d(x, y);
    }

    public void computeArc() {
        if (Vector3d.Magnitude(endPoint - startPoint) == 0)
            endPoint.Add(0,.000001);
        double x0 = StartPoint.x;
        double y0 = StartPoint.x;
        double rx = radiusX;
        double ry = radiusY;
        double angle = rotation;
        bool largeArcFlag = isLargeArc;
        bool sweepFlag = (direction == SweepDirection.CLOCKWISE) ? false : true;
        double x = EndPoint.x;
        double y = EndPoint.y;
        // Compute the half distance between the current and the final point
        double dx2 = (x0 - x) / 2.0;
        double dy2 = (y0 - y) / 2.0;
        // Convert angle from degrees to radians
        angle = Mathd.Deg2Rad * (angle % 360.0);
        double cosAngle = Mathd.Cos(angle);
        double sinAngle = Mathd.Sin(angle);

        //
        // Step 1 : Compute (x1, y1)
        //
        double x1 = (cosAngle * dx2 + sinAngle * dy2);
        double y1 = (-sinAngle * dx2 + cosAngle * dy2);
        // Ensure radii are large enough
        rx = Mathd.Abs(rx);
        ry = Mathd.Abs(ry);
        double Prx = rx * rx;
        double Pry = ry * ry;
        double Px1 = x1 * x1;
        double Py1 = y1 * y1;
        // check that radii are large enough
        double radiiCheck = Px1 / Prx + Py1 / Pry;
        if (radiiCheck > 0.99999) { // don't cut it too close
            double radiiScale = Mathd.Sqrt(radiiCheck) * 1.00001;
            rx = radiiScale * rx;
            ry = radiiScale * ry;
            Prx = rx * rx;
            Pry = ry * ry;
        }

        //
        // Step 2 : Compute (cx1, cy1)
        //
        double sign = (largeArcFlag == sweepFlag) ? -1 : 1;
        double sq = ((Prx * Pry) - (Prx * Py1) - (Pry * Px1)) / ((Prx * Py1) + (Pry * Px1));
        sq = (sq < 0) ? 0 : sq;
        double coef = (sign * Mathd.Sqrt(sq));
        double cx1 = coef * ((rx * y1) / ry);
        double cy1 = coef * -((ry * x1) / rx);

        //
        // Step 3 : Compute (cx, cy) from (cx1, cy1)
        //
        double sx2 = (x0 + x) / 2.0;
        double sy2 = (y0 + y) / 2.0;
        double cx = sx2 + (cosAngle * cx1 - sinAngle * cy1);
        double cy = sy2 + (sinAngle * cx1 + cosAngle * cy1);

        //
        // Step 4 : Compute the angleStart (angle1) and the angleExtent (dangle)
        //
        double ux = (x1 - cx1) / rx;
        double uy = (y1 - cy1) / ry;
        double vx = (-x1 - cx1) / rx;
        double vy = (-y1 - cy1) / ry;
        double p, n;
        // Compute the angle start
        n = Mathd.Sqrt((ux * ux) + (uy * uy));
        p = ux; // (1 * ux) + (0 * uy)
        sign = (uy < 0) ? -1.0 : 1.0;
        double angleStart = Mathd.Rad2Deg * (sign * Mathd.Acos(p / n));

        // Compute the angle extent
        n = Mathd.Sqrt((ux * ux + uy * uy) * (vx * vx + vy * vy));
        p = ux * vx + uy * vy;
        sign = (ux * vy - uy * vx < 0) ? -1.0 : 1.0;
        double angleExtent = Mathd.Rad2Deg * (sign * Mathd.Acos(p / n));
        if (!sweepFlag && angleExtent > 0) {
            angleExtent -= 360f;
        } else if (sweepFlag && angleExtent < 0) {
            angleExtent += 360f;
        }
        angleExtent %= 360f;
        angleStart %= 360f;

        centerX = cx;
        centerY = cy;
        this.angleStart = angleStart;
        this.angleExtent = angleExtent;

    }

    public override List<Vector2d> getPointsList()
    {
        return new List<Vector2d>(new Vector2d[]{this.startPoint, this.endPoint});
    }

    public override Vector2d[] getPoints()
    {
        return new Vector2d[]{this.startPoint, this.endPoint};
    }
}
