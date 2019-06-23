using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]

public class QuadCurve : MonoBehaviour {
	Mesh mesh;

	Vector3[] vertices;
	Vector2[] uv;
	int[] tris;
	void Awake(){
		mesh = GetComponent<MeshFilter> ().mesh;
	}

	// Use this for initialization
	void Start () {
		MakeMesh ();
		CreateMesh ();
	}
	
	void MakeMesh(){
		vertices = new Vector3[]{ new Vector3(-8,-4,0), new Vector3(-4,4,0), new Vector3(0,-4,0)};
		uv = new Vector2[]{ new Vector2(0,0), new Vector2(.5f,0), new Vector2(1,1)};
		tris = new int[]{ 0,1,2};
	}

	void CreateMesh(){
		mesh.Clear ();
		mesh.vertices = vertices;
		mesh.triangles = tris;
		mesh.uv = uv;
	}
}
