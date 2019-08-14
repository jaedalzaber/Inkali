using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLine : Segment {

    private Vector2d[] points = new Vector2d[2]; 

    public PLine(Vector2d endPoint) {
        this.endPoint = endPoint;
        points[0] = this.startPoint;
        points[1] = this.endPoint;
    }

    public PLine(Vector2d startPoint, Vector2d endPoint) {   
        this.startPoint = startPoint;
        this.endPoint = endPoint;
        points[0] = this.startPoint;
        points[1] = this.endPoint;
    }

    public override double ApproxLength(int samples) {
        throw new System.NotImplementedException();
    }

    public override Vector2d DerivativeAt(double t) {
        return endPoint - startPoint;
    }

    public override List<Vector2d> getPointsList()
    {
        return new List<Vector2d>(new Vector2d[]{this.startPoint, this.endPoint});
    }

    public override Vector2d NormalAt(double t, int dir) {
        Vector2d n = DerivativeAt(t);
		if(dir<0)n.Set(n.y, -n.x);
		if(dir >= 0)n.Set(-n.y, n.x);
		n.Normalize();
		return n;
    }

    public override Vector2d ValueAt(double t) {
        throw new System.NotImplementedException();
    }

    public override Vector2d[] getPoints()
    {
        return points;
    }

    public override void reverse()
    {
        Vector2d tmp = new Vector2d(startPoint);
        startPoint = endPoint;
        endPoint = tmp;
        points[0] = this.startPoint;
        points[1] = this.endPoint;
    }

    public override Vector2d StartPoint {
        get {
            return base.StartPoint;
        }

        set {
            points[0] = value;
            base.StartPoint = value;
        }
    }

    public override Vector2d EndPoint {
        get {
            return base.EndPoint;
        }

        set {
            points[1] = value;
            base.EndPoint = value;
        }
    }

}
