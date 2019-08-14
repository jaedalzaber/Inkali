using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Shape : Entity {
    public Mesh meshFill;
    public Mesh meshStroke;
    public GameObject obj;
    public GameObject objFill;
    public GameObject objStroke;

    protected MeshRenderer fillRenderer;
    protected MeshRenderer strokeRenderer;
    private static int queue = 3002;

    protected float strokeWidth = 0;
    protected float strokeOpacity = 1;
    protected float[] strokeDashArray = null;
    protected LineCap linecap = LineCap.FLAT;
    protected LineJoin linejoin = LineJoin.MILTER;
    protected float milterLimit = 4;
    private bool closed = true;

    protected float fillOpacity = 3001;
    protected FillRule fillrule = FillRule.EVEN_ODD;
    protected Paint fill;
    private bool updateFillPaint;
    protected Paint stroke = new PaintSolid(Color.magenta);
    private bool updateStrokePaint;

    public List<Segment> segments;
    public Vector2d startPoint;
    protected Vector2d endPoint;

    private float depth;

    public Shape() {
        segments = new List<Segment>();
        CompFill comp = new CompFill();
        fill = comp.fill;
        CompStroke compStroke = new CompStroke();
        stroke = compStroke.stroke;
        add(comp);
        add(compStroke);
    }

    void Start() {
        obj = gameObject;
        objFill = new GameObject(obj.name + "_fill");
        objFill.transform.parent = obj.transform;
        objStroke = new GameObject(obj.name + "_stroke");
        objStroke.transform.parent = obj.transform;

        // UpdateStroke= false;

        // Create Fill
        MeshFilter meshFilterFill = objFill.AddComponent(typeof(MeshFilter)) as MeshFilter;
        fillRenderer = objFill.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        meshFill = meshFilterFill.mesh;
        meshFill.subMeshCount = 2;

        fillRenderer.materials = new Material[] { new Material(Shader.Find("Inkali/Stencil EvenOdd")), fill.CreateMaterialEO() };
        fillRenderer.materials[0].renderQueue = queue++;
        fillRenderer.materials[1].renderQueue = queue++;

        // Create Stroke
        MeshFilter meshFilterStroke = objStroke.AddComponent(typeof(MeshFilter)) as MeshFilter;
        strokeRenderer = objStroke.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        meshStroke = meshFilterStroke.mesh;

        strokeRenderer.materials = new Material[] { new Material(Shader.Find("Inkali/Stencil NonZero")), fill.CreateMaterialNZ() };
        strokeRenderer.materials[0].renderQueue = queue++;
        strokeRenderer.materials[1].renderQueue = queue++;

    }

    protected void Add(Segment segment) {
        if (segments.Count == 0)
            startPoint = segment.StartPoint;
        else
            segment.StartPoint = endPoint;
        segments.Add(segment);
        endPoint = segment.EndPoint;
        UpdateFill = true;
        UpdateStroke = true;
    }

    protected void AddSeperate(Segment segment) {
        if (segments.Count == 0)
            startPoint = segment.StartPoint;
        // else
        //     segment.StartPoint = endPoint;
        segments.Add(segment);
        endPoint = segment.EndPoint;
        UpdateFill = true;
        UpdateStroke = true;
    }

    protected void AddAll(params Segment[] segments) {
        foreach(Segment s in segments) {
            if (this.segments.Count == 0)
                startPoint = s.StartPoint;
            else
                s.StartPoint = endPoint;
            this.segments.Add(s);
            endPoint = s.EndPoint;
        }
        UpdateFill = true;
    }

    protected void AddAt(Segment segment, int index) {
        if (index == 0)
            startPoint = segment.StartPoint;
        else
            segment.StartPoint = segments[index-1].EndPoint;
        segments.Insert(index, segment);
        endPoint = segment.EndPoint;
        UpdateFill = true;
    }

 /*   protected void AddAfter(Segment segment, Segment after) {
        if (segments.Contains(after)) {
            int index = segments.IndexOf(after) + 1;
            segments.Insert(index, segment);
            if((index + 1) == segments.Count)
                endPoint = segment.EndPoint;
            UpdateFill = true;
        }
    }

    protected void AddBefore(Segment segment, Segment before) {
        if (segments.Contains(before)) {
            int index = segments.IndexOf(before);
            segments.Insert(index, segment);
            if (index == 0)
                startPoint = segment.StartPoint;
            UpdateFill = true;
        }
    }
    */
    protected void Remove(Segment segment) {
        segments.Remove(segment);
        UpdateFill = true;
    }

    protected void RemoveAt(int index) {
        segments.RemoveAt(index);
        UpdateFill = true;
    }

    public bool UpdateStroke { get; set; }
    public bool UpdateFill { get; set; }

    public float StrokeWidth {
        get { return strokeWidth; }
        set {
            float v = Mathf.Abs(value);
            if (v == 0)
                remove(typeof(CompStroke));
            else {
                CompStroke c = getComponent<CompStroke>(typeof(CompStroke));
                CompDash d = getComponent<CompDash>(typeof(CompDash));
                if (c == null) {
                    c = new CompStroke();
                    add(new CompStroke());
                }
                c.strokeWidth = v;
                c.linecap = linecap;
                c.linejoin = linejoin;
                c.milterLimit = milterLimit;
                c.strokeOpacity = strokeOpacity;
                c.stroke = stroke;
                if (strokeDashArray != null) {
                    if (d == null) { 
                        d = new CompDash();
                        add(d);
                    }
                d.strokeDashArray = strokeDashArray;
                }   
                
                UpdateStroke = true;
            }
            strokeWidth = v;
        }
    }
    public float StrokeOpacity {
        get { return strokeOpacity; }
        set {
            float v = Mathf.Clamp(value, 0.0f, 1.0f);
            CompStroke c = getComponent<CompStroke>(typeof(CompStroke));
            if (c != null) {
                c.strokeOpacity = v;
            }
            strokeOpacity = v;
        }
    }
    public float[] StrokeDashArray {
        get { return strokeDashArray; }
        set {
            CompStroke s = getComponent<CompStroke>(typeof(CompStroke));
            CompDash d = getComponent<CompDash>(typeof(CompDash));
            if (s != null) {
                if (d == null)
                    d = new CompDash();
                d.strokeDashArray = value;
                UpdateStroke = true;
            }
            strokeDashArray = value;
        }
    }
    public LineCap LineCap {
        get { return linecap; }
        set {
            CompStroke c = getComponent<CompStroke>(typeof(CompStroke));
            if (c != null) {
                c.linecap = value;
                UpdateStroke = true;
            }
            linecap = value;
        }
    }
    public LineJoin LineJoin {
        get { return linejoin; }
        set {
            CompStroke c = getComponent<CompStroke>(typeof(CompStroke));
            if (c != null) {
                c.linejoin = value;
                UpdateStroke = true;
            }
            linejoin = value;
        }
    }
    public float MilterLimit {
        get { return milterLimit; }
        set {
            CompStroke c = getComponent<CompStroke>(typeof(CompStroke));
            if (c != null) {
                c.milterLimit = value;
                UpdateStroke = true;
            }
            milterLimit = value;
        }
    }

    public float FillOpacity {
        get { return fillOpacity; }
        set {
            float v = Mathf.Clamp(value, 0.0f, 1.0f);
            CompFill f = getComponent<CompFill>(typeof(CompFill));
            f.fillOpacity = v;
            fillOpacity = v;
            fill.setOpacity(value);
        }
    }
    public FillRule FillRule {
        get { return fillrule; }
        set {
            CompFill f = getComponent<CompFill>(typeof(CompFill));
            f.fillrule = value;
            fillrule = value;
        }
    }
    public Paint FillPaint {
        get { return fill; }
        set {
            CompFill f = getComponent<CompFill>(typeof(CompFill));
            f.fill = value;
            fill = value;
            updateFillPaint = true;
        }
    }
    public Paint StrokePaint {
        get { return stroke; }
        set {
            // objStroke.GetComponent<MeshRenderer>().material = value.CreateMaterial();
            CompStroke s = getComponent<CompStroke>(typeof(CompStroke));
            if(s != null)
                s.stroke = value;
            stroke = value;
            updateStrokePaint = true;
        }
    }

    public float Depth
    {
        get
        {
            return depth;
        }

        set
        {
            depth = value;
            UpdateStroke = true;
            UpdateFill = true;
        }
    }

    public bool Closed
    {
        get
        {
            return closed;
        }

        set
        {
            closed = value;
        }
    }

    void Update() {
        if (updateFillPaint) {
            fill.CreateMaterialEO();
            fillRenderer.materials[1] = fill.CreateMaterialEO();
        }
        if (fill.update) {
            fill.Update(fillRenderer.materials[1]);
        }
        if (updateStrokePaint) {
            stroke.CreateMaterialEO();
            strokeRenderer.materials[1] = stroke.CreateMaterialEO();
        }
        if (stroke.update) {
            stroke.Update(strokeRenderer.materials[1]);
        }
    }
}
