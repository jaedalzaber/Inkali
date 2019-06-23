using UnityEngine;

public class OffsetResult {
    public Vector2d c;
    public Vector2d n;
    public Vector2d xy;

    public OffsetResult(Vector2d c, double[] n, double d) {
        this.c = c;
        this.n = new Vector2d(n[0], n[1]);
        this.xy = new Vector2d(c.x + n[0] * d, c.y + n[1] * d);
    }

}
