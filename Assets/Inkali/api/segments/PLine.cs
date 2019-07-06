﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLine : Segment {
    public PLine(Vector2d endPoint) {
        this.endPoint = endPoint;
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
        throw new System.NotImplementedException();
    }

    public override Vector2d ValueAt(double t) {
        throw new System.NotImplementedException();
    }

    public override Vector2d[] getPoints()
    {
        return new Vector2d[]{this.startPoint, this.endPoint};
    }
}
