using System.Collections.Generic;
using UnityEngine;

public class PCirFsix : Segment
{
    private float radius;
    private Vector2 center;

    public float Radius
    {
        get
        {
            return radius;
        }

        set
        {
            radius = value;
        }
    }

    public Vector2 Center
    {
        get
        {
            return center;
        }

        set
        {
            center = value;
        }
    }

    public PCirFsix(float radius, Vector2 center){
        this.radius = radius;
        this.center = center;
    }

    public void ToCircle(List<Vector3> vertices, List<Vector4> uv, List<int> indices, float z){
        vertices.Add(new Vector3(center.x - radius, center.y - radius, z));
        vertices.Add(new Vector3(center.x - radius, center.y + radius, z));
        vertices.Add(new Vector3(center.x + radius, center.y - radius, z));

        uv.Add(new Vector4(-1, -1, 1, 4));
        uv.Add(new Vector4(-1, 1, 1, 4));
        uv.Add(new Vector4(1, -1, 1, 4));

        indices.Add(indices.Count);
        indices.Add(indices.Count);
        indices.Add(indices.Count);       

        vertices.Add(new Vector3(center.x - radius, center.y + radius, z));
        vertices.Add(new Vector3(center.x + radius, center.y + radius, z));
        vertices.Add(new Vector3(center.x + radius, center.y - radius, z));

        uv.Add(new Vector4(-1, 1, 1, 4));
        uv.Add(new Vector4(1, 1, 1, 4));
        uv.Add(new Vector4(1, -1, 1, 4));

        indices.Add(indices.Count);
        indices.Add(indices.Count);
        indices.Add(indices.Count);    
    }

    public override double ApproxLength(int samples)
    {
        throw new System.NotImplementedException();
    }

    public override Vector2d DerivativeAt(double t)
    {
        throw new System.NotImplementedException();
    }

    public override Vector2d[] getPoints()
    {
        throw new System.NotImplementedException();
    }

    public override List<Vector2d> getPointsList()
    {
        throw new System.NotImplementedException();
    }

    public override Vector2d NormalAt(double t, int dir)
    {
        throw new System.NotImplementedException();
    }

    public override void reverse()
    {
        throw new System.NotImplementedException();
    }

    public override Vector2d ValueAt(double t)
    {
        throw new System.NotImplementedException();
    }
}