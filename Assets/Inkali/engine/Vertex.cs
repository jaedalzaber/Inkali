using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex {

		public Vector3 xyz;

		public Vector4 coords;

		public Vertex(Vector3 xyz, Vector4 coords) {
			this.xyz = xyz;
			this.coords = coords;
		}

		public Vertex(float x, float y, float z, float k, float l, float m, float t) {
			this.xyz = new Vector3(x, y, z);
			this.coords = new Vector4(k, l, m, t);
		}

    public Vertex(Vector3 xyz) {
        this.xyz = xyz;
        this.coords = new Vector4(1, 1, 1, 0);
    }

    public Vertex() {
        this.xyz = new Vector3(0, 0, 0);
        this.coords = new Vector4(1,1,1, 0);
    }

    public Vertex(float x, float y, float z) {
			this.xyz = new Vector3(x, y, z);
			this.coords = new Vector4(1, 1, 1, 0);
		}

		public float[] getValues() {
			return new float[] {
				this.xyz.x,
				this.xyz.y,
				this.xyz.z,
				this.coords.x,
				this.coords.y,
				this.coords.z};
		}

    public void Set(float x, float y, float z, float a, float b, float c, float t) {
        xyz.Set(x, y, z);
        coords.Set(a, b, c, t);
    }

	public Vector3 cpy(){
		return new Vector3 (xyz.x, xyz.y, xyz.z);
	}

		/*public Vertex print() {
			System.out.println((this.xy.x + ("f," 
				+ (this.xy.y + ("f," + (50 + ("f," 
					+ (this.coords.x + ("f," 
						+ (this.coords.y + ("f," 
							+ (this.coords.z + "f,"))))))))))));
			return this;
		}*/

		public Vertex copyFlip() {
			return new Vertex(this.xyz.x, this.xyz.y, this.xyz.z, (this.coords.x * -1), (this.coords.y * -1), this.coords.z, this.coords.w);
		}
	}