using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PQuadratic : Segment {
    private Vector2d ctrl1 = new Vector2d();
    public Vector2d Ctrl1 {
        get { return ctrl1; }
        set { ctrl1 = value; }
    }

    public PQuadratic(Vector2d ctrl1, Vector2d endPoint) {
        this.ctrl1 = ctrl1;
        this.endPoint = endPoint;
    }

    public PQuadratic(Vector2d startPoint, Vector2d ctrl1, Vector2d endPoint) {
        if (this.startPoint == null)
            this.startPoint = startPoint;
        this.ctrl1 = ctrl1;
        this.endPoint = endPoint;
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
}
