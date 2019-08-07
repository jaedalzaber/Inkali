using System;
using System.Collections.Generic;

public class Path : Shape {
    public Path() : base(){ }

    public new void Add(Segment segment) {
        base.Add(segment);
    }

    public new void AddSeperate(Segment segment) {
        base.AddSeperate(segment);
    }

    public new void AddAll(params Segment[] segments) {
        base.AddAll(segments);
    }

    public new void AddAt(Segment segment, int index) {
        base.AddAt(segment, index);
    }

/*    public new void AddAfter(Segment segment, Segment after) {
        base.AddAfter(segment, after);
    }

    public new void AddBefore(Segment segment, Segment before) {
        base.AddBefore(segment, before);
    }
    */
    public new void Remove(Segment segment) {
        base.Remove(segment);
    }

    public new void RemoveAt(int index) {
        base.RemoveAt(index);
    }

    public void Clear() {
        segments.Clear();
    }
    public void Clean() {
        segments.Clear();
        strokeWidth = 0;
        strokeOpacity = 0;
        strokeDashArray = null;
        linecap = LineCap.FLAT;
        linejoin = LineJoin.MILTER;
        milterLimit = 10;

        fillOpacity = 0;
        fillrule = FillRule.EVEN_ODD;
        fill = null;
        stroke = null;
    }
}
