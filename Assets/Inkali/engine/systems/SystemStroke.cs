using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SystemStroke : EntitySystem, EntityListener {
    private List<Shape> shapes;
    public override void addedToEngine(Engine engine) {
        vertices = new List<Vector3>();
        uv = new List<Vector4>();
        indices = new List<int>();
        shapes = engine.getEntitiesFor(Family.all(typeof(CompStroke)).get()).toList().Cast<Shape>().ToList();
    }

    public void entityAdded(Entity entity) {
        if (entity.GetType().BaseType == typeof(Shape))
            shapes.Add((Shape)entity);
    }

    public void entityRemoved(Entity entity) {
        if (entity.GetType().BaseType == typeof(Shape))
            shapes.Remove((Shape)entity);
    }

    public override void removedFromEngine(Engine engine) {
        //entities = engine.getEntitiesFor(Family.all(typeof(CompFill)).get());
    }

    public override void update(float deltaTime) {
        foreach(Shape shape in shapes) {
            if (shape.UpdateStroke) {
                ProcessStroke(shape);
            }
        }
    }

    Vector3 v1 = new Vector3();
    Vector3 v2 = new Vector3();
    Vector3 v3 = new Vector3();
    Vector3 v4 = new Vector3();
    int[] idx = new int[6];
    private List<Vector3> vertices;
    private List<Vector4> uv;
    private List<int> indices;
    private BBoxResult res;

    private void ProcessStroke(Shape shape){
        CompStroke comp = shape.getComponent<CompStroke>(typeof(CompStroke));
        List<Segment> segs = shape.GetSegmentsCopy();
        vertices.Clear();
        uv.Clear();
        indices.Clear();
        
        res = shape.segments.Count>0 ? PathUtils.bbox(shape.segments[0]) : null;
        if(shape.Closed){
            PLine l = new PLine(shape.segments.Last().EndPoint, shape.startPoint);
            Vector2d v0 = l.StartPoint + l.NormalAt(0, 1) * comp.strokeWidth;
            Vector2d v1 = l.EndPoint     + l.NormalAt(1, 1) * comp.strokeWidth;
            Vector2d v2 = l.EndPoint + l.NormalAt(1, -1) * comp.strokeWidth;
            Vector2d v3 = l.StartPoint + l.NormalAt(0, -1) * comp.strokeWidth;
            PQuad q = new PQuad(v3, v0, v1, v2);
            segs.Add(l);
        }

        Segment prev = new PLine(Vector2d.zero);
        Segment prevSeg = new PLine(Vector2d.zero);
        foreach (Segment seg in segs) {
            Path stroke = new Path();
            List<List<Segment>> list =  PathUtils.outline(seg, shape, comp.strokeWidth);
            List<Segment> s = list[0];
            List<Segment> j = list[1];

            // Add line join - ROUND, MITER, BEVEL
            switch(comp.linejoin){
                case LineJoin.ROUND:
                if(shape.segments.IndexOf(seg) == 0)
                    s.Add(new PCirFsix((float)comp.strokeWidth, seg.StartPoint.f()));
                s.Add(new PCirFsix((float)comp.strokeWidth, seg.EndPoint.f()));
                break;

                case LineJoin.MILTER:
                if(shape.segments.IndexOf(seg) == 0){
                    if(shape.Closed){
                        Segment l = segs.Last();
                        Vector2d l2 = l.EndPoint     + l.NormalAt(1, 1) * comp.strokeWidth;
                        Vector2d l3 = l.EndPoint + l.NormalAt(1, -1) * comp.strokeWidth;

                        Segment first = segs.First();
                        Segment last = segs.Last();

                        Vector2d s0 = first.StartPoint + first.NormalAt(0, -1) * comp.strokeWidth;
                        Vector2d s1 = first.StartPoint + first.NormalAt(0, 1) * comp.strokeWidth;

                        // PQuad q1 = new PQuad(l2, s1, s0, l3);
                        // q1.ToQuad(vertices, uv, indices, shape.Depth-.2f);

                        Vector2d s0d = s0 + seg.DerivativeAt(0);
                        Vector2d s1d = s1 + seg.DerivativeAt(0);
                        Vector2d l2d = l2 + l.DerivativeAt(1);
                        Vector2d l3d = l3 + l.DerivativeAt(1);

                        // Find intersection points 
                        Vector2d I1 = PathUtils.FindIntersection(l2d, l2, s1d, s1);
                        Vector2d I2 = PathUtils.FindIntersection(s0d, s0, l3d, l3);
                        Vector2d c = PathUtils.FindIntersection(s0, s1, l3, l2);

                        if((1/Mathd.Sin(Mathd.Abs(PathUtils.Angle(l2, c, s1))/2)) < comp.milterLimit){
                            PQuad Q1 = new PQuad(l2, I1, s1, c);
                            if(Q1.Clockwise()){
                                Q1.ToQuad(vertices, uv, indices, shape.Depth-.2f);
                                bboxQuad(Q1, res);
                            }
                            PQuad Q2 = new PQuad(I2, l3, c, s0);
                            if(Q2.Clockwise()){
                                Q2.ToQuad(vertices, uv, indices, shape.Depth-.2f);
                                bboxQuad(Q2, res);
                            }
                            prev = j[0];
                            prevSeg = seg;
                            break;

                        }
                        goto case LineJoin.BEVEL;
                    }   
                    prev = j[0];
                    prevSeg = seg;
                    break;
                }

/*                            E2d     E2          i1
                ───────────────◯────◯         ⚫
                                      |
                                      |
                                      |
                                      | C          
                    S0  ◯───────────⚫─────────◯  S1
                        |             |          |
                   S0d  ◯            |         ◯  S1d
                        |             |          |
                        |             |          |
                        |             |          |
                ────────⚫──────◯───◯          |
                     i2 |       E3d   E3         |
                        |                        |
                        |                        |
*/ 

                Vector2d S0 = j[0].getPoints()[0];
                Vector2d S1 = j[0].getPoints()[1];
                Vector2d E2 = prev.getPoints()[2];
                Vector2d E3 = prev.getPoints()[3];

                Vector2d S0d = S0 + seg.DerivativeAt(0);
                Vector2d S1d = S1 + seg.DerivativeAt(0);
                Vector2d E2d = E2 + prevSeg.DerivativeAt(1);
                Vector2d E3d = E3 + prevSeg.DerivativeAt(1);

                // Find intersection points 
                Vector2d i1 = PathUtils.FindIntersection(E2d, E2, S1d, S1);
                Vector2d i2 = PathUtils.FindIntersection(S0d, S0, E3d, E3);
                Vector2d C = PathUtils.FindIntersection(S0, S1, E3, E2);

                if((1/Mathd.Sin(Mathd.Abs(PathUtils.Angle(E2, C, S1))/2)) < comp.milterLimit){
                    PQuad Q1 = new PQuad(E2, i1, S1, C);
                    if(Q1.Clockwise()){
                        Q1.ToQuad(vertices, uv, indices, shape.Depth-.2f);
                        bboxQuad(Q1, res);
                    }
                    PQuad Q2 = new PQuad(i2, E3, C, S0);
                    if(Q2.Clockwise()){
                        Q2.ToQuad(vertices, uv, indices, shape.Depth-.2f);
                        bboxQuad(Q2, res);
                    }
                    prev = j[0];
                    prevSeg = seg;
                    break;

                }
                goto case LineJoin.BEVEL;

                case LineJoin.BEVEL:
                if(shape.segments.IndexOf(seg) == 0){
                    if(shape.Closed){
                        Segment l = segs.Last();
                        Vector2d l2 = l.EndPoint     + l.NormalAt(1, 1) * comp.strokeWidth;
                        Vector2d l3 = l.EndPoint + l.NormalAt(1, -1) * comp.strokeWidth;

                        Segment first = segs.First();
                        Segment last = segs.Last();

                        Vector2d s0 = first.StartPoint + first.NormalAt(0, -1) * comp.strokeWidth;
                        Vector2d s1 = first.StartPoint + first.NormalAt(0, 1) * comp.strokeWidth;

                        PQuad q1 = new PQuad(l2, s1, s0, l3);
                        q1.ToQuad(vertices, uv, indices, shape.Depth-.2f);
                    }   
                    prev = j[0];
                    prevSeg = seg;
                    break;
                }
                PQuad q = new PQuad(prev.getPoints()[2], j[0].getPoints()[1], j[0].getPoints()[0], prev.getPoints()[3]);
                q.ToQuad(vertices, uv, indices, shape.Depth-.2f);

                prev = j[0];
                prevSeg = seg;
                break;
            }

            foreach (Segment ss in s){
                if(ss.GetType() == typeof(PQuad)){
                    ((PQuad)ss).ToQuad(vertices, uv, indices, shape.Depth-.2f);
                    bboxQuad((PQuad)ss, res);
                } else if(ss.GetType() == typeof(PCirFsix)){
                    ((PCirFsix)ss).ToCircle(vertices, uv, indices, shape.Depth-.1f);
                    stroke.AddSeperate(ss);
                } 
                else
                    stroke.AddSeperate(ss);
            }
            
            ProcessShape(stroke, shape.Depth);
        }
        
        // Bounding Box Stencil - Two trinagles
        if(res != null){
            vertices.Add(new Vector3((float)res.x.min, (float)res.y.min, shape.Depth-.1f));
            vertices.Add(new Vector3((float)res.x.max, (float)res.y.max, shape.Depth-.1f));
            vertices.Add(new Vector3((float)res.x.min, (float)res.y.max, shape.Depth-.1f));

            uv.Add(new Vector4(1, 1, 1, 5));
            uv.Add(new Vector4(1, 1, 1, 5));
            uv.Add(new Vector4(1, 1, 1, 5));

            indices.Add(indices.Count);
            indices.Add(indices.Count);
            indices.Add(indices.Count);       

            vertices.Add(new Vector3((float)res.x.min, (float)res.y.min, shape.Depth-.1f));
            vertices.Add(new Vector3((float)res.x.max, (float)res.y.min, shape.Depth-.1f));
            vertices.Add(new Vector3((float)res.x.max, (float)res.y.max, shape.Depth-.1f));

            uv.Add(new Vector4(1, 1, 1, 5));
            uv.Add(new Vector4(1, 1, 1, 5));
            uv.Add(new Vector4(1, 1, 1, 5));

            indices.Add(indices.Count);
            indices.Add(indices.Count);
            indices.Add(indices.Count);    
        }

        // Add all calculated data in Mesh data of shape
        shape.meshStroke.Clear();
        shape.meshStroke.subMeshCount = 2;
        shape.meshStroke.SetVertices(vertices);
        shape.meshStroke.SetUVs(0, uv);
        shape.meshStroke.SetTriangles(indices.ToArray(), 0);
        shape.meshStroke.SetTriangles(indices.ToArray(), 1);

        shape.UpdateStroke = false;
    }

    private void ProcessShape(Shape shape, float z) {
        foreach (Segment seg in shape.segments) {
            if(seg.GetType() == typeof(PCubic)) {
                PathUtils.ComputeCubic((PCubic)seg, vertices, uv, indices, z-.1f);
            } else if (seg.GetType() == typeof(PQuadratic)) {
                PathUtils.ComputeQuadratic((PQuadratic)seg, vertices, uv, indices, z-.1f);
            } else if (seg.GetType() == typeof(PArc)) {
                PathUtils.ComputeArc((PArc)seg, vertices, uv, indices);
            } 
           
            BBoxResult r = PathUtils.bbox(seg);
            if(r.x.max > res.x.max) res.x.max = r.x.max;
            if(r.y.max > res.y.max) res.y.max = r.y.max;
            if(r.x.min < res.x.min) res.x.min = r.x.min;
            if(r.y.min < res.y.min) res.y.min = r.y.min;
            // if(seg.GetType() != typeof(PCirFsix)){
                
            // }
        }

    //    FillShape(shape, vertices, uv, indices);
    }

    private void bboxQuad(PQuad q, BBoxResult res){
        foreach (Vector2d v in q.getPoints()) {
            if(v.x > res.x.max) res.x.max = v.x;
            if(v.y > res.y.max) res.y.max = v.y;
            if(v.x < res.x.min) res.x.min = v.x;
            if(v.y < res.y.min) res.y.min = v.y;
        }
    }

    private void FillShape(Shape shape, List<Vector3> vertices, List<Vector4> uv, /* List<Vector2> uv2,*/ List<int> indices) {
        if (shape.segments.Count > 1) {
            for(int i=shape.segments.Count-2; i> -1; i--) {
                if(shape.segments[i].GetType() == typeof(PArc)) {
                    // AddLineVert(shape.segments[i].StartPoint.f3(), vertices, uv, indices);
                    // AddLineVert(new Vector3((float)((PArc)shape.segments[i]).CenterX, (float)((PArc)shape.segments[i]).CenterY, 1), vertices, uv, indices);
                    // AddLineVert(shape.segments[i].EndPoint.f3(), vertices, uv, indices);
                } else {
                    vertices.Add(shape.segments[0].StartPoint.f3(shape.Depth));
                    vertices.Add(shape.segments[i].EndPoint.f3(shape.Depth));
                    vertices.Add(shape.segments[i + 1].EndPoint.f3(shape.Depth));

                    if(i == shape.segments.Count-2 && shape.segments.Count > 1){
                        uv.Add(new Vector4(1, 0, 1, 6));
                        uv.Add(new Vector4(1, 1, 1, 6));
                        uv.Add(new Vector4(1, 0, 1, 6));
                    }else if(shape.segments[i+1].GetType() == typeof(PLine)){
                        uv.Add(new Vector4(1, 1, 1, 6));
                        uv.Add(new Vector4(1, 0, 1, 6));
                        uv.Add(new Vector4(1, 0, 1, 6));
                    }else {
                        uv.Add(new Vector4(1, 1, 1, 0));
                        uv.Add(new Vector4(1, 1, 1, 0));
                        uv.Add(new Vector4(1, 1, 1, 0));
                    }

                    indices.Add(indices.Count);
                    indices.Add(indices.Count);
                    indices.Add(indices.Count);
                }


                //shape.segments[0].StartPoint.print();
                //shape.segments[i].EndPoint.print();
                //shape.segments[i+1].EndPoint.print();
            }
        } 
    }

    private void AddLineVert(Vector3 v, List<Vector3> vertices, List<Vector4> uv, List<int> indices) {
        vertices.Add(v);
        uv.Add(new Vector4(1, 1, 1, 0));
        indices.Add(indices.Count);
    }
}
    
