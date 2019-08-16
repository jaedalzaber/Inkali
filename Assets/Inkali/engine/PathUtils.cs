using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PathUtils {
    private static Vector2 start = new Vector2();
    private static Vector2 ctrl1 = new Vector2();
    private static Vector2 ctrl2 = new Vector2();
    private static Vector2 end = new Vector2();
    private static float[] klm;
    private static Line l = new Line();

    private static List<Vector3> verts = new List<Vector3>();
    public enum CurveType{UNKNOWN, SERPENTINE, LOOP, CUSP, QUADRATIC, LINE};

    public static double epsilon = 0.000001;
    public static double[] Tvalues = new double[] { -0.0640568928626056260850430826247450385909f,
            0.0640568928626056260850430826247450385909f, -0.1911188674736163091586398207570696318404f,
            0.1911188674736163091586398207570696318404f, -0.3150426796961633743867932913198102407864f,
            0.3150426796961633743867932913198102407864f, -0.4337935076260451384870842319133497124524f,
            0.4337935076260451384870842319133497124524f, -0.5454214713888395356583756172183723700107f,
            0.5454214713888395356583756172183723700107f, -0.6480936519369755692524957869107476266696f,
            0.6480936519369755692524957869107476266696f, -0.7401241915785543642438281030999784255232f,
            0.7401241915785543642438281030999784255232f, -0.8200019859739029219539498726697452080761f,
            0.8200019859739029219539498726697452080761f, -0.8864155270044010342131543419821967550873f,
            0.8864155270044010342131543419821967550873f, -0.9382745520027327585236490017087214496548f,
            0.9382745520027327585236490017087214496548f, -0.9747285559713094981983919930081690617411f,
            0.9747285559713094981983919930081690617411f, -0.9951872199970213601799974097007368118745f,
            0.9951872199970213601799974097007368118745f };

    public static double[] Cvalues = new double[] { 0.1279381953467521569740561652246953718517f,
            0.1279381953467521569740561652246953718517f, 0.1258374563468282961213753825111836887264f,
            0.1258374563468282961213753825111836887264f, 0.121670472927803391204463153476262425607f,
            0.121670472927803391204463153476262425607f, 0.1155056680537256013533444839067835598622f,
            0.1155056680537256013533444839067835598622f, 0.1074442701159656347825773424466062227946f,
            0.1074442701159656347825773424466062227946f, 0.0976186521041138882698806644642471544279f,
            0.0976186521041138882698806644642471544279f, 0.086190161531953275917185202983742667185f,
            0.086190161531953275917185202983742667185f, 0.0733464814110803057340336152531165181193f,
            0.0733464814110803057340336152531165181193f, 0.0592985849154367807463677585001085845412f,
            0.0592985849154367807463677585001085845412f, 0.0442774388174198061686027482113382288593f,
            0.0442774388174198061686027482113382288593f, 0.0285313886289336631813078159518782864491f,
            0.0285313886289336631813078159518782864491f, 0.0123412297999871995468056670700372915759f,
            0.0123412297999871995468056670700372915759f };

    public class Line {
        public Vector2d p1, p2;

        public Line(Vector2d start, Vector2d end) {
            p1 = start;
            p2 = end;
        }

        public Line() {
            p1 = new Vector2d();
            p2 = new Vector2d();
        }
    }

    public static Vector2d ComputeCubic(PCubic cubic, List<Vector3> vertices, List<Vector4> uv, List<int> indices, float z) {
        start = cubic.StartPoint.f();
        ctrl1 = cubic.Ctrl1.f();
        ctrl2 = cubic.Ctrl2.f();
        end = cubic.EndPoint.f();

        switch (orientation(start, ctrl1, ctrl2, end)) {
            case 2:
                ctrl1.Set(ctrl1.x+.0001f, ctrl1.y+.0001f);
                break;
            case 3:
                ctrl2.Set(ctrl2.x + .0001f, ctrl2.y + .0001f);
                break;
            default:
                break;
        }
        verts.Clear();
        List<Vector3> v = new List<Vector3>();
        List<Vector4> u = new List<Vector4>();
        List<int> i = new List<int>();

        CurveType curve_type = compute(cubic, (float)start.x, (float)start.y, (float)ctrl1.x, (float)ctrl1.y, (float)ctrl2.x, (float)ctrl2.y, (float)end.x, (float)end.y, -1,
            v, u, i, z);

       int ol = orientationLoop(start, ctrl1, ctrl2, end);
        Vector2d point = new Vector2d();
        PCubic c = new PCubic(start, ctrl1, ctrl2, end);

        
        //if (curve_type == CurveType.LOOP && ol == -1 && verts.Count / 3 == 6) {
        //    l.p1 = new Vector2d(start);
        //    l.p2 = new Vector2d(verts[6].x, verts[6].y);
        //    l.p2 = new Vector2(4, 4);
        //    DoubleArray r = LineIntersects(c, l);
        //    float t = 0;
        //    for (int i = 0; i < r.size; i++) {
        //        if (r.get(i) > .0001f && r.get(i) < .9999f)
        //            t = (float)r.get(i);
        //    }
        //    point.Set(c.ValueAt(t));
        //    AddVertexPlain(new Vertex(verts[5]), verts, uv, indices);
        //    AddVertexPlain(new Vertex(point.f()), verts, uv, indices);
        //    AddVertexPlain(new Vertex(start), verts, uv, indices);
        //} else if (curve_type == CurveType.LOOP && ol == 1 && verts.Count / 3 == 6) {
        //    l.p1 = new Vector2d(end);
        //    l.p2 = new Vector2d(verts[12].x, verts[12].y);
        //    DoubleArray r = LineIntersects(c, l);
        //    float t = 0;
        //    for (int i = 0; i < r.size; i++) {
        //        if (r.get(i) > .1f && r.get(i) < .9f)
        //            t = (float)r.get(i);
        //    }
        //    point.Set(c.ValueAt(t));
        //    AddVertexPlain(new Vertex(verts[14]), verts, uv, indices);
        //    AddVertexPlain(new Vertex(point.f()), verts, uv, indices);
        //    AddVertexPlain(new Vertex(end), verts, uv, indices);
        //}

        // vertices.AddRange(verts);


        // Make all trinagles front facing
        for(int j=0; j<v.Count; j+=3){
            double angle = Angle(v[j], v[j+1], v[j+2]);
            if((angle > 0.0 && !cubic.cut) || (angle < 0.0 && cubic.cut)){
                vertices.Add(new Vector3(v[j].x, v[j].y, v[j].z));
                uv.Add(new Vector4(u[j].x, u[j].y, u[j].z, u[j].w));
                indices.Add(indices.Count);

                vertices.Add(new Vector3(v[j+2].x, v[j+2].y, v[j+2].z));
                uv.Add(new Vector4(u[j+2].x, u[j+2].y, u[j+2].z, u[j+2].w));
                indices.Add(indices.Count);

                vertices.Add(new Vector3(v[j+1].x, v[j+1].y, v[j+1].z));
                uv.Add(new Vector4(u[j+1].x, u[j+1].y, u[j+1].z, u[j+1].w));
                indices.Add(indices.Count);
            } else {
                vertices.Add(new Vector3(v[j].x, v[j].y, v[j].z));
                uv.Add(new Vector4(u[j].x, u[j].y, u[j].z, u[j].w));
                indices.Add(indices.Count);

                vertices.Add(new Vector3(v[j+1].x, v[j+1].y, v[j+1].z));
                uv.Add(new Vector4(u[j+1].x, u[j+1].y, u[j+1].z, u[j+1].w));
                indices.Add(indices.Count);

                vertices.Add(new Vector3(v[j+2].x, v[j+2].y, v[j+2].z));
                uv.Add(new Vector4(u[j+2].x, u[j+2].y, u[j+2].z, u[j+2].w));
                indices.Add(indices.Count);
            }
        }
        return point;
    }

    private static CurveType compute(PCubic c, float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3, int recursiveType,
        List<Vector3> vertices, List<Vector4> uv, List<int> indices, float z) {
        Result res = new Result(x0, y0, x1, y1, x2, y2, x3, y3);
        CurveType curve_type = res.curve_type;

        float d3 = res.fd3;
        float d1 = res.fd1;
        float d2 = res.fd2;
        //		System.out.println(curve_type);
        float OneThird = (1.0f / 3.0f);
        float TwoThirds = (2.0f / 3.0f);
        float t1;
        float ls;
        float lt;
        float ms;
        float mt;
        float ltMinusLs;
        float mtMinusMs;
        float lsMinusLt;
        float ql;
        float qm;
        //  bool flip
        bool flip = false;
        //  artifact on loop
        int errorLoop = -1;
        float splitParam = 0;
        klm = new float[12];
        for (int i = 0; (i < 12); i++) {
            klm[i] = 0;
        }

        int o = orientation(start, ctrl1, ctrl2, end);
        int ol = orientationLoop(start, ctrl1, ctrl2, end);
        if ((o == -1)) {
            d1 = (d1 * -1);
            d2 = (d2 * -1);
            d3 = (d3 * -1);
        }

        int error = insideTriangle();
        switch (curve_type) {
            case CurveType.UNKNOWN:
                break;
            case CurveType.SERPENTINE:
                t1 = Mathf.Sqrt(9.0f * d2 * d2 - 12.0f * d1 * d3);
                ls = ((3.0f * d2) - t1);
                lt = (6.0f * d1);
                ms = ((3.0f * d2) + t1);
                mt = lt;
                ltMinusLs = (lt - ls);
                mtMinusMs = (mt - ms);

                if (error != -1) {
                    errorLoop = (error == 0) ? 2 : 1;
                    l.p1 = new Vector2d(start);
                    l.p2 = new Vector2d(end);
                    DoubleArray r = LineIntersects(new PCubic(start, ctrl1, ctrl2, end), l);
                    float t = .5f;
                    for (int i = 0; i < r.size; i++) {
                        if (r.get(i) > .001f && r.get(i) < .999f)
                            t = (float)r.get(i);
                    }
                    splitParam = t;
                }

                klm[0] = ls * ms;
                klm[1] = ls * ls * ls;
                klm[2] = ms * ms * ms;

                klm[3] = OneThird * (3.0f * ls * ms - ls * mt - lt * ms);
                klm[4] = ls * ls * (ls - lt);
                klm[5] = ms * ms * (ms - mt);

                klm[6] = OneThird * (lt * (mt - 2.0f * ms) + ls * (3.0f * ms - 2.0f * mt));
                klm[7] = ltMinusLs * ltMinusLs * ls;
                klm[8] = mtMinusMs * mtMinusMs * ms;

                klm[9] = ltMinusLs * mtMinusMs;
                klm[10] = -(ltMinusLs * ltMinusLs * ltMinusLs);
                klm[11] = -(mtMinusMs * mtMinusMs * mtMinusMs);


                if ((d1 < 0.0)) {
                    flip = true;
                }

                break;
            case CurveType.LOOP:
                t1 = (Mathf.Sqrt(4.0f * d1 * d3 - 3.0f * d2 * d2));
                ls = (d2 - t1);
                lt = (2.0f * d1);
                ms = (d2 + t1);
                mt = lt;
                ql = (ls / lt);
                qm = (ms / mt);

                if (0.0 < ql && ql < 1.0) {
                    errorLoop = 1;
                    splitParam = ql;
                }

                if (0.0 < qm && qm < 1.0) {
                    errorLoop = 2;
                    splitParam = qm;
                    if (ol == 1) {
                        errorLoop = 1;
                        splitParam = ql;
                    }
                }

                ltMinusLs = lt - ls;
                mtMinusMs = mt - ms;

                klm[0] = ls * ms;
                klm[1] = ls * ls * ms;
                klm[2] = ls * ms * ms;
                klm[3] = OneThird * (-ls * mt - lt * ms + 3.0f * ls * ms);
                klm[4] = -OneThird * ls * (ls * (mt - 3.0f * ms) + 2.0f * lt * ms);
                klm[5] = -OneThird * ms * (ls * (2.0f * mt - 3.0f * ms) + lt * ms);
                klm[6] = OneThird * (lt * (mt - 2.0f * ms) + ls * (3.0f * ms - 2.0f * mt));
                klm[7] = OneThird * (lt - ls) * (ls * (2.0f * mt - 3.0f * ms) + lt * ms);
                klm[8] = OneThird * (mt - ms) * (ls * (mt - 3.0f * ms) + 2.0f * lt * ms);
                klm[9] = ltMinusLs * mtMinusMs;
                klm[10] = -(ltMinusLs * ltMinusLs) * mtMinusMs;
                klm[11] = -ltMinusLs * mtMinusMs * mtMinusMs;

                if (recursiveType == -1)
                    flip = ((d1 > 0.0 && klm[0] < 0.0) || (d1 < 0.0 && klm[0] > 0.0));
                break;

            case CurveType.CUSP:
                ls = d3;
                lt = 3.0f * d2;
                lsMinusLt = ls - lt;
                klm[0] = ls;
                klm[1] = ls * ls * ls;
                klm[2] = 1.0f;
                klm[3] = ls - OneThird * lt;
                klm[4] = ls * ls * lsMinusLt;
                klm[5] = 1.0f;
                klm[6] = ls - TwoThirds * lt;
                klm[7] = lsMinusLt * lsMinusLt * ls;
                klm[8] = 1.0f;
                klm[9] = lsMinusLt;
                klm[10] = lsMinusLt * lsMinusLt * lsMinusLt;
                klm[11] = 1.0f;
                break;
            case CurveType.QUADRATIC:
                klm[0] = 0;
                klm[1] = 0;
                klm[2] = 0;
                klm[3] = OneThird;
                klm[4] = 0;
                klm[5] = OneThird;
                klm[6] = TwoThirds;
                klm[7] = OneThird;
                klm[8] = TwoThirds;
                klm[9] = 1;
                klm[10] = 1;
                klm[11] = 1;
                if ((d3 < 0.0)) {
                    flip = true;
                }

                break;
            case CurveType.LINE:
                break;
        }
        if (((errorLoop != -1)
            && ((recursiveType == -1)
                && (d1 > 0.0)))) {
            float x01 = (((x1 - x0) * splitParam) + x0);
            float x12 = (((x2 - x1) * splitParam) + x1);
            float x23 = (((x3 - x2) * splitParam) + x2);
            float y01 = (((y1 - y0) * splitParam) + y0);
            float y12 = (((y2 - y1) * splitParam) + y1);
            float y23 = (((y3 - y2) * splitParam) + y2);
            float x012 = (((x12 - x01) * splitParam) + x01);
            float x123 = (((x23 - x12) * splitParam) + x12);
            float y012 = (((y12 - y01) * splitParam) + y01);
            float y123 = (((y23 - y12) * splitParam) + y12);
            float x0123 = (((x123 - x012) * splitParam) + x012);
            float y0123 = (((y123 - y012) * splitParam) + y012);

            vertices.Add(new Vector3(x0, y0, z));
            vertices.Add(new Vector3(x0123, y0123, z));
            vertices.Add(new Vector3(x3, y3, z));
            uv.Add(new Vector4(1, 1, 1, 6));
            uv.Add(new Vector4(1, 1, 1, 6));
            uv.Add(new Vector4(1, 1, 1, 6));
            indices.Add(indices.Count);
            indices.Add(indices.Count);
            indices.Add(indices.Count);

            if ((errorLoop == 1)) {
                //  flip second
                compute(c, x0, y0, x01, y01, x012, y012, x0123, y0123, 0, vertices, uv, indices, z);
                compute(c, x0123, y0123, x123, y123, x23, y23, x3, y3, 1, vertices, uv, indices, z);
            } else if ((errorLoop == 2)) {
                //  flip first
                compute(c, x0, y0, x01, y01, x012, y012, x0123, y0123, 1, vertices, uv, indices, z);
                compute(c, x0123, y0123, x123, y123, x23, y23, x3, y3, 0, vertices, uv, indices, z);
            }
            return curve_type;
        } 

        if ((recursiveType == 1)) {
            flip = !flip;
        }

        if (flip) {
            klm[0] = (klm[0] * -1.0f);
            klm[1] = (klm[1] * -1.0f);
            klm[3] = (klm[3] * -1.0f);
            klm[4] = (klm[4] * -1.0f);
            klm[6] = (klm[6] * -1.0f);
            klm[7] = (klm[7] * -1.0f);
            klm[9] = (klm[9] * -1.0f);
            klm[10] = (klm[10] * -1.0f);

        }

        Triangulation(c, x0, y0, x1, y1, x2, y2, x3, y3, klm, vertices, uv, indices, z);
        return curve_type;
    }
    private static Vertex v0 = new Vertex();
    private static Vertex v1 = new Vertex();
    private static Vertex v2 = new Vertex();
    private static Vertex v3 = new Vertex();
    private static Vertex[] vertices = new Vertex[4] { v0, v1, v2, v3};

    private static void AddVertex(PCubic c, Vertex v, List<Vector3> vertices, List<Vector4> uv, List<int> indices) {
        vertices.Add(new Vector3(v.xyz.x, v.xyz.y, v.xyz.z));
        uv.Add(new Vector4(v.coords.x, v.coords.y, v.coords.z, c.cut == true ? 7 : 1));
        indices.Add(indices.Count);
    }

    private static void AddVertexPlain(Vertex v, List<Vector3> vertices, List<Vector4> uv,  List<int> indices) {
        vertices.Add(new Vector3(v.xyz.x, v.xyz.y, v.xyz.z));
        uv.Add(new Vector4(v.coords.x, v.coords.y, v.coords.z, 0));
        indices.Add(indices.Count);
    }
    private static void Triangulation(PCubic c, float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3, 
    float[] klm, List<Vector3> verts, List<Vector4> uv, List<int> idx, float z) {
        v0.Set(x0, y0, z, klm[0], klm[1], klm[2], 1);
        v1.Set(x1, y1, z, klm[3], klm[4], klm[5], 1);
        v2.Set(x2, y2, z, klm[6], klm[7], klm[8], 1);
        v3.Set(x3, y3, z, klm[9], klm[10], klm[11], 1);
        for (int i = 0; (i < 4); i++) {
            for (int j = (i + 1); (j < 4); j++) {
                if (approxEqual(vertices[i].xyz, vertices[j].xyz)) {
                    int[] indices = new int[] { 0, 0, 0 };
                    int index = 0;
                    for (int k = 0; (k < 4); k++) {
                        if ((k != j)) {
                            indices[index++] = k;
                        }

                    }
                    AddVertex(c, vertices[indices[0]], verts, uv, idx);
                    AddVertex(c, vertices[indices[1]], verts, uv, idx);
                    AddVertex(c, vertices[indices[2]], verts, uv, idx);
                    return;
                }

            }

        }

        for (int i = 0; (i < 4); i++) {
            int[] indices = new int[] { 0, 0, 0 };
            int index = 0;
            for (int j = 0; (j < 4); j++) {
                if ((i != j)) {
                    indices[index++] = j;
                }

            }

            if (pointInTriangle(vertices[i].xyz, vertices[indices[0]].xyz, vertices[indices[1]].xyz, vertices[indices[2]].xyz)) {
                for (int j = 0; (j < 3); j++) {
                    AddVertex(c, vertices[indices[(j % 3)]], verts, uv, idx);
                    AddVertex(c, vertices[indices[((j + 1) % 3)]], verts, uv, idx);
                    AddVertex(c, vertices[i], verts, uv, idx);
                }

                return;
            }

        }

        if (intersect(vertices[0].xyz, vertices[2].xyz, vertices[1].xyz, vertices[3].xyz)) {
            if ((vertices[2].cpy() - vertices[0].xyz).sqrMagnitude < (vertices[3].cpy() - vertices[1].xyz).sqrMagnitude) {
                AddVertex(c, vertices[0], verts, uv, idx);
                AddVertex(c, vertices[1], verts, uv, idx);
                AddVertex(c, vertices[2], verts, uv, idx);
                AddVertex(c, vertices[0], verts, uv, idx);
                AddVertex(c, vertices[2], verts, uv, idx);
                AddVertex(c, vertices[3], verts, uv, idx);
            } else {
                AddVertex(c, vertices[0], verts, uv, idx);
                AddVertex(c, vertices[1], verts, uv, idx);
                AddVertex(c, vertices[3], verts, uv, idx);
                AddVertex(c, vertices[1], verts, uv, idx);
                AddVertex(c, vertices[2], verts, uv, idx);
                AddVertex(c, vertices[3], verts, uv, idx);
            }
        } else if (intersect(vertices[0].xyz, vertices[3].xyz, vertices[1].xyz, vertices[2].xyz)) {
            if ((vertices[3].cpy() - vertices[0].xyz).sqrMagnitude < (vertices[2].cpy() - vertices[1].xyz).sqrMagnitude) {
                AddVertex(c, vertices[0], verts, uv, idx);
                AddVertex(c, vertices[1], verts, uv, idx);
                AddVertex(c, vertices[3], verts, uv, idx);
                AddVertex(c, vertices[0].copyFlip(), verts, uv, idx);
                AddVertex(c, vertices[3].copyFlip(), verts, uv, idx);
                AddVertex(c, vertices[2].copyFlip(), verts, uv, idx);
            } else {
                AddVertex(c, vertices[0], verts, uv, idx);
                AddVertex(c, vertices[1], verts, uv, idx);
                AddVertex(c, vertices[3], verts, uv, idx);
                AddVertex(c, vertices[2].copyFlip(), verts, uv, idx);
                AddVertex(c, vertices[0].copyFlip(), verts, uv, idx);
                AddVertex(c, vertices[3].copyFlip(), verts, uv, idx);
            }

        } else if ((vertices[1].cpy() - vertices[0].xyz).sqrMagnitude < (vertices[3].cpy() - vertices[2].xyz).sqrMagnitude) {
            AddVertex(c, vertices[0], verts, uv, idx);
            AddVertex(c, vertices[2], verts, uv, idx);
            AddVertex(c, vertices[1], verts, uv, idx);
            AddVertex(c, vertices[0], verts, uv, idx);
            AddVertex(c, vertices[1], verts, uv, idx);
            AddVertex(c, vertices[3], verts, uv, idx);
        } else {
            AddVertex(c, vertices[0], verts, uv, idx);
            AddVertex(c, vertices[2], verts, uv, idx);
            AddVertex(c, vertices[3], verts, uv, idx);
            AddVertex(c, vertices[3], verts, uv, idx);
            AddVertex(c, vertices[2], verts, uv, idx);
            AddVertex(c, vertices[1], verts, uv, idx);
        }

    }

    private static int insideTriangle() {
        Vector2[] vertices = new Vector2[4] { start, ctrl1, ctrl2, end};

        for (int i = 0; i < 4; ++i) {
            int[] indices = { 0, 0, 0 };
            int index = 0;
            for (int j = 0; j < 4; ++j)
                if (i != j)
                    indices[index++] = j;

            if (pointInTriangle(vertices[i], vertices[indices[0]], vertices[indices[1]],
                    vertices[indices[2]])) {
                if (i == 0 || i == 3) {
                    return i;
                }
            }
        }
        return -1;
    }

    public static bool approxEqual(Vector2 v0, Vector2 v1) {
        return (v0-v1).SqrMagnitude() < epsilon * epsilon;
    }

    private static bool pointInTriangle(Vector2 point, Vector2 a, Vector2 b, Vector2 c) {
        float x0 = c.x - a.x;
        float y0 = c.y - a.y;
        float x1 = b.x - a.x;
        float y1 = b.y - a.y;
        float x2 = point.x - a.x;
        float y2 = point.y - a.y;

        float dot00 = x0 * x0 + y0 * y0;
        float dot01 = x0 * x1 + y0 * y1;
        float dot02 = x0 * x2 + y0 * y2;
        float dot11 = x1 * x1 + y1 * y1;
        float dot12 = x1 * x2 + y1 * y2;
        float denominator = dot00 * dot11 - dot01 * dot01;
        if (denominator == 0)
            return false;

        float inverseDenominator = 1.0f / denominator;
        float u = (dot11 * dot02 - dot01 * dot12) * inverseDenominator;
        float v = (dot00 * dot12 - dot01 * dot02) * inverseDenominator;

        return (u > 0.0) && (v > 0.0) && (u + v < 1.0);
    }

    private static int orientation(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2) {
        int o1 = orientation(p1, q1, p2);
        int o2 = orientation(p1, q1, q2);
        int o3 = orientation(p2, q2, p1);
        int o4 = orientation(p2, q2, q1);
        int o5 = orientation(p1, p2, q2);

        if (o2 == 0)
            return 2;

        if (o3 == 0)
            return 3;

        if ((o1 >= 0 && o2 >= 0 && o3 >= 0) ||
           (o1 >= 0 && o2 >= 0 && o3 < 0) ||
           (o1 < 0 && o2 >= 0 && o3 >= 0) ||
           //		   (o1<0 && o2<0 && o3>=0 && o4<0) || 
           (o1 >= 0 && o2 < 0 && o3 >= 0)) {
            return 1;
        }
        return -1;
    }

    private static int orientation(Vector2 p1, Vector2 p2, Vector2 p3) {
        double crossProduct = (p2.y - p1.y) * (p3.x - p2.x) - (p3.y - p2.y) * (p2.x - p1.x);
        return (crossProduct < 0.0) ? -1 : ((crossProduct > 0.0) ? 1 : 0);
    }

    private static int orientationLoop(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2) {
        int o1 = orientation(p1, q1, p2);
        int o2 = orientation(p1, q1, q2);
        int o3 = orientation(p2, q2, p1);
        int o4 = orientation(p2, q2, q1);

        if ((o1 < 0 && o2 < 0 && o3 >= 0 && o4 < 0) || (o1 > 0 && o2 > 0 && o3 < 0 && o4 > 0))
            return 1;

        if ((o1 < 0 && o2 > 0 && o3 <= 0 && o4 < 0) || (o1 > 0 && o2 < 0 && o3 > 0 && o4 > 0))
            return -1;
        return 0;
    }

    public static bool intersect(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2) {
        return (orientation(p1, q1, p2) != orientation(p1, q1, q2)
                && orientation(p2, q2, p1) != orientation(p2, q2, q1));

    }

    public static double arcfn(Segment c, double t) {
        Vector2d d = derivative(c, t);
        double l = d.x * d.x + d.y * d.y;
        return Mathd.Sqrt(l);
    }

    /*
	 * public static Vector2d derivative(PCubic c, float t) { final float dt = 1f -
	 * t; final float dt2 = dt * dt; final float t2 = t * t; return new
	 * Vector2d().set(c.ctrl1).sub(c.start).scl(dt2 *
	 * 3f).add(c.ctrl2.cpy().sub(c.ctrl1).scl(dt * t * 6f))
	 * .add(c.end.cpy().sub(c.ctrl2).scl(t2 * 3f)); }
	 */

    public static Vector2d derivative(Segment cb, double t) {
        int order = cb.getPoints().Length - 1;
        double mt = 1 - t;
        double a=0, b=0, c=0;


        List<Vector2d> pp = derive(cb)[0];
        if(order == 1) {
            Vector2d[] p = new Vector2d[] { pp[0], pp[1], pp[2] };
            return new Vector2d((a * p[0].x + b * p[1].x + c * p[2].x), (a * p[0].y + b * p[1].y + c * p[2].y));
        }
        if(order == 2) {
            Vector2d[] p = new Vector2d[] { pp[0], pp[1], Vector2d.zero };
            a = mt;
            b = t;
            return new Vector2d((a * p[0].x + b * p[1].x + c * p[2].x), (a * p[0].y + b * p[1].y + c * p[2].y));
        }
        if(order == 3) {
            Vector2d[] p = new Vector2d[] { pp[0], pp[1], pp[2] };
            a = mt * mt; 
            b = mt * t * 2; 
            c = t * t;
            return new Vector2d((a * p[0].x + b * p[1].x + c * p[2].x), (a * p[0].y + b * p[1].y + c * p[2].y));
        }
        return Vector2d.zero;
    }

    // public static Vector2d compute(double t, Segment curve) {
    //     int order = curve.getPointsList().Count - 1;
    //     if (t == 0)
    //         return curve.getPoints()[0];
    //     if (t == 1)
    //         return curve.getPoints()[order];

    //     if (order == 0) {
    //         return curve.getPoints()[0];
    //     }

    //     Vector2d[] p = curve.getPoints();
    //     double mt = 1f - t;
    //     if (order == 1) {
    //         return new Vector2d(mt * p[0].x + t * p[1].x, mt * p[0].y + t * p[1].y);
    //     }

    //     if (order < 4) {
    //         double mt2 = mt * mt, t2 = t * t, a = 0, b = 0, c = 0, d = 0;
    //         if (order == 2) {
    //             p = new Vector2d[]{p[0], p[1], p[2], Vector2d.zero};
    //             a = mt2;
    //             b = mt * t * 2;
    //             c = t2;
    //         } else if (order == 3) {
    //             a = mt2 * mt;
    //             b = mt2 * t * 3f;
    //             c = mt * t2 * 3f;
    //             d = t * t2;
    //         }
    //         double x = (a * p[0].x + b * p[1].x + c * p[2].x + d * p[3].x);
    //         double y = (a * p[0].y + b * p[1].y + c * p[2].y + d * p[3].y);
    //         return new Vector2d(x, y);
    //     }

    //     return Vector2d.zero;
    // }

    public static Vector2d compute(double t, List<Vector2d> points) {
        int order = points.Count - 1;
        if (t == 0)
            return points[0];
        if (t == 1)
            return points[order];

        if (order == 0) {
            return points[0];
        }

        Vector2d[] p = points.ToArray();
        double mt = 1f - t;
        if (order == 1) {
            return new Vector2d(mt * p[0].x + t * p[1].x, mt * p[0].y + t * p[1].y);
        }

        if (order < 4) {
            double mt2 = mt * mt, t2 = t * t, a = 0, b = 0, c = 0, d = 0;
            if (order == 2) {
                p = new Vector2d[]{p[0], p[1], p[2], Vector2d.zero};
                a = mt2;
                b = mt * t * 2;
                c = t2;
            } else if (order == 3) {
                a = mt2 * mt;
                b = mt2 * t * 3f;
                c = mt * t2 * 3f;
                d = t * t2;
            }
            double x = (a * p[0].x + b * p[1].x + c * p[2].x + d * p[3].x);
            double y = (a * p[0].y + b * p[1].y + c * p[2].y + d * p[3].y);
            
            return new Vector2d(x, y);
        }

        return Vector2d.zero;
    }

    public static List<List<Vector2d>> derive(Segment seg) {
        List<List<Vector2d>> dpoints = new List<List<Vector2d>>();
        List<Vector2d> p = seg.getPointsList();
        // Debug.Log(seg.GetType()+": " + p.Count);
        for (int d = p.Count, c = d - 1; d > 1; d--, c--) {
            List<Vector2d> list = new List<Vector2d>();
            for (int j = 0; j < c; j++) {
                double x = c * (p[j + 1].x - p[j].x);
                double y = c * (p[j + 1].y - p[j].y);
                list.Add(new Vector2d(x, y));
            }
            dpoints.Add(list);
            p = list;
        }
        return dpoints;
    }

    public static bool approximately(double a, double b, double precision) {
        return (Mathd.Abs(a - b) <= precision);
    }

    public static bool between(double v, double m, double M) {
        return (m <= v && v <= M) || approximately(v, m, epsilon) || approximately(v, M, epsilon);
    }

    /*
	 * public static double length(PCubic c, int step) { float tempLength = 0;
	 * Vector2d tmp2 = new Vector2d(); Vector2d tmp3 = new Vector2d();
	 * 
	 * for (int i = 0; i < step; ++i) { tmp2.set(tmp3); tmp3 = valueAt(c, (i) /
	 * ( step - 1)); if (i > 0) tempLength += dst(tmp2, tmp3); } return
	 * tempLength; }
	 */

    public static double length(Segment c, int step) {
        double z = 0.5f, sum = 0, len = Tvalues.Length, t;
        for (int i = 0; i < len; i++) {
            t = z * Tvalues[i] + z;
            sum += Cvalues[i] * arcfn(c, t);
        }
        return z * sum;

    }

    public static Vector2d valueAt(PCubic c, double t) {
        return c.ValueAt(t);
    }

    public static double dst(Vector2d v1, Vector2d v2) {
         double x_d = v2.x - v1.x;
         double y_d = v2.y - v1.y;
        return Mathd.Sqrt(x_d * x_d + y_d * y_d);
    }

    public static double map(double v, double ds, double de, double ts, double te) {
        double d1 = de - ds, d2 = te - ts, v2 = v - ds, r = v2 / d1;
        return (ts + d2 * r);
    }

    public static Vector2d lerp(double r, Vector2d v1, Vector2d v2) {
        double x = (v1.x + r * (v2.x - v1.x));
        double y = (v1.y + r * (v2.y - v1.y));
        return new Vector2d(x, y);
    }

    public static String pointToString(Vector2d p) {
        String s = "(" + p.x + ", " + p.y + ")";
        return s;
    }

    public static String pointsToString(Vector2d[] p) {
        String s = "";
        foreach (Vector2d v in p) {
            s += "(" + v.x + ", " + v.y + ") ";
        }
        return s;
    }

    public static double Angle(Vector2d o, Vector2d v1, Vector2d v2) {
        double dx1 = v1.x - o.x, dy1 = v1.y - o.y, dx2 = v2.x - o.x, dy2 = v2.y - o.y, cross = dx1 * dy2 - dy1 * dx2,
                dot = dx1 * dx2 + dy1 * dy2;
        //		System.out.println("cross: "+cross+", dot: "+dot);
        return Mathd.Atan2(cross, dot);
    }
    public static double Angle(Vector3 o, Vector3 v1, Vector3 v2) {
        double dx1 = v1.x - o.x, dy1 = v1.y - o.y, dx2 = v2.x - o.x, dy2 = v2.y - o.y, cross = dx1 * dy2 - dy1 * dx2,
                dot = dx1 * dx2 + dy1 * dy2;
        //		System.out.println("cross: "+cross+", dot: "+dot);
        return Mathd.Atan2(cross, dot);
    }
    /*
	 * // round as string, to avoid rounding errors round: function(v, d) { var s =
	 * "" + v; var pos = s.indexOf("."); return parseFloat(s.substring(0, pos + 1 +
	 * d)); },
	 */

    public static List<Vector2d> getLUT(PCubic c, int steps) {
        List<Vector2d> lut = new List<Vector2d>();
        steps--;
        for (int t = 0; t <= steps; t++) {
            lut.Add(compute(t / steps, c.getPointsList()));
        }

        return lut;
    }

    public class Closest {
        public double mdist;
        public int idx;

        public Closest(double mdist, int idx) {
            this.mdist = mdist;
            this.idx = idx;
        }
    }

    public static Closest closest(List<Vector2d> LUT, Vector2d point) {
        double mdist = Mathd.Pow(2, 63), d;
        int mpos = -1;
        for (int i = 0; i < LUT.Count; i++) {
            d = dst(point, LUT[i]);
            if (d < mdist) {
                mdist = d;
                mpos = i;
            }
        }

        return new Closest(mdist, mpos);
    }

    public static double abcratio(double t) {
        t = Mathd.Clamp(t, 0.0, 1.0);
        if (t == 0 || t == 1) {
            return t;
        }
        double bottom = (Mathd.Pow(t, 3) + Mathd.Pow(1 - t, 3));
        double top = bottom - 1;
        return Mathd.Abs(top / bottom);
    }

    public static double projectionratio(double t) {
        t = Mathd.Clamp(t, 0.0, 1.0);
        if (t == 0 || t == 1) {
            return t;
        }
        double top = Mathd.Pow(1 - t, 3);
        double bottom = Mathd.Pow(t, 3) + top;
        return top / bottom;
    }

    public static Vector2d lli8(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4) {
        double nx = (x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4);
        double ny = (x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4);
        double d = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
        //		System.out.println("D:" + d);
        if (d == 0) {
            //	       		return null;
        }
        return new Vector2d(nx / d, ny / d);
    }

    public static Vector2d lli4(Vector2d p1, Vector2d p2, Vector2d p3, Vector2d p4) {
        double x1 = p1.x, y1 = p1.y, x2 = p2.x, y2 = p2.y, x3 = p3.x, y3 = p3.y, x4 = p4.x, y4 = p4.y;
        return lli8(x1, y1, x2, y2, x3, y3, x4, y4);
    }

    public static PCubic makeline(Vector2d p1, Vector2d p2) {
        double x1 = p1.x, y1 = p1.y, x2 = p2.x, y2 = p2.y, dx = (x2 - x1) / 3.0, dy = (y2 - y1) / 3;
        return new PCubic(x1, y1, x1 + dx, y1 + dy, x1 + 2.0 * dx, y1 + 2.0 * dy, x2, y2);
    }

    public static Vector2d FindIntersection(Vector2d s1, Vector2d e1, Vector2d s2, Vector2d e2) {
        double a1 = e1.y - s1.y;
        double b1 = s1.x - e1.x;
        double c1 = a1 * s1.x + b1 * s1.y;
 
        double a2 = e2.y - s2.y;
        double b2 = s2.x - e2.x;
        double c2 = a2 * s2.x + b2 * s2.y;
 
        double delta = a1 * b2 - a2 * b1;
        //If lines are parallel, the result will be (NaN, NaN).
        return delta == 0 ? new Vector2d(float.NaN, float.NaN)
            : new Vector2d((b2 * c1 - b1 * c2) / delta, (a1 * c2 - a2 * c1) / delta);
    }

    public static DoubleArray LineIntersects(PCubic c, Line line) {
        double minX = Mathd.Min(line.p1.x, line.p2.x);
        double minY = Mathd.Min(line.p1.y, line.p2.y);
        double maxX = Mathd.Max(line.p1.x, line.p2.x);
        double maxY = Mathd.Max(line.p1.y, line.p2.y);

        DoubleArray roots = Roots(c, line);
        DoubleArray ret = new DoubleArray();
        //		for (int i = 0; i < roots.Count; i++) {
        //			Vector2d p = compute(roots[i), c);
        //			if(between(p.x, minX, maxX) && between(p.y, minY, maxY))
        //				ret.add(roots[i));
        //		}
        return roots;
    }

    public static DoubleArray Roots(PCubic curve, Line line) {
        // int order = 3;
        List<Vector2d> pnts = Align(curve, line);
        double pa = pnts[0].y, pb = pnts[1].y, pc = pnts[2].y, pd = pnts[3].y,
                d = -pa + 3.0 * pb - 3.0 * pc + pd, a = 3.0 * pa - 6.0 * pb + 3.0 * pc, b = -3.0 * pa + 3.0 * pb,
                c = pa;

        /*
		 * Check for solution for ther orders if (utils.approximately(d, 0)) { // this
		 * is not a cubic curve. if (utils.approximately(a, 0)) { // in fact, this is
		 * not a quadratic curve either. if (utils.approximately(b, 0)) { // in fact in
		 * fact, there are no solutions. return []; } // linear solution: return [-c /
		 * b].filter(reduce); } // quadratic solution: var q = sqrt(b * b - 4 * a * c),
		 * a2 = 2 * a; return [(q - b) / a2, (-b - q) / a2].filter(reduce); }
		 */

        a /= d;
        b /= d;
        c /= d;

        double p = (3.0 * b - a * a) / 3.0, p3 = p / 3.0, q = (2.0 * a * a * a - 9.0 * a * b + 27.0 * c) / 27.0,
                q2 = q / 2.0, discriminant = q2 * q2 + p3 * p3 * p3, u1, v1, x1, x2, x3;
        DoubleArray roots = new DoubleArray();
        if (discriminant < 0.0) {
            double mp3 = -p / 3.0, mp33 = mp3 * mp3 * mp3, r = Mathd.Sqrt(mp33), t = -q / (2.0 * r),
                    cosphi = t < -1.0 ? -1.0 : t > 1.0 ? 1.0 : t, phi = Mathd.Acos(cosphi), crtr = crt(r),
                    t1 = 2.0 * crtr;
            x1 = t1 * Mathd.Cos(phi / 3.0) - a / 3.0;
            x2 = t1 * Mathd.Cos((phi + Mathd.PI * 2.0) / 3.0) - a / 3.0;
            x3 = t1 * Mathd.Cos((phi + 2.0 * Mathd.PI * 2.0) / 3.0) - a / 3.0;

            if (reduce(x1))
                roots.add(x1);
            if (reduce(x2))
                roots.add(x2);
            if (reduce(x3))
                roots.add(x3);
            return roots;
        } else if (discriminant == 0.0) {
            u1 = q2 < 0.0 ? crt(-q2) : -crt(q2);
            x1 = 2.0 * u1 - a / 3;
            x2 = -u1 - a / 3.0;
            if (reduce(x1))
                roots.add(x1);
            if (reduce(x2))
                roots.add(x2);
            return roots;
        } else {
            double sd = Mathd.Sqrt(discriminant);
            u1 = crt(-q2 + sd);
            v1 = crt(q2 + sd);

            if (reduce(u1 - v1 - a / 3.0))
                roots.add(u1 - v1 - a / 3.0);
            return roots;
        }

    }

    public static List<Vector2d> Align(PCubic c, Line line) {
        double tx = line.p1.x;
        double ty = line.p1.y;
        double a = -Mathd.Atan2(line.p2.y - ty, line.p2.x - tx);

        List<Vector2d> p = c.getPointsList();
        List<Vector2d> pnts = new List<Vector2d>();
        for (int i = 0; i < p.Count; i++) {
            double x = (p[i].x - tx) * Mathd.Cos(a) - (p[i].y - ty) * Mathd.Sin(a);
            double y = (p[i].x - tx) * Mathd.Sin(a) + (p[i].y - ty) * Mathd.Cos(a);
            pnts.Add(new Vector2d(x, y));
        }

        return pnts;
    }

    public static bool reduce(double t) {
        return t >= 0.0 && t <= 1.0;
    }

    public static double crt(double v) {
        return v < 0 ? -Mathd.Pow(-v, 1.0 / 3.0) : Mathd.Pow(v, 1.0 / 3.0);
    }

    public static DoubleArray Droots(DoubleArray p) {
        DoubleArray arr = new DoubleArray();
        // quadratic roots are easy
        if (p.size == 3) {
            double a = p.get(0), b = p.get(1), c = p.get(2), 
            d = a - 2.0 * b + c;
            if (d != 0.0) {
                double m1 = -Mathd.Sqrt(b * b - a * c), 
                m2 = -a + b, 
                v1 = -(m1 + m2) / d, 
                v2 = -(-m1 + m2) / d;
                arr.addAll(v1, v2);
                return arr;
            } else if (b != c && d == 0.0) {
                arr.add((2.0 * b - c) / (2.0 * (b - c)));
                return arr;
            }
        } else if (p.size == 2) {
            double a = p.get(0), b = p.get(1);
            if (a != b) {
                arr.add(a / (a - b));
            }
        }
        return arr;
    }

    public static XtrmResult Extrema(Segment c) {
        int order = c.getPoints().Length-1;
        XtrmResult result = new XtrmResult();
        DoubleArray roots = new DoubleArray();
        for (int i = 0; i < 2; i++) {
            List<Vector2d> dpoints = derive(c)[0];
            DoubleArray p = new DoubleArray();
            // Debug.Log("dpoint x size: " + dpoints.Count);
            
            for (int j = 0; j < dpoints.Count; j++) {
                if (i == 0)
                    p.add(dpoints[j].x);
                else
                    p.add(dpoints[j].y);
            }
            if (i == 0) {
                DoubleArray x = Droots(p);
                for (int j = 0; j < x.size; j++)
                    if (x.get(j) >= 0.0 && x.get(j) <= 1.0)
                        result.x.add(x.get(j));
                roots.addAll(numberSort(result.x));
            } else {
                DoubleArray y = Droots(p);
                for (int j = 0; j < y.size; j++)
                    if (y.get(j) >= 0.0 && y.get(j) <= 1.0)
                        result.y.add(y.get(j));
                roots.addAll(numberSort(result.y));
            }
            if(order == 3){
                dpoints = derive(c)[1];
                p = new DoubleArray();

                for (int j = 0; j < dpoints.Count; j++) {
                    if (i == 0)
                        p.add(dpoints[j].x);
                    else
                        p.add(dpoints[j].y);
                }
                if (i == 0) {
                    DoubleArray x = Droots(p);
                    for (int j = 0; j < x.size; j++)
                        if (x.get(j) >= 0.0 && x.get(j) <= 1.0)
                            result.x.add(x.get(j));
                    roots.addAll(numberSort(result.x));
                } else {
                    DoubleArray y = Droots(p);
                    for (int j = 0; j < y.size; j++)
                        if (y.get(j) >= 0.0 && y.get(j) <= 1.0)
                            result.y.add(y.get(j));
                    roots.addAll(numberSort(result.y));
                }
            }
            DoubleArray v = new DoubleArray();
            for (int j = 0; j < roots.size; j++) {
                if (roots.indexOf(roots.get(j)) == j)
                    v.add(roots.get(j));
            }
            result.values = numberSort(v);
        }
        //		System.out.println("extrema: " +result.values.Count);
        return result;
    }

    public static BBoxResult bbox(Segment c) {
        if(c.GetType() == typeof(PCirFsix)){
            PCirFsix cir = (PCirFsix)c;
            BBoxResult r = new BBoxResult();
            r.x = new BBoxDim(cir.Center.x - cir.Radius, cir.Center.x + cir.Radius);
            r.y = new BBoxDim(cir.Center.y - cir.Radius, cir.Center.y + cir.Radius);
            return r;
        }
        XtrmResult extrema = Extrema(c);
        BBoxResult result = new BBoxResult();
        result.x = getMinMax(c, 0, extrema.x);
        result.y = getMinMax(c, 1, extrema.y);
        return result;
    }

    public static BBoxResult bbox(Segment c, BBoxResult res) {
        XtrmResult extrema = Extrema(c);
        res.x = getMinMax(c, 0, extrema.x);
        res.y = getMinMax(c, 1, extrema.y);
        return res;
    }

    public static BBoxDim getMinMax(Segment curve, int dim, DoubleArray list) {
        double min = Double.MaxValue, max = -Double.MaxValue, t;
        Vector2d c;
        if (list.indexOf(0.0) == -1)
            list.insert(0, 0.0);
        if (list.indexOf(1.0) == -1)
            list.add(1.0);
        for (int i = 0; i < list.size; i++) {
            t = list.get(i);
            c = compute(t, curve.getPointsList());
            double cd = (dim == 0 ? c.x : c.y);
            if (cd < min) {
                min = cd;
            }
            if (cd > max) {
                max = cd;
            }
        }
        BBoxDim bbd = new BBoxDim();
        bbd.min = min;
        bbd.mid = min + max;
        bbd.max = max;
        bbd.size = max - min;

        return bbd;
    }

    public static DoubleArray numberSort(DoubleArray a) {
        double temp;
        for (int i = 0; i < a.size; i++) {
            for (int j = i + 1; j < a.size; j++) {
                if (a.get(i) > a.get(j)) {
                    temp = a.get(i);
                    a.set(i, a.get(j));
                    a.set(j, temp);
                }
            }
        }

        return a;
    }

    public static List<Vector2d> hull(Segment c, double t) {
        List<Vector2d> p = c.getPointsList();
        List<Vector2d> _p = new List<Vector2d>();
        Vector2d pt;
        List<Vector2d> q = new List<Vector2d>();
        q.Add(p[0]);
        q.Add(p[1]);
        q.Add(p[2]);
        if (p.Count - 1 == 3) {
            q.Add(p[3]);
        }

        while (p.Count > 1) {
            _p = new List<Vector2d>();
            for (int i = 0; i < p.Count - 1; i++) {
                pt = lerp(t, p[i], p[i + 1]);
                q.Add(pt);
                _p.Add(pt);
            }
            p = _p;
        }
        return q;
    }

    public static SplitResult split(Segment c, double t) {
        int order = c.getPoints().Length - 1;
        List<Vector2d> q = hull(c, t);
        SplitResult result = new SplitResult();
        if (t == 0.0)
            result.right = c;
        else if (t == 1.0)
            result.left = c;
        else {
            result.left = order == 2 
                ? (Segment)new PQuadratic(q[0], q[3], q[5]) 
                : (Segment)new PCubic(q[0], q[4], q[7], q[9]);
            result.right = order == 2 
                ? (Segment)new PQuadratic(q[5], q[4], q[2]) 
                : (Segment)new PCubic(q[9], q[8], q[6], q[3]);
            result.span = q;
        }
        return result;
    }

    public static Segment split(Segment c, double t1, double t2) {
        int order = c.getPoints().Length - 1;
        if (t1 == 0.0 && t2 != 0.0)
            return split(c, t2).left;
        if (t2 == 1.0)
            return split(c, t1).right;

        List<Vector2d> q = hull(c, t1);
        SplitResult result = new SplitResult();
        result.left = order == 2 
            ? (Segment)new PQuadratic(q[0], q[3], q[5]) 
            : (Segment)new PCubic(q[0], q[4], q[7], q[9]);
        result.right = order == 2 
            ? (Segment)new PQuadratic(q[5], q[4], q[2]) 
            : (Segment)new PCubic(q[9], q[8], q[6], q[3]);
        result.span = q;

        if (t2 == 0.0)
            return result.left;

        double t = map(t2, t1, 1.0, 0.0, 1.0);
        SplitResult subsplit = split(result.right, t);
        return subsplit.left;
    }

    public static List<List<Segment>> reduce(Segment c) {
        double t1 = 0, t2 = 0, step = .4;
        const double STEP_5 = .4, STEP_1 = .2, STEP_05 = .05, STEP_01 = .01;
        Segment segment;
        List<Segment> fix = new List<Segment>();
        List<Segment> pass1 = new List<Segment>();
        List<Segment> pass2 = new List<Segment>();
        List<Segment> pass3 = new List<Segment>();

        DoubleArray extrema = Extrema(c).values;
        if (extrema.indexOf(0.0) == -1) {
            extrema.insert(0, 0.0);
        }
        if (extrema.indexOf(1.0) == -1) {
            extrema.add(1.0);
        }

        t1 = extrema.get(0);
        for (int i = 1; i < extrema.size; i++) {
            t2 = extrema.get(i);
            segment = split(c, t1, t2);
            pass1.Add(segment);
            t1 = t2;
        }
        int tick = 0;
        for (int i = 0; i < pass1.Count; i++) {
            Segment p1 = pass1[i];
            t1 = 0.0;
            t2 = 0.0;
        B: while (t2 <= 1.0) {
                for (t2 = t1 + step; t2 <= (1.0 + step); t2 += step) {
                    segment = split(p1, t1, t2);
                    if (!simple(segment)) {
                        t2 -= step;
                        if(step == STEP_5){
                            step = STEP_1;
                            continue;
                        } else if(step == STEP_1){
                            step = STEP_05;
                            continue;
                        } else if(step == STEP_05){
                            step = STEP_01;
                            continue;
                        } 
                        if (Mathd.Abs(t1 - t2) < step) {
                            t1 = t2 + step;
                            // pass2.Add(segment);
                            // pass2.Add(new PLine(compute(t1, p1.getPointsList()), compute(t2, p1.getPointsList())));
                            tick++;
                            goto B;
                        }
                        segment = split(p1, t1, t2);
                        pass2.Add(segment);
                        t1 = t2;
                        tick++;
                        break;
                    }
                    if(step == STEP_01){
                        step = STEP_05;
                    } else if(step == STEP_05){
                        step = STEP_1;
                    } else if(step == STEP_1){
                        step = STEP_5;
                    } 
                    tick++;
                }
            }
            if (t1 < 1.0) {
                segment = split(p1, t1, 1);
                pass2.Add(segment);
            }
            step = STEP_5;
        }
        // Debug.Log("tick: " + tick);

        List<List<Segment>> l = new List<List<Segment>>();
        l.Add(pass1);
        l.Add(pass2);
        return l;
    }

    public static float disc(PCubic c) {
        float d1 = 0, d2 = 0, d3 = 0;
        Vector3 b0 = new Vector3((float)c.StartPoint.x, (float)c.StartPoint.y, 1);
        Vector3 b1 = new Vector3((float)c.Ctrl1.x, (float)c.Ctrl1.y, 1);
        Vector3 b2 = new Vector3((float)c.Ctrl2.x, (float)c.Ctrl2.y, 1);
        Vector3 b3 = new Vector3((float)c.EndPoint.x, (float)c.EndPoint.y, 1);

        float a1 = Vector3.Dot(b0, Vector3.Cross(b3, b2));
        float a2 = Vector3.Dot(b1, Vector3.Cross(b0, b3));
        float a3 = Vector3.Dot(b2, Vector3.Cross(b1, b0));

        d1 = a1 - 2 * a2 + 3 * a3;
        d2 = -a2 + 3 * a3;
        d3 = 3 * a3;

        Vector3 d = new Vector3(d1, d2, d3);
        d.Normalize();
        d1 = d.x;
        d2 = d.y;
        d3 = d.z;

        float D = 3.0f * d2 * d2 - 4.0f * d1 * d3;
        return d1 * d1 * D;
    }

    public static Vector2d curvature(PCubic c, double t) {
        List<List<Vector2d>> dpoints = derive(c);

        List<Vector2d> d1 = dpoints[0];
        List<Vector2d> d2 = dpoints[1];
        Vector2d d = compute(t, d1);
        Vector2d dd = compute(t, d2);
        double num, dnm;
        num = d.x * dd.y - d.y * dd.x;
        dnm = Mathd.Pow(d.x * d.x + d.y * d.y, 3f / 2f);

        if (num == 0 || dnm == 0) {
            return new Vector2d(0, 0);
        }

        return new Vector2d(num / dnm, dnm / num);
    }

    public static bool simple(Segment s) {
        int order = s.getPoints().Length - 1;
        if(order == 3){
            double a1 = Angle(s.getPoints()[0], s.getPoints()[3], s.getPoints()[1]);
            double a2 = Angle(s.getPoints()[0], s.getPoints()[3], s.getPoints()[2]);
        //		System.out.println("angles: "+ a1 +", "+a2);
            if ((a1 > 0.0 && a2 < 0.0) || (a1 < 0.0 && a2 > 0.0))
                return false;
        }
        
        double[] n1 = normal(s, 0.0);
        double[] n2 = normal(s, 1.0);
        double ss = n1[0] * n2[0] + n1[1] * n2[1];
        double angle = Mathd.Abs(Mathd.Acos(ss));

        return angle < Mathd.PI / 5f;
    }

    public static double[] normal(Segment c, double t) {
        Vector2d d = derivative(c, t);
        double q = Mathd.Sqrt(d.x * d.x + d.y * d.y);
        return new double[] { -d.y / q, d.x / q };
    }

    public static double DistanceFn(double v, LinearDistanceFunction l) {
        double f1 = l.alen / l.tlen, f2 = (l.alen + l.slen) / l.tlen, d = l.e - l.s;
        return map(v, 0.0, 1.0, l.s + f1 * d, l.s + f2 * d);
    }

    private static List<Vector2d> np = new List<Vector2d>();

    public static Segment raise(Segment c) {
        Vector2d[] p = c.getPoints();
        np.Clear();
        np.Add(p[0]);
        int k = p.Length;
        Vector2d pi, pim;
        for (int i = 1; i < k; i++) {
            pi = p[i];
            pim = p[i - 1];
            np.Add(new Vector2d((k - i) / k * pi.x + i / k * pim.x, (k - i) / k * pi.y + i / k * pim.y));
        }
        np.Insert(k, p[k - 1]);
        return p.Length - 1 == 3 ? (Segment)new PCubic(np) : (Segment)new PQuadratic(np);
    }

    // public static List<PCubic> offset(PCubic c, double d) {
    //     Vector2d[] points = c.points;
    //     bool _linear = true;
    //     List<Vector2d> a = Align(c, new Line(points[0], points[3]));
    //     List<Vector2d> p = new List<Vector2d>();
    //     for (int i = 0; i < a.Count; i++) {
    //         if (Mathd.Abs(a[i].y) > 0.0001f) {
    //             _linear = false;
    //         }
    //     }

    //     if (_linear) {
    //         for (int i = 0; i < c.points.Length; i++) {
    //             double[] nv = normal(c, 0.0);
    //             p.Add(new Vector2d((c.points[i].x + d * nv[0]), (c.points[i].y + d * nv[1])));
    //         }
    //         PCubic s = new PCubic(new PCubic(p));
    //         List<PCubic> arr = new List<PCubic>();
    //         arr.Add(s);
    //         return arr;
    //     }

    //     List<PCubic> reduced = reduce(c);
    //     List<PCubic> result = new List<PCubic>();
    //     foreach (PCubic segment in reduced) {
    //         PCubic s = scale(segment, d, null);
    //         if (s != null)
    //             result.Add(s);
    //     }
    //     return result;
    // }

    public static List<Segment> offset(Segment c, double d, LinearDistanceFunction distanceFn) {
        List<Segment> reduced = reduce(c)[1];
        List<Segment> result = new List<Segment>();
        foreach (PCubic segment in reduced) {
            Segment s = scale(segment, d, null);
            if (s != null)
                result.Add(s);
        }
        return result;
    }

    public static OffsetResult offset(Segment s, double t, double d) {
        return new OffsetResult(compute(t, s.getPointsList()), normal(s, t), d);
    }

    public static Segment scale(Segment s, double d, LinearDistanceFunction distanceFn) {
        int order = s.getPoints().Length - 1;
        if (distanceFn != null && order == 2) {
            return scale(raise(s), d, distanceFn);
        }
        // TODO: add special handling for degenerate (=linear) curves
        bool clockwise = Clockwise(s);
        double r1 = distanceFn != null ? DistanceFn(0.0, distanceFn) : d;
        double r2 = distanceFn != null ? DistanceFn(1.0, distanceFn) : d;

        OffsetResult[] v = new OffsetResult[] { offset(s, 0.0, 10.0), offset(s, 1.0, 10.0) };
        Vector2d o = lli4(v[0].xy, v[0].c, v[1].xy, v[1].c);

        if (Double.IsNaN(o.x)) {
            //			System.out.println("null insise scale");
            //			return null;
            //			 throw new GdxRuntimeException("cannot scale this curve. Try reducing it first.");
        }
        //		System.out.println("o: "+o.toString());

        // move all points by distance 'd' wrt the origin 'o'
        Vector2d[] points = s.getPoints();
        Vector2d[] np = new Vector2d[points.Length];

        // move end points by fixed distance along normal.
        for (int t = 0; t < 2; t++) {
            np[t * order] = points[t * order].cpy();
            np[t * order].x += (t == 1 ? r2 : r1) * v[t].n.x;
            np[t * order].y += (t == 1 ? r2 : r1) * v[t].n.y;
            //			System.out.println("np["+(t*3)+"]: "+np[t*3].toString());
        }
        
        if (distanceFn == null) {
            // move control points to lie on the intersection of the offset
            // derivative vector, and the origin-through-control vector
            for (int t = 0; t < 2; t++) {
                if (order == 2 && t > 0) 
                    break;
                Vector2d p = np[t * order];
                Vector2d dd = derivative(s, t);
                Vector2d p2 = new Vector2d(p.x + dd.x, p.y + dd.y);
                np[t + 1] = lli4(p, p2, o, points[t + 1].cpy());
                //	            System.out.println("np["+(t+1)+"]: "+np[t+1].toString());
            }
            // Debug.Log("off: " + np[3].x +", "+ np[3].y);
            return order == 3 ? (Segment)new PCubic(np) : (Segment)new PQuadratic(np);
        }
        // move control points by "however much necessary to
        // ensure the correct tangent to endpoint".
        for (int t = 0; t < 2; t++) {
            if (order == 2 && t > 0) 
                    break;
            Vector2d p = points[t + 1].cpy();
            Vector2d ov = new Vector2d(p.x - o.x, p.y - o.y);
            double rc = distanceFn != null ? DistanceFn((t + 1.0) / order, distanceFn) : d;
            if (distanceFn != null && !clockwise)
                rc = -rc;
            double m = Mathd.Sqrt(ov.x * ov.x + ov.y * ov.y);
            ov *= (1.0 / m);
            np[t + 1] = new Vector2d(p.x + rc * ov.x, p.y + rc * ov.y);
        }

        return order == 3 ? (Segment)new PCubic(np) : (Segment)new PQuadratic(np);
    }

    public static bool Clockwise(Segment s) {
        Vector2d[] points = s.getPoints();
        double angle = Angle(points[0], points[s.getPoints().Length - 1], points[1]);
        // double angle2 = Angle(points[s.getPoints().Length - 1], points[2], points[1]);
        // Debug.Log("same: " + (angle > 0.0 && angle2 > 0.0));
        
        return angle > 0.0  ;
    }

    private void LineToQuad(PLine line, List<Vector3> vertices, List<Vector4> uv,List<int> indices, double d){
        
    }

    // public static List<Segment> outlineLine(Segment c, Shape shape, params double[] d) {

    // }

    public static List<List<Segment>> outline(Segment c, Shape shape, params double[] d) {
        List<List<Segment>> output = new List<List<Segment>>();
        List<Segment> segments = new List<Segment>();
        List<Segment> lineJoin = new List<Segment>();

        // Outline PLine then return
        if(c.GetType() == typeof(PLine)){
            PLine p = (PLine)c;
            Vector2d v0 = p.StartPoint + p.NormalAt(0, 1) * d[0];
            Vector2d v1 = p.EndPoint + p.NormalAt(0, 1) * d[0];
            Vector2d v2 = p.EndPoint + p.NormalAt(0, -1) * d[0];
            Vector2d v3 = p.StartPoint + p.NormalAt(0, -1) * d[0];
            PQuad q = new PQuad(v3, v0, v1, v2);

            segments.Add(q);
            lineJoin.Add(q);
            output.Add(segments);
            output.Add(lineJoin);

            return output;
        }
        
        double d2 = d.Length > 1 ? d[1] : d[0];
        List<List<Segment>> list = reduce(c);
        List<Segment> reduced1 = list[0];
        List<Segment> reduced2 = list[1];

        
        int len = reduced2.Count;
        List<Segment> fcurves = new List<Segment>();
        List<Segment> bcurves = new List<Segment>();
        double alen = 0.0;
        double tlen = length(c, 10);

        //TODO: Fix graduated stroke
        bool graduated = /*d.Length == 4 */ false;

        // Add a circle at each extrema, except at start and end point
        Vector2d start = reduced1[0].StartPoint;
        Vector2d end = reduced1[reduced1.Count-1].EndPoint;
        for(int i=0; i<reduced1.Count-1; i++){
            Vector2d center = reduced1[i].EndPoint;
            double sr = d2 * d2;
            if((start-center).sqrMagnitude > sr && (center-end).sqrMagnitude > sr)
            segments.Add(new PCirFsix((float)d2, center.f()));
        }

        // Generate stroke by scaling curve in two sides and add fill quad
        foreach (Segment segment in reduced2) {
            double slen = length(segment, 10);
            
            if (graduated) {
                fcurves.Add(
                        scale(segment, 0.0, new LinearDistanceFunction(d[0], d[2], tlen, alen, slen)));
                bcurves.Add(
                        scale(segment, 0.0, new LinearDistanceFunction(-d2, -d[4], tlen, alen, slen)));
            } else {
                Segment f = scale(segment, d[0], null);
                Segment b = scale(segment, -d2, null);
                f.cut = Clockwise(f) ? false : true;
                b.cut = Clockwise(b) ? true : false;

                // Add Filling Rectangle between front and back curves
                segments.Add(new PQuad(f.StartPoint, f.EndPoint, b.EndPoint, b.StartPoint));
                fcurves.Add(f);
                bcurves.Add(b);
            }
            alen += slen;
        }

        foreach (Segment segment in bcurves)
            segment.reverse();
        bcurves.Reverse();

        // form the endcaps as lines
        Vector2d fs = fcurves[0].getPoints()[0];
        Vector2d fe = fcurves[len - 1].getPoints()[fcurves[len - 1].getPoints().Length - 1];
        Vector2d bs = bcurves[len - 1].getPoints()[bcurves[len - 1].getPoints().Length - 1];
        Vector2d be = bcurves[0].getPoints()[0];
        PCubic le = makeline(fe, be);
        PCubic ls = makeline(bs, fs);
        segments.AddRange(fcurves);
        segments.Add(new PLine(be));
        segments.AddRange(bcurves);
        segments.Add(new PLine(fs));

        PQuad quad = new PQuad(bcurves.Last().EndPoint, fcurves.First().StartPoint, fcurves.Last().EndPoint, bcurves.First().StartPoint);
        lineJoin.Add(quad);

        output.Add(segments);
        output.Add(lineJoin);

        return output;
    }

    public static DoubleArray inflections(PCubic cb) {
        // FIXME: TODO: add in inflection abstraction for quartic+ curves?
        DoubleArray arr = new DoubleArray();
        Vector2d[] points = cb.points;
        List<Vector2d> pp = Align(cb, new Line(points[0], points[3]));
        Vector2d[] p = new Vector2d[] { pp[0], pp[1], pp[2], pp[3] };
        double a = p[2].x * p[1].y, b = p[3].x * p[1].y, c = p[1].x * p[2].y, d = p[3].x * p[2].y,
                v1 = 18.0 * (-3.0 * a + 2.0 * b + 3.0 * c - d), v2 = 18.0 * (3.0 * a - b - 3.0 * c),
                v3 = 18.0 * (c - a);

        if (approximately(v1, 0.0, epsilon)) {
            if (!approximately(v2, 0.0, epsilon)) {
                double t = -v3 / v2;
                if (0.0 <= t && t <= 1.0) {
                    arr.add(t);
                    return arr;
                }
            }
            return arr;
        }

        double trm = v2 * v2 - 4.0 * v1 * v3;
        double sq = Mathd.Sqrt(trm);
        d = 2.0 * v1;

        if (approximately(d, 0.0, epsilon)) {
            return arr;
        }

        double t1 = (sq - v2) / d;
        double t2 = -(v2 + sq) / d;
        if (t1 >= 0.0 && t1 <= 1.0)
            arr.add(t1);
        if (t2 >= 0.0 && t2 <= 1.0)
            arr.add(t2);
        return arr;
    }

    internal static void ComputeQuadratic(PQuadratic seg, List<Vector3> vertices, List<Vector4> uv, List<int> indices, float z) {
        vertices.Add(seg.StartPoint.f3(z));
        vertices.Add(seg.Ctrl1.f3(z));
        vertices.Add(seg.EndPoint.f3(z));
        uv.Add(new Vector4(0, 0, 0, seg.cut == true ? 8 : 2));
        uv.Add(new Vector4(.5f, 0, 0, seg.cut == true ? 8 : 2));
        uv.Add(new Vector4(1, 1, 0, seg.cut == true ? 8 : 2));
        indices.Add(indices.Count);
        indices.Add(indices.Count);
        indices.Add(indices.Count);
    }

    internal static void ComputeArc(PArc arc, List<Vector3> vertices, List<Vector4> uv, List<int> indices) {
        //float a = arc.Direction == PArc.SweepDirection.ANTI_CLOCKWISE ? (float)arc.AngleExtent : -(float)arc.AngleExtent;
        arc.computeArc();
        float a = Mathf.Abs((float)arc.AngleExtent);
        float d = arc.Direction == PArc.SweepDirection.ANTI_CLOCKWISE ? 1 : -1;
        Debug.Log("extend: "+ a);
        Vector3 v1 = new Vector3d(arc.CenterX - arc.RadiusX, arc.CenterY - arc.RadiusY, 1).f();
        Vector3 v2 = new Vector3d(arc.CenterX + arc.RadiusX, arc.CenterY - arc.RadiusY, 1).f();
        Vector3 v3 = new Vector3d(arc.CenterX + arc.RadiusX, arc.CenterY + arc.RadiusY, 1).f();
        Vector3 v4 = new Vector3d(arc.CenterX - arc.RadiusX, arc.CenterY + arc.RadiusY, 1).f();
        v1 = Rotate((float)arc.CenterX, (float)arc.CenterY, v1, (float)arc.AngleStart);
        v2 = Rotate((float)arc.CenterX, (float)arc.CenterY, v2, (float)arc.AngleStart);
        v3 = Rotate((float)arc.CenterX, (float)arc.CenterY, v3, (float)arc.AngleStart);
        v4 = Rotate((float)arc.CenterX, (float)arc.CenterY, v4, (float)arc.AngleStart);

        vertices.Add(v1);
        uv.Add(new Vector4(-1, -1*d, a, 3));
        indices.Add(indices.Count);
        vertices.Add(v2);
        uv.Add(new Vector4(1, -1 * d, a, 3));
        indices.Add(indices.Count);
        vertices.Add(v3);
        uv.Add(new Vector4(1, 1 * d, a, 3));
        indices.Add(indices.Count);

        vertices.Add(v1);
        uv.Add(new Vector4(-1, -1 * d, a, 3));
        indices.Add(indices.Count);
        vertices.Add(v3);
        uv.Add(new Vector4(1, 1 * d, a, 3));
        indices.Add(indices.Count);
        vertices.Add(v4);
        uv.Add(new Vector4(-1, 1 * d, a, 3));
        indices.Add(indices.Count);

    }

   

    private static Vector3 Rotate(float cx, float cy, Vector3 p, float a) {
        return new Vector3(((p.x - cx) * Mathf.Cos(a * Mathf.Deg2Rad) - (p.y - cy) * Mathf.Sin(a * Mathf.Deg2Rad)) +cx,
                            ((p.y - cy) * Mathf.Cos(a * Mathf.Deg2Rad) + (p.x - cx) * Mathf.Sin(a * Mathf.Deg2Rad)) +cy,
                             p.z);
    }
}