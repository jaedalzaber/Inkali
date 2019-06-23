using UnityEngine;

public abstract class Paint {
    public bool update = true;
    public abstract Material CreateMaterial();
    public abstract void Update(Material m);
    public abstract void setOpacity(float a);
}