using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class CubicCurve : MonoBehaviour {

	Mesh mesh;
	public List<GameObject> points;
	public List<Vector3> vertices;
	public List<Vector4> uv;
    public List<int> indices;
    int[] triangles;
    BBoxResult bbox;

	void Awake(){
		mesh = GetComponent<MeshFilter> ().mesh;
        mesh.subMeshCount = 2;

    }


	// Use this for initialization
	void Start () {
		MakeMesh ();
		CreateMesh ();
        mesh.subMeshCount = 2;
    }

	void MakeMesh(){
		CubicBezier (
			new Vector2 (points[0].transform.position.x, points[0].transform.position.y), 
			new Vector2 (points[1].transform.position.x, points[1].transform.position.y), 
			new Vector2 (points[2].transform.position.x, points[2].transform.position.y), 
			new Vector2 (points[3].transform.position.x, points[3].transform.position.y));
		
		vertices = new List<Vector3>();
		uv = new List<Vector4> ();
        indices = new List<int>();
        
	}

	void CreateMesh(){
		vertices.Clear ();
		uv.Clear();

        triangles = new int[tris.Count];
        int i = 0;
		foreach(Vertex v in tris){
			vertices.Add(new Vector3 (v.xy.x, v.xy.y, 0));
			uv.Add (v.coords);
			triangles [i] = i;
			i++;
		}
		mesh.Clear ();
		mesh.SetVertices(vertices);
		mesh.SetUVs(0, uv);
		mesh.triangles = triangles;
	}
    Vector3 v1 = new Vector3();
    Vector3 v2 = new Vector3();
    Vector3 v3 = new Vector3();
    Vector3 v4 = new Vector3();
    int[] idx = new int[6];
    void CreateMesh2() {
        bbox = PathUtils.bbox(c);
        v1.Set((float)bbox.x.min, (float)bbox.y.min, 1);
        v2.Set((float)bbox.x.min, (float)bbox.y.max, 1);
        v3.Set((float)bbox.x.max, (float)bbox.y.max, 1);
        v4.Set((float)bbox.x.max, (float)bbox.y.min, 1);
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        vertices.Add(v4);
        uv.Add(new Vector3(1, 1, 1));
        uv.Add(new Vector3(1, 1, 1));
        uv.Add(new Vector3(1, 1, 1));
        uv.Add(new Vector3(1, 1, 1));
        idx[0] = indices.Count;
        idx[1] = indices.Count+1;
        idx[2] = indices.Count+2;
        idx[3] = indices.Count;
        idx[4] = indices.Count+2;
        idx[5] = indices.Count+3;

        mesh.Clear();
        mesh.subMeshCount = 2;
        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uv);
        mesh.SetTriangles(indices.ToArray(), 0);
        mesh.SetTriangles(indices.ToArray(), 1);
    }



    public enum CurveType {
		UNKNOWN, SERPENTINE, LOOP, CUSP, QUADRATIC, LINE,
	}

	//     private CurveType curveType;
	private float[] klm;

	public bool drawAdditionalTri;

	public InkList<Vertex> tris;


	public float M_EPS;

	public Vector2 start;

	public Vector2 ctrl1;

	public Vector2 ctrl2;

	public Vector2 end;

	bool once = true;
    PCubic c;

	public void CubicBezier(Vector2 start, Vector2 ctrl1, Vector2 ctrl2, Vector2 end) {
		this.start = start;
		this.ctrl1 = ctrl1;
		this.ctrl2 = ctrl2;
		this.end = end;
        c = new PCubic(start, ctrl1, ctrl2, end);

		this.drawAdditionalTri = false;
		this.tris = new InkList<Vertex>();
		this.compute(this.start.x, this.start.y, this.ctrl1.x, this.ctrl1.y, this.ctrl2.x, this.ctrl2.y, this.end.x, this.end.y, -1);
	}

	

	void Update(){
		this.start.Set (points[0].transform.position.x-.5f, points[0].transform.position.y + .25f);
		this.ctrl1.Set (points[1].transform.position.x-.5f, points[1].transform.position.y + .25f);
		this.ctrl2.Set (points[2].transform.position.x-.5f, points[2].transform.position.y + .25f);
		this.end.Set (points[3].transform.position.x-.5f, points[3].transform.position.y + .25f);
        //Debug.Log(start.x + ", " + start.y);
        //Debug.Log(ctrl1.x + ", " + ctrl1.y);
        //Debug.Log(ctrl2.x + ", " + ctrl2.y);
        //Debug.Log(end.x + ", " + end.y);
        this.compute ();
		this.CreateMesh2 ();
	}

   
	public void compute() {
		this.tris.Clear();
		this.drawAdditionalTri = false;
        c.set(start, ctrl1, ctrl2, end);
        vertices.Clear();
        uv.Clear();
        indices.Clear();
        Vector2d point = PathUtils.ComputeCubic(c, vertices, uv, indices);
        //this.compute(this.start.x, this.start.y, this.ctrl1.x, this.ctrl1.y, this.ctrl2.x, this.ctrl2.y, this.end.x, this.end.y, -1);
        //         subdivide(start.x, start.y, ctrl1.x, ctrl1.y, ctrl2.x, ctrl2.y, end.x, end.y, .5f);
    }

    bool flipSerpent = false;

	private CurveType curve_type;

	public void compute(float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3, int recursiveType) {
		Result res = new Result (x0, y0, x1, y1, x2, y2, x3, y3);
		this.curve_type = res.curve_type;

		float d3 = res.fd3;
		float d1 = res.fd1;
		float d2 = res.fd2;
//		System.out.println(this.curve_type);
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
		this.klm = new float[12];
		for (int i = 0; (i < 12); i++) {
			this.klm[i] = 0;
		}

		//         if(intersect(vertices[0].xy, vertices[3].xy, vertices[1].xy, vertices[2].xy))
		int o = this.orientation(this.start, this.ctrl1, this.ctrl2, this.end);
	//	int oo = this.orientation(this.ctrl1, this.ctrl2, this.end);
		//         System.out.println("Orientation: "+o);
		//         System.out.println("Orientation other: "+oo);
		if ((o == -1)) {
			d1 = (d1 * -1);
			d2 = (d2 * -1);
			d3 = (d3 * -1);
		}

		int error = this.insideTriangle();
		switch (this.curve_type) {
		case CurveType.UNKNOWN:
			break;
		case CurveType.SERPENTINE:
			t1 = (float)(Mathf.Sqrt(9.0f * d2 * d2 - 12.0f * d1 * d3));
			ls = ((3.0f * d2) - t1);
			lt = (6.0f * d1);
			ms = ((3.0f * d2) + t1);
			mt = lt;
			ltMinusLs = (lt - ls);
			mtMinusMs = (mt - ms);
		//	if ((error != -1)) {
		//		errorLoop = (error == 0)?2:1;
		//		splitParam = 0.5f;
		//	}

			this.klm[0] =   ls * ms;
			this.klm[1] =   ls * ls * ls;
			this.klm[2] =   ms * ms * ms;

			this.klm[3] =   OneThird * (3.0f * ls * ms - ls * mt - lt * ms);
			this.klm[4] =   ls * ls * (ls - lt);
			this.klm[5] =   ms * ms * (ms - mt);

			this.klm[6] =   OneThird * (lt * (mt - 2.0f * ms) + ls * (3.0f * ms - 2.0f * mt));
			this.klm[7] =   ltMinusLs * ltMinusLs * ls;
			this.klm[8] =   mtMinusMs * mtMinusMs * ms;

			this.klm[9] =   ltMinusLs * mtMinusMs;
			this.klm[10] =   -(ltMinusLs * ltMinusLs * ltMinusLs);
			this.klm[11] =   -(mtMinusMs * mtMinusMs * mtMinusMs);


			if ((d1 < 0.0f)) {
				flip = true;
				this.flipSerpent = true;
			}

			break;
		case CurveType.LOOP:
			t1 = (float)(Mathf.Sqrt(4.0f * d1 * d3 - 3.0f * d2 * d2));
			ls = (d2 - t1);
			lt = (2.0f * d1);
			ms = (d2 + t1);
			mt = lt;
			//  Figure out whether there is a rendering artifact requiring
			//  the curve to be subdivided by the caller.
			ql = (ls / lt);
			qm = (ms / mt);
	//		int o2 = this.orientation(this.start, this.ctrl1, this.end);
	//		System.out.println(("qm: "  + (qm + "  -----------------------")));
	//		System.out.println(("ql: "  + (ql + "  -----------------------")));

			if (0.0f < ql && ql < 1.0f) 
			{
				errorLoop = 1;
				splitParam = ql;
				//std::cout << "error loop 1\n";
			}

			if (0.0f < qm && qm < 1.0f) 
			{
				errorLoop = 2;
				splitParam = qm;

				//std::cout << "error loop 2\n";
			}
		
			ltMinusLs = lt - ls;
			mtMinusMs = mt - ms;
			
			this.klm[0] =   ls * ms;
			this.klm[1] =   ls * ls * ms;
			this.klm[2] =   ls * ms * ms;
			this.klm[3] =   OneThird * (-ls * mt - lt * ms + 3.0f * ls * ms);
			this.klm[4] =   -OneThird * ls * (ls * (mt - 3.0f * ms) + 2.0f * lt * ms);
			this.klm[5] =   -OneThird * ms * (ls * (2.0f * mt - 3.0f * ms) + lt * ms);
			this.klm[6] =   OneThird * (lt * (mt - 2.0f * ms) + ls * (3.0f * ms - 2.0f * mt));
			this.klm[7] =   OneThird * (lt - ls) * (ls * (2.0f * mt - 3.0f * ms) + lt * ms);
			this.klm[8] =   OneThird * (mt - ms) * (ls * (mt - 3.0f * ms) + 2.0f * lt * ms);
			this.klm[9] =   ltMinusLs * mtMinusMs;
			this.klm[10] =   -(ltMinusLs * ltMinusLs) * mtMinusMs;
			this.klm[11] =   -ltMinusLs * mtMinusMs * mtMinusMs;
			
			if (recursiveType == -1)
				flip = ((d1 > 0.0f && klm[0] < 0.0f) || (d1 < 0.0f && klm[0] > 0.0f));
			break;

		case CurveType.CUSP:
			ls = d3;
			lt = 3.0f * d2;
			lsMinusLt = ls - lt;
			this.klm[0] = ls;
			this.klm[1] = ls * ls * ls;
			this.klm[2] = 1.0f;
			this.klm[3] = ls - OneThird * lt;
			this.klm[4] = ls * ls * lsMinusLt;
			this.klm[5] = 1.0f;
			this.klm[6] = ls - TwoThirds * lt;
			this.klm[7] = lsMinusLt * lsMinusLt * ls;
			this.klm[8] = 1.0f;
			this.klm[9] = lsMinusLt;
			this.klm[10] = lsMinusLt * lsMinusLt * lsMinusLt;
			this.klm[11] = 1.0f;
			break;
		case CurveType.QUADRATIC:
			this.klm[0] = 0;
			this.klm[1] = 0;
			this.klm[2] = 0;
			this.klm[3] = OneThird;
			this.klm[4] = 0;
			this.klm[5] = OneThird;
			this.klm[6] = TwoThirds;
			this.klm[7] = OneThird;
			this.klm[8] = TwoThirds;
			this.klm[9] = 1;
			this.klm[10] = 1;
			this.klm[11] = 1;
			if ((d3 < 0.0f)) {
				flip = true;
			}

			break;
		case CurveType.LINE:
			break;
		}
		//         && orientation(start, ctrl1, end)>=0 && orientation(ctrl2, end, start)>=0
		if (((errorLoop != -1) 
			&& ((recursiveType == -1) 
				&& (d1 > 0.0f)))) {
			//System.out.println(("error: "  + (errorLoop + (", splitparam: " + splitParam))));
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
			this.drawAdditionalTri = true;
			this.tris.Add(new Vertex(x0, y0, 1, 1, 1, 0));
			this.tris.Add(new Vertex(x0123, y0123, 1, 1, 1,0));
			this.tris.Add(new Vertex(x3, y3, 1, 1, 1,0));
			//             tris.Add(new Vertex(x0, y0, 1, 1, 1));
			//             tris.Add(new Vertex(x0123, y0123, 1, 1, 1));
			//             tris.Add(new Vertex( x012, y012, 1, 1, 1));
			if ((errorLoop == 1)) {
				//  flip second
				this.compute(x0, y0, x01, y01, x012, y012, x0123, y0123, 0);
				this.compute(x0123, y0123, x123, y123, x23, y23, x3, y3, 1);
			}
			else if ((errorLoop == 2)) {
				//  flip first
				this.compute(x0, y0, x01, y01, x012, y012, x0123, y0123, 1);
				this.compute(x0123, y0123, x123, y123, x23, y23, x3, y3, 0);
			}

			// 
			return;
		}
		else if (((errorLoop == -1) 
			&& (recursiveType == -1))) {
			this.drawAdditionalTri = false;
		}

		if ((recursiveType == 1)) {
			flip = !flip;
		}

		//         if(o == -1 && curve_type==CurveType.SERPENTINE)
		//             flip = !flip;
		if (flip) {
			this.klm[0] = (this.klm[0] * -1.0f);
			this.klm[1] = (this.klm[1] * -1.0f);
			this.klm[3] = (this.klm[3] * -1.0f);
			this.klm[4] = (this.klm[4] * -1.0f);
			this.klm[6] = (this.klm[6] * -1.0f);
			this.klm[7] = (this.klm[7] * -1.0f);
			this.klm[9] = (this.klm[9] * -1.0f);
			this.klm[10] = (this.klm[10] * -1.0f);

		}
	////	Debug.Log (klm[0]);
	//	Debug.Log (klm[1]);
	//	Debug.Log (klm[2]);
	//	Debug.Log (klm[3]);
	//	Debug.Log (klm[4]);
	//	Debug.Log (klm[5]);
	//	Debug.Log (klm[6]);
	//	Debug.Log (klm[7]);
	//	Debug.Log (klm[8]);
	//	Debug.Log (klm[9]);
	//	Debug.Log (klm[10]);
	//	Debug.Log (klm[11]);

		this.Triangulation(x0, y0, x1, y1, x2, y2, x3, y3, this.klm);
	}

	private int insideTriangle() {
		Vertex[] vertices = new Vertex[4];
		vertices[0] = new Vertex(this.start.x, this.start.y);
		vertices[1] = new Vertex(this.ctrl1.x, this.ctrl1.y);
		vertices[2] = new Vertex(this.ctrl2.x, this.ctrl2.y);
		vertices[3] = new Vertex(this.end.x, this.end.y);
		for (int i = 0; (i < 4); i++) {
			int[] indices = new int[] {
				0,
				0,
				0};
			int index = 0;
			for (int j = 0; (j < 4); j++) {
				if ((i != j)) {
					indices[index++] = j;
				}

			}

			if (this.pointInTriangle(vertices[i].xy, vertices[indices[0]].xy, vertices[indices[1]].xy, vertices[indices[2]].xy)) {
				if (((i == 0) 
					|| (i == 3))) {
					return i;
				}

			}

		}

		return -1;
	}

	public bool approxEqual(Vector2 v0, Vector2 v1) {
		return ((v0 - v1).sqrMagnitude
			< (this.M_EPS * this.M_EPS));
	}

	public bool pointInTriangle(Vector2 point, Vector2 a, Vector2 b, Vector2 c) {
		float x0 = (c.x - a.x);
		float y0 = (c.y - a.y);
		float x1 = (b.x - a.x);
		float y1 = (b.y - a.y);
		float x2 = (point.x - a.x);
		float y2 = (point.y - a.y);
		float Dot00 = ((x0 * x0) + (y0 * y0));
		float Dot01 = ((x0 * x1) + (y0 * y1));
		float Dot02 = ((x0 * x2) + (y0 * y2));
		float Dot11 = ((x1 * x1) + (y1 * y1));
		float Dot12 = ((x1 * x2) + (y1 * y2));
		float denominator = ((Dot00 * Dot11) - (Dot01 * Dot01));
		if ((denominator == 0.0f)) {
			return false;
		}

		float inverseDenominator = (1.0f / denominator);
		float u = (((Dot11 * Dot02) 
			- (Dot01 * Dot12)) 
			* inverseDenominator);
		float v = (((Dot00 * Dot12) 
			- (Dot01 * Dot02)) 
			* inverseDenominator);
		return (u > 0.0f) && (v > 0.0f) && ((u+v) < 1.0f);
	}

	public int orientation(Vector2 p1, Vector2 p2, Vector2 p3) {
		float crossProduct = (((p2.y - p1.y) 
			* (p3.x - p2.x)) 
			- ((p3.y - p2.y) 
				* (p2.x - p1.x)));
		//         System.out.println("cross product: " + crossProduct);
		return (crossProduct < 0.0f) ? -1 : ((crossProduct > 0.0f) ? 1 : 0);
	}

	public bool intersect(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2) {
		return ((this.orientation(p1, q1, p2) != this.orientation(p1, q1, q2)) 
			&& (this.orientation(p2, q2, p1) != this.orientation(p2, q2, q1)));
	}

	public int orientation(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2) {
		int o1 = this.orientation(p1, q1, p2);
		int o2 = this.orientation(p1, q1, q2);
		int o3 = this.orientation(p2, q2, p1);
		int o4 = this.orientation(p2, q2, q1);
		if ((((o1 >= 0.0f)  && ((o2 >= 0.0f)  && (o3 >= 0.0f))) 
			|| (((o1 >= 0.0f)  && ((o2 >= 0.0f)  && (o3 < 0.0f))) 
				|| (((o1 < 0.0f)  && ((o2 >= 0.0f)  && (o3 >= 0.0f))) 
					|| ((o1 >= 0.0f)  && ((o2 < 0.0f) && (o3 >= 0.0f))))))) {
			return 1;
		}

		return -1;
	}


	public void Triangulation(float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3, float[] klm) {
		//         tris.clear();
		Vertex[] vertices = new Vertex[4];
		vertices[0] = new Vertex(x0, y0, this.klm[0], this.klm[1], this.klm[2], 1);
		vertices[1] = new Vertex(x1, y1, this.klm[3], this.klm[4], this.klm[5], 1);
		vertices[2] = new Vertex(x2, y2, this.klm[6], this.klm[7], this.klm[8], 1);
		vertices[3] = new Vertex(x3, y3, this.klm[9], this.klm[10], this.klm[11], 1);
		for (int i = 0; (i < 4); i++) {
			for (int j = (i + 1); (j < 4); j++) {
				if (this.approxEqual(vertices[i].xy, vertices[j].xy)) {
					int[] indices = new int[] { 0, 0, 0};
					int index = 0;
					for (int k = 0; (k < 4); k++) {
						if ((k != j)) {
							indices[index++] = k;
						}

					}

					this.tris.Add(vertices[indices[0]]);
					this.tris.Add(vertices[indices[1]]);
					this.tris.Add(vertices[indices[2]]);
					return;
				}

			}

		}

		for (int i = 0; (i < 4); i++) {
			int[] indices = new int[] { 0, 0, 0};
			int index = 0;
			for (int j = 0; (j < 4); j++) {
				if ((i != j)) {
					indices[index++] = j;
				}

			}

			if (this.pointInTriangle(vertices[i].xy, vertices[indices[0]].xy, vertices[indices[1]].xy, vertices[indices[2]].xy)) {
				for (int j = 0; (j < 3); j++) {
					this.tris.Add(vertices[indices[(j % 3)]]);
					this.tris.Add(vertices[indices[((j + 1) 
						% 3)]]);
					this.tris.Add(vertices[i]);
				}

				return;
			}

		}

		if (this.intersect(vertices[0].xy, vertices[2].xy, vertices[1].xy, vertices[3].xy)) {
			if ((vertices[2].cpy() - vertices[0].xy).sqrMagnitude < (vertices[3].cpy() - vertices[1].xy).sqrMagnitude) {
				this.tris.Add(vertices[0]);
				this.tris.Add(vertices[1]);
				this.tris.Add(vertices[2]);
				this.tris.Add(vertices[0]);
				this.tris.Add(vertices[2]);
				this.tris.Add(vertices[3]);
			}
			else {
				this.tris.Add(vertices[0]);
				this.tris.Add(vertices[1]);
				this.tris.Add(vertices[3]);
				this.tris.Add(vertices[1]);
				this.tris.Add(vertices[2]);
				this.tris.Add(vertices[3]);
			}
		}
		else if (this.intersect(vertices[0].xy, vertices[3].xy, vertices[1].xy, vertices[2].xy)) {
			if ((vertices[3].cpy() - vertices[0].xy).sqrMagnitude < (vertices[2].cpy() - vertices[1].xy).sqrMagnitude) {
				this.tris.Add(vertices[0]);
				this.tris.Add(vertices[1]);
				this.tris.Add(vertices[3]);
				this.tris.Add(vertices[0].copyFlip());
				this.tris.Add(vertices[3].copyFlip());
				this.tris.Add(vertices[2].copyFlip());
			}
			else {
				this.tris.Add(vertices[0]);
				this.tris.Add(vertices[1]);
				this.tris.Add(vertices[3]);
				this.tris.Add(vertices[2].copyFlip());
				this.tris.Add(vertices[0].copyFlip());
				this.tris.Add(vertices[3].copyFlip());
			}

		}
		else if ((vertices[1].cpy() - vertices[0].xy).sqrMagnitude < (vertices[3].cpy() - vertices[2].xy).sqrMagnitude) {
			this.tris.Add(vertices[0]);
			this.tris.Add(vertices[2]);
			this.tris.Add(vertices[1]);
			this.tris.Add(vertices[0]);
			this.tris.Add(vertices[1]);
			this.tris.Add(vertices[3]);
		}
		else {
			this.tris.Add(vertices[0]);
			this.tris.Add(vertices[2]);
			this.tris.Add(vertices[3]);
			this.tris.Add(vertices[3]);
			this.tris.Add(vertices[2]);
			this.tris.Add(vertices[1]);
		}

	}

	

}
