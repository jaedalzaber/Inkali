
public class BBoxDim {
    public double min, mid, max, size;

    public BBoxDim(){
        this.min = 0;
        this.max = 0;
        this.mid = 0;
        this.size = 0;
    }

    public BBoxDim(double min, double max){
        this.min = min;
        this.max = max;
        this.mid = (min + max) / 2;
        this.size = max - min;
    }
}