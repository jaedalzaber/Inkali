using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex {

		public Vector2 xy;

		public Vector4 coords;
		public Vector2 uv2;

		public Vertex(Vector2 xy, Vector4 coords,  Vector2 uv2) {
			this.xy = xy;
			this.coords = coords;
			this.uv2 = uv2;
		}

		public Vertex(float x, float y, float k, float l, float m, float t, float u, float v) {
			this.xy = new Vector2(x, y);
			this.coords = new Vector4(k, l, m, t);
			this.uv2 = new Vector2(u, v);
		}

    public Vertex(Vector2 xy) {
        this.xy = xy;
        this.coords = new Vector4(1, 1, 1, 0);
		this.uv2 = new Vector2(1, 1);
    }

    public Vertex() {
        this.xy = new Vector2(0, 0);
        this.coords = new Vector4(1,1,1, 0);
		this.uv2 = new Vector2(1, 1);
    }

    public Vertex(float x, float y) {
			this.xy = new Vector2(x, y);
			this.coords = new Vector4(1, 1, 1, 0);
			this.uv2 = new Vector2(1, 1);
		}

		public float[] getValues() {
			return new float[] {
				this.xy.x,
				this.xy.y,
				1,
				this.coords.x,
				this.coords.y,
				this.coords.z,
				this.uv2.x,
				this.uv2.y};
		}

    public void Set(float x, float y, float a, float b, float c, float t, float u, float v) {
        xy.Set(x, y);
        coords.Set(a, b, c, t);
		uv2.Set(u, v);
    }

	public Vector2 cpy(){
		return new Vector2 (xy.x, xy.y);
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
			return new Vertex(this.xy.x, this.xy.y, (this.coords.x * -1), (this.coords.y * -1), this.coords.z, this.coords.w, this.uv2.x, this.uv2.y);
		}
	}