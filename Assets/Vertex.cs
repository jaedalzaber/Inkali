using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex {

		public Vector2 xy;

		public Vector4 coords;

		public Vertex(Vector2 xy, Vector4 coords) {
			this.xy = xy;
			this.coords = coords;
		}

		public Vertex(float x, float y, float k, float l, float m, float t) {
			this.xy = new Vector2(x, y);
			this.coords = new Vector4(k, l, m, t);
		}

    public Vertex(Vector2 xy) {
        this.xy = xy;
        this.coords = new Vector4(1, 1, 1, 0);
    }

    public Vertex() {
        this.xy = new Vector2(0, 0);
        this.coords = new Vector4(1,1,1, 0);
    }

    public Vertex(float x, float y) {
			this.xy = new Vector2(x, y);
			this.coords = new Vector3(1, 1, 1);
		}

		public float[] getValues() {
			return new float[] {
				this.xy.x,
				this.xy.y,
				50,
				this.coords.x,
				this.coords.y,
				this.coords.z};
		}

    public void Set(float x, float y, float a, float b, float c, float t) {
        xy.Set(x, y);
        coords.Set(a, b, c, t);
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
			return new Vertex(this.xy.x, this.xy.y, (this.coords.x * -1), (this.coords.y * -1), this.coords.z, this.coords.w);
		}
	}