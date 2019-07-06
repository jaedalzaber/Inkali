using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCubic : Segment {
    private Vector2d ctrl1 = new Vector2d();
    private Vector2d ctrl2 = new Vector2d();
    public Vector2d[] points = new Vector2d[4];

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

    public Vector2d Ctrl2 {
        get { return ctrl2; }
        set {
            points[2] = value;
            ctrl2 = value;
        }
    }

    public override Vector2d EndPoint {
        get {
            return base.EndPoint;
        }

        set {
            points[3] = value;
            base.EndPoint = value;
        }
    }

    public PCubic(Vector2d ctrl1, Vector2d ctrl2, Vector2d endPoint) {
        this.ctrl1 = ctrl1;
        this.ctrl2 = ctrl2;
        this.endPoint = endPoint;
        points[0] = startPoint;
        points[1] = ctrl1;
        points[2] = ctrl2;
        points[3] = endPoint;
    }



    public PCubic(Vector2d startPoint, Vector2d ctrl1, Vector2d ctrl2, Vector2d endPoint) {
        this.startPoint = startPoint;
        this.ctrl1 = ctrl1;
        this.ctrl2 = ctrl2;
        this.endPoint = endPoint;
        points[0] = startPoint;
        points[1] = ctrl1;
        points[2] = ctrl2;
        points[3] = endPoint;
    }

    public PCubic(Vector2 startPoint, Vector2 ctrl1, Vector2 ctrl2, Vector2 endPoint) {
        this.startPoint = new Vector2d(startPoint);
        this.ctrl1 = new Vector2d(ctrl1);
        this.ctrl2 = new Vector2d(ctrl2);
        this.endPoint = new Vector2d(endPoint);
        points[0] = this.startPoint;
        points[1] = this.ctrl1;
        points[2] = this.ctrl2;
        points[3] = this.endPoint;
    }

    public void set(Vector2 startPoint, Vector2 ctrl1, Vector2 ctrl2, Vector2 endPoint) {
        this.startPoint = new Vector2d(startPoint);
        this.ctrl1 = new Vector2d(ctrl1);
        this.ctrl2 = new Vector2d(ctrl2);
        this.endPoint = new Vector2d(endPoint);
        points[0] = this.startPoint;
        points[1] = this.ctrl1;
        points[2] = this.ctrl2;
        points[3] = this.endPoint;
    }

    public void set(Vector2 ctrl1, Vector2 ctrl2, Vector2 endPoint) {
        this.ctrl1 = new Vector2d(ctrl1);
        this.ctrl2 = new Vector2d(ctrl2);
        this.endPoint = new Vector2d(endPoint);
        points[1] = this.ctrl1;
        points[2] = this.ctrl2;
        points[3] = this.endPoint;
    }

    public PCubic(double x1, double y1, double v1, double v2, double v3, double v4, double x2, double y2) {
        if (this.startPoint == null)
            this.startPoint = new Vector2d(x1, y1);
        this.ctrl1 = new Vector2d(v1, v2);
        this.ctrl2 = new Vector2d(v3, v4);
        this.endPoint = new Vector2d(x2, y2);
        points[0] = startPoint;
        points[1] = ctrl1;
        points[2] = ctrl2;
        points[3] = endPoint;
    }

    public PCubic(PCubic c) {
        if (this.startPoint == null)
            this.startPoint = c.startPoint;
        this.ctrl1 = c.ctrl1;
        this.ctrl2 = c.ctrl2;
        this.endPoint = c.endPoint;
        points[0] = c.startPoint;
        points[1] = c.ctrl1;
        points[2] = c.ctrl2;
        points[3] = c.endPoint;
    }

    public PCubic(List<Vector2d> np) {
        if (this.startPoint == null)
            this.startPoint = np[0];
        this.ctrl1 = np[1];
        this.ctrl2 = np[2];
        this.endPoint = np[3];
        points[0] = startPoint;
        points[1] = ctrl1;
        points[2] = ctrl2;
        points[3] = endPoint;
    }

    public PCubic(Vector2d[] np) {
        if (this.startPoint == null)
            this.startPoint = np[0];
        this.ctrl1 = np[1];
        this.ctrl2 = np[2];
        this.endPoint = np[3];
        points[0] = startPoint;
        points[1] = ctrl1;
        points[2] = ctrl2;
        points[3] = endPoint;
    }

    private Vector2d tmp = new Vector2d(), tmp2 = new Vector2d();
    private List<Vector2d> np;
    private Vector2d[] np1;

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
        double dt = 1f - t;
        double dt2 = dt * dt;
        double t2 = t * t;
        return (ctrl1-startPoint) * (dt2 * 3) + (ctrl2-ctrl1) * (dt * t * 6) + (endPoint-ctrl2) * (t2 * 3);
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
        double dt2 = dt * dt;
        double t2 = t * t;
        return startPoint * (dt2 * dt) + ctrl1 * (3 * dt2 * t) + ctrl2 * (3 * dt * t2) + endPoint * (t2 * t);
    }

    internal void reverse() {
        Vector2d tmp = new Vector2d(startPoint);
        startPoint = endPoint;
        endPoint = tmp;
        Vector2d tmp2 = ctrl1;
        ctrl1 = ctrl2;
        ctrl2 = tmp2;
        points[0] = this.startPoint;
        points[1] = this.ctrl1;
        points[2] = this.ctrl2;
        points[3] = this.endPoint;
    }

    public override List<Vector2d> getPointsList()
    {
        return new List<Vector2d>(points);
    }

    public override Vector2d[] getPoints()
    {
        return points;
    }
}
