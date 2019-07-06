using System.Collections.Generic;
using UnityEngine;

public abstract class Segment {
    protected Vector2d startPoint;
    protected Vector2d endPoint;

    public abstract Vector2d DerivativeAt(double t);
    public abstract Vector2d NormalAt(double t, int dir);
    public abstract Vector2d ValueAt(double t);
    public abstract double ApproxLength(int samples);

    public virtual Vector2d StartPoint {
        get {
            return startPoint;
        }
        set {
            startPoint = value;
        }
    }

    public virtual Vector2d EndPoint {
        get {
            return endPoint;
        }
        set {
            endPoint = value;
        }
    }

    public abstract List<Vector2d> getPointsList();
    public abstract Vector2d[] getPoints();
}