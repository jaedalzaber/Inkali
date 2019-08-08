using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PQuadratic : Segment {
    private Vector2d ctrl1 = new Vector2d();
    private Vector2d[] points = new Vector2d[3]; 

    public override Vector2d StartPoint {
        get {
            return base.StartPoint;
        }

        set {
            points[0] = value;
            base.StartPoint = value;
        }
    }

    public Vector2d Ctrl1 {
        get { return ctrl1; }
        set { 
            points[1] = value;
            ctrl1 = value; 
        }
    }

    public override Vector2d EndPoint {
        get {
            return base.EndPoint;
        }

        set {
            points[2] = value;
            base.EndPoint = value;
        }
    }

    public PQuadratic(Vector2d ctrl1, Vector2d endPoint) {
        this.ctrl1 = ctrl1;
        this.endPoint = endPoint;
        points[0] = this.startPoint;
        points[1] = this.ctrl1;
        points[2] = this.endPoint;
    }

    public PQuadratic(Vector2d startPoint, Vector2d ctrl1, Vector2d endPoint) {
        // if (this.startPoint == null)
        this.startPoint = startPoint;
        this.ctrl1 = ctrl1;
        this.endPoint = endPoint;
        points[0] = this.startPoint;
        points[1] = this.ctrl1;
        points[2] = this.endPoint;
    }

    public PQuadratic(List<Vector2d> np) {
        this.startPoint = np[0];
        this.ctrl1 = np[1];
        this.endPoint = np[2];
        points[0] = this.startPoint;
        points[1] = this.ctrl1;
        points[2] = this.endPoint;
    }

    public PQuadratic(Vector2d[] np) {
        this.startPoint = np[0];
        this.ctrl1 = np[1];
        this.endPoint = np[2];
        points[0] = this.startPoint;
        points[1] = this.ctrl1;
        points[2] = this.endPoint;
    }

    private Vector2d tmp = new Vector2d(), tmp2 = new Vector2d();
    public override double ApproxLength(int samples) {
        double tempLength = 0;
        for (int i = 0; i < samples; ++i) {
            tmp.Set(tmp2.x, tmp2.y);
            tmp2 = ValueAt((i) / ((double)samples - 1));
            if (i > 0) tempLength += Vector2d.Distance(tmp, tmp2);
        }
        return tempLength;
    }

    public override Vector2d DerivativeAt(double t) {
        return (ctrl1 - startPoint) * 2 * (1 - t) + (endPoint- ctrl1) * t * 2;
    }

    public override Vector2d NormalAt(double t, int dir) {
        Vector2d n = DerivativeAt(t);
        if (dir < 0) n.Set(n.y, -n.x);
        if (dir >= 0) n.Set(-n.y, n.x);
        n.Normalize();
        return n;
    }

    public override Vector2d ValueAt(double t) {
        double dt = 1f - t;
        return startPoint * (dt * dt)+ ctrl1 * (2 * dt * t) + endPoint * (t * t);
    }

    public override List<Vector2d> getPointsList()
    {
        return new List<Vector2d>(points);
    }

    public override Vector2d[] getPoints()
    {
        return points;
    }

    public override void reverse() {
        Vector2d tmp = new Vector2d(startPoint);
        startPoint = endPoint;
        endPoint = tmp;
        points[0] = this.startPoint;
        points[1] = this.ctrl1;
        points[2] = this.endPoint;
    }
}
