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
        vertices.Clear();
        uv.Clear();
        indices.Clear();
        
        res = shape.segments.Count>0 ? PathUtils.bbox(shape.segments[0]) : null;
        foreach (Segment seg in shape.segments) {
            Path stroke = new Path();
            List<Segment> s = PathUtils.outline(seg, comp.strokeWidth);
            foreach (Segment ss in s){
                if(ss.GetType() == typeof(PQuad)){
                    ((PQuad)ss).ToQuad(vertices, uv, indices, shape.Depth-.2f);
                } else if(ss.GetType() == typeof(PCirFsix)){
                    ((PCirFsix)ss).ToCircle(vertices, uv, indices, shape.Depth-.1f);
                } 
                else
                    stroke.AddSeperate(ss);
            }
            ProcessShape(stroke, shape.Depth);
        }
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
                PathUtils.ComputeQuadratic((PQuadratic)seg, vertices, uv, indices);
            } else if (seg.GetType() == typeof(PArc)) {
                PathUtils.ComputeArc((PArc)seg, vertices, uv, indices);
            }
           
            
            if(seg.GetType() != typeof(PCirFsix)){
                BBoxResult r = PathUtils.bbox(seg);
                if(r.x.max > res.x.max) res.x.max = r.x.max;
                if(r.y.max > res.y.max) res.y.max = r.y.max;
                if(r.x.min < res.x.min) res.x.min = r.x.min;
                if(r.y.min < res.y.min) res.y.min = r.y.min;
            }
        }

    //    FillShape(shape, vertices, uv, indices);
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
    
