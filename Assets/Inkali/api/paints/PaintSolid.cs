using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintSolid : Paint {
    private Color color;

    public Color Color { get {
            return color;
        } set {
            color = value;
            update = true;
        }
    } 

    public PaintSolid(Color color) {
        this.color = color;
    }

    public override void Update(Material m) {
        m.SetColor("_inColor", color);
        update = false;
    }

    public override Material CreateMaterial() {
        return new Material(Shader.Find("Unlit/cubic_shader"));
    }

    public override void setOpacity(float a) {
        color.a = Mathf.Clamp(a, 0.0f, 1.0f);
        update = true;
    }
}
