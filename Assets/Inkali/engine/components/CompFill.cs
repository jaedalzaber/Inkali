using System.Collections.Generic;
using UnityEngine;

public class CompFill : Component{
    public List<Vector3> vertices = new List<Vector3>();
    public List<Vector3> uv = new List<Vector3>();
    public List<int> indices = new List<int>();

    public float fillOpacity = 1;
    public FillRule fillrule = FillRule.EVEN_ODD;
    public Paint fill = new PaintSolid(Color.black);
}