using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SystemStroke : EntitySystem, EntityListener {
    private List<Shape> shapes;
    public override void addedToEngine(Engine engine) {
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
                ProcessShape(shape);
            }
        }
    }

    Vector3 v1 = new Vector3();
    Vector3 v2 = new Vector3();
    Vector3 v3 = new Vector3();
    Vector3 v4 = new Vector3();
    int[] idx = new int[6];

    private void ProcessShape(Shape shape) {
        CompFill comp = shape.getComponent<CompFill>(typeof(CompFill));
        List<Vector3> vertices = new List<Vector3>();
        List<Vector4> uv = new List<Vector4>();
        List<int> indices = new List<int>();
        vertices.Clear();
        uv.Clear();
        indices.Clear();

        BBoxResult res = shape.segments.Count>0 ? PathUtils.bbox(shape.segments[0]) : null;

        foreach (Segment seg in shape.segments) {
            if(seg.GetType() == typeof(PCubic)) {
                PathUtils.ComputeCubic((PCubic)seg, vertices, uv, indices);
            } else if (seg.GetType() == typeof(PQuadratic)) {
                PathUtils.ComputeQuadratic((PQuadratic)seg, vertices, uv, indices);
            } else if (seg.GetType() == typeof(PArc)) {
                PathUtils.ComputeArc((PArc)seg, vertices, uv, indices);
            }
            BBoxResult r = PathUtils.bbox(seg);
                if(r.x.max > res.x.max) res.x.max = r.x.max;
                if(r.y.max > res.y.max) res.y.max = r.y.max;
                if(r.x.min < res.x.min) res.x.min = r.x.min;
                if(r.y.min < res.y.min) res.y.min = r.y.min;
        }

        FillShape(shape, vertices, uv, indices);

        if(res != null){
            vertices.Add(new Vector3((float)res.x.min, (float)res.y.min, 1.1f));
            vertices.Add(new Vector3((float)res.x.max, (float)res.y.max, 1.1f));
            vertices.Add(new Vector3((float)res.x.min, (float)res.y.max, 1.1f));

            uv.Add(new Vector4(1, 1, 1, 5));
            uv.Add(new Vector4(1, 1, 1, 5));
            uv.Add(new Vector4(1, 1, 1, 5));

            indices.Add(indices.Count);
            indices.Add(indices.Count);
            indices.Add(indices.Count);       

            vertices.Add(new Vector3((float)res.x.min, (float)res.y.min, 1.1f));
            vertices.Add(new Vector3((float)res.x.max, (float)res.y.min, 1.1f));
            vertices.Add(new Vector3((float)res.x.max, (float)res.y.max, 1.1f));

            uv.Add(new Vector4(1, 1, 1, 5));
            uv.Add(new Vector4(1, 1, 1, 5));
            uv.Add(new Vector4(1, 1, 1, 5));

            indices.Add(indices.Count);
            indices.Add(indices.Count);
            indices.Add(indices.Count);    
        }

        shape.meshFill.Clear();
        shape.meshFill.subMeshCount = 2;
        Debug.Log("verts: " + vertices.Count);
        Debug.Log("uv: " + uv.Count);
        shape.meshFill.SetVertices(vertices);
        shape.meshFill.SetUVs(0, uv);
        shape.meshFill.SetTriangles(indices.ToArray(), 0);
        //indices.Clear();
        shape.meshFill.SetTriangles(indices.ToArray(), 1);

        shape.UpdateFill = false;
    }

    private void FillShape(Shape shape, List<Vector3> vertices, List<Vector4> uv, List<int> indices) {
        if (shape.segments.Count > 1) {
            for(int i=0; i< shape.segments.Count-1; i++) {
                if(shape.segments[i].GetType() == typeof(PArc)) {
                    // AddLineVert(shape.segments[i].StartPoint.f3(), vertices, uv, indices);
                    // AddLineVert(new Vector3((float)((PArc)shape.segments[i]).CenterX, (float)((PArc)shape.segments[i]).CenterY, 1), vertices, uv, indices);
                    // AddLineVert(shape.segments[i].EndPoint.f3(), vertices, uv, indices);
                } else {
                    vertices.Add(shape.segments[0].StartPoint.f3());
                    vertices.Add(shape.segments[i].EndPoint.f3());
                    vertices.Add(shape.segments[i + 1].EndPoint.f3());

                    if(i == shape.segments.Count-2 && shape.segments.Count > 1){
                        uv.Add(new Vector4(1, 1, 1, 6));
                        uv.Add(new Vector4(1, 1, 1, 6));
                        uv.Add(new Vector4(1, 1, 1, 6));
                    }else if(shape.segments[i+1].GetType() == typeof(PLine)){
                        uv.Add(new Vector4(1, 1, 1, 6));
                        uv.Add(new Vector4(1, 1, 1, 6));
                        uv.Add(new Vector4(1, 1, 1, 6));
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
    
