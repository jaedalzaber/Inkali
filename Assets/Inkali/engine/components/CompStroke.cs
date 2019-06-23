using System.Collections.Generic;
using UnityEngine;

public class CompStroke : Component{
    // All default values
    public List<Vector3> vertices;
    public List<Vector3> uv;
    public List<int> indices;

    public float strokeWidth = 1; // px
    public float strokeOpacity = 1;
    public LineCap linecap = LineCap.FLAT;
    public LineJoin linejoin = LineJoin.MILTER;
    public float milterLimit = 4;

    public Paint stroke;
}
