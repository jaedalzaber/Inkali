using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PathUtils;

public class Result{

	public float fd1=0,fd2=0,fd3=0;
	public CurveType curve_type;

	public Result(float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3){
		curve_type = determineType (x0, y0, x1, y1, x2, y2, x3, y3);
	}

	public CurveType determineType(float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3){
		float d3 = 0;
		float d1 = 0;
		float d2 = 0;
		Vector3 b0 = new Vector3(x0, y0, 1);
		Vector3 b1 = new Vector3(x1, y1, 1);
		Vector3 b2 = new Vector3(x2, y2, 1);
		Vector3 b3 = new Vector3(x3, y3, 1);

		float a1 = Vector3.Dot(b0, Vector3.Cross(b3, b2));
		float a2 = Vector3.Dot(b1, Vector3.Cross(b0, b3));
		float a3 = Vector3.Dot(b2, Vector3.Cross(b1, b0));

		d1 = ((a1 - (2 * a2)) + (3 * a3));
		d2 = ((a2 * -1) + (3 * a3));
		d3 = (3 * a3);
		Vector3 d = new Vector3(d1, d2, d3);
		d.Normalize();
		d1 = d.x;
		d2 = d.y;
		d3 = d.z;
		fd1 = d1;
		fd2 = d2;
		fd3 = d3;
		float D = ((3 
			* (d2 * d2)) - (4 
				* (d1 * d3)));
		float disc = (d1 
			* (d1 * D));
	//	Debug.Log ("d1: "  + (d1 + (",    d2: "  + (d2 + (",    d3: "  + (d3 + (",    disc: " + disc)))))));
		if ((disc == 0.0f)) {
			if (((d1 == 0.0f) 
				&& (d2 == 0.0f))) {
				if ((d3 == 0.0f)) {
					return CurveType.LINE;
				}
				return CurveType.QUADRATIC;
			}

			if ((d1 == 0.0f)) {
				return CurveType.CUSP;
			}

			if ((D < 0.0f)) {
				return CurveType.LOOP;
			}

		}

		if ((disc > 0.0f)) {
			return CurveType.SERPENTINE;
		}
		return CurveType.LOOP; 
	}
}
