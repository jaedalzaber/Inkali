using UnityEngine;

public abstract class Paint {
    public bool update = true;
    public abstract Material CreateMaterialEO();
    public abstract Material CreateMaterialNZ();
    public abstract void Update(Material m);
    public abstract void setOpacity(float a);
}