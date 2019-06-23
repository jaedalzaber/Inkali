using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DoubleArray {
    public double[] items;
    public int size;
    public bool ordered;

    /** Creates an ordered array with a capacity of 16. */
    public DoubleArray() : this(true, 16) { 
    }

    /** Creates an ordered array with the specified capacity. */
    public DoubleArray(int capacity) : this(true, capacity) { 
    }

    /** @param ordered If false, methods that remove elements may change the order of other elements in the array, which avoids a
	 *           memory copy.
	 * @param capacity Any elements added beyond this will cause the backing array to be grown. */
    public DoubleArray(bool ordered, int capacity) {
        this.ordered = ordered;
        items = new double[capacity];
    }

    /** Creates a new array containing the elements in the specific array. The new array will be ordered if the specific array is
	 * ordered. The capacity is set to the number of elements, so any subsequent elements added will cause the backing array to be
	 * grown. */
    public DoubleArray(DoubleArray array) {
        this.ordered = array.ordered;
        size = array.size;
        items = new double[size];
        Array.Copy(array.items, 0, items, 0, size);
    }

    /** Creates a new ordered array containing the elements in the specified array. The capacity is set to the number of elements,
	 * so any subsequent elements added will cause the backing array to be grown. */
    public DoubleArray(double[] array) : this(true, array, 0, array.Length) { }

    /** Creates a new array containing the elements in the specified array. The capacity is set to the number of elements, so any
	 * subsequent elements added will cause the backing array to be grown.
	 * @param ordered If false, methods that remove elements may change the order of other elements in the array, which avoids a
	 *           memory copy. */
    public DoubleArray(bool ordered, double[] array, int startIndex, int count) : this(ordered, count) {
        size = count;
        Array.Copy(array, startIndex, items, 0, count);
    }

    public void add(double value) {
        double[] items = this.items;
        if (size == items.Length) items = resize(Mathd.Max(8, (int)(size * 1.75f)));
        items[size++] = value;
    }

    public void add(double value1, double value2) {
        double[] items = this.items;
        if (size + 1 >= items.Length) items = resize(Mathd.Max(8, (int)(size * 1.75f)));
        items[size] = value1;
        items[size + 1] = value2;
        size += 2;
    }

    public void add(double value1, double value2, double value3) {
        double[] items = this.items;
        if (size + 2 >= items.Length) items = resize(Mathd.Max(8, (int)(size * 1.75f)));
        items[size] = value1;
        items[size + 1] = value2;
        items[size + 2] = value3;
        size += 3;
    }

    public void add(double value1, double value2, double value3, double value4) {
        double[] items = this.items;
        if (size + 3 >= items.Length) items = resize(Mathd.Max(8, (int)(size * 1.8f))); // 1.75 isn't enough when size=5.
        items[size] = value1;
        items[size + 1] = value2;
        items[size + 2] = value3;
        items[size + 3] = value4;
        size += 4;
    }

    public void addAll(DoubleArray array) {
        addAll(array, 0, array.size);
    }

    public void addAll(DoubleArray array, int offset, int length) {
        if (offset + length > array.size)
            throw new ArgumentException("offset + length must be <= size: " + offset + " + " + length + " <= " + array.size);
        addAll(array.items, offset, length);
    }

    public void addAll(params double[] array) {
        addAll(array, 0, array.Length);
    }

    public void addAll(double[] array, int offset, int length) {
        double[] items = this.items;
        int sizeNeeded = size + length;
        if (sizeNeeded > items.Length) items = resize(Mathd.Max(8, (int)(sizeNeeded * 1.75f)));
        Array.Copy(array, offset, items, size, length);
        size += length;
    }

    public double get(int index) {
        if (index >= size) throw new IndexOutOfRangeException("index can't be >= size: " + index + " >= " + size);
        return items[index];
    }

    public void set(int index, double value) {
        if (index >= size) throw new IndexOutOfRangeException("index can't be >= size: " + index + " >= " + size);
        items[index] = value;
    }

    public void incr(int index, double value) {
        if (index >= size) throw new IndexOutOfRangeException("index can't be >= size: " + index + " >= " + size);
        items[index] += value;
    }

    public void mul(int index, double value) {
        if (index >= size) throw new IndexOutOfRangeException("index can't be >= size: " + index + " >= " + size);
        items[index] *= value;
    }

    public void insert(int index, double value) {
        if (index > size) throw new IndexOutOfRangeException("index can't be > size: " + index + " > " + size);
        double[] items = this.items;
        if (size == items.Length) items = resize(Mathd.Max(8, (int)(size * 1.75f)));
        if (ordered)
            Array.Copy(items, index, items, index + 1, size - index);
        else
            items[size] = items[index];
        size++;
        items[index] = value;
    }

    public void swap(int first, int second) {
        if (first >= size) throw new IndexOutOfRangeException("first can't be >= size: " + first + " >= " + size);
        if (second >= size) throw new IndexOutOfRangeException("second can't be >= size: " + second + " >= " + size);
        double[] items = this.items;
        double firstValue = items[first];
        items[first] = items[second];
        items[second] = firstValue;
    }

    public bool contains(double value) {
        int i = size - 1;
        double[] items = this.items;
        while (i >= 0)
            if (items[i--] == value) return true;
        return false;
    }

    public int indexOf(double value) {
        double[] items = this.items;
        for (int i = 0, n = size; i < n; i++)
            if (items[i] == value) return i;
        return -1;
    }

    public int lastIndexOf(char value) {
        double[] items = this.items;
        for (int i = size - 1; i >= 0; i--)
            if (items[i] == value) return i;
        return -1;
    }

    public bool removeValue(double value) {
        double[] items = this.items;
        for (int i = 0, n = size; i < n; i++) {
            if (items[i] == value) {
                removeIndex(i);
                return true;
            }
        }
        return false;
    }

    /** Removes and returns the item at the specified index. */
    public double removeIndex(int index) {
        if (index >= size) throw new IndexOutOfRangeException("index can't be >= size: " + index + " >= " + size);
        double[] items = this.items;
        double value = items[index];
        size--;
        if (ordered)
            Array.Copy(items, index + 1, items, index, size - index);
        else
            items[index] = items[size];
        return value;
    }

    /** Removes the items between the specified indices, inclusive. */
    public void removeRange(int start, int end) {
        if (end >= size) throw new IndexOutOfRangeException("end can't be >= size: " + end + " >= " + size);
        if (start > end) throw new IndexOutOfRangeException("start can't be > end: " + start + " > " + end);
        double[] items = this.items;
        int count = end - start + 1;
        if (ordered)
            Array.Copy(items, start + count, items, start, size - (start + count));
        else {
            int lastIndex = this.size - 1;
            for (int i = 0; i < count; i++)
                items[start + i] = items[lastIndex - i];
        }
        size -= count;
    }

    /** Removes from this array all of elements contained in the specified array.
	 * @return true if this array was modified. */
    public bool removeAll(DoubleArray array) {
        int size = this.size;
        int startSize = size;
        double[] items = this.items;
        for (int i = 0, n = array.size; i < n; i++) {
            double item = array.get(i);
            for (int ii = 0; ii < size; ii++) {
                if (item == items[ii]) {
                    removeIndex(ii);
                    size--;
                    break;
                }
            }
        }
        return size != startSize;
    }

    /** Removes and returns the last item. */
    public double pop() {
        return items[--size];
    }

    /** Returns the last item. */
    public double peek() {
        return items[size - 1];
    }

    /** Returns the first item. */
    public double first() {
        if (size == 0) throw new ArgumentException("Array is empty.");
        return items[0];
    }

    public void clear() {
        size = 0;
    }

    /** Reduces the size of the backing array to the size of the actual items. This is useful to release memory when many items
	 * have been removed, or if it is known that more items will not be added.
	 * @return {@link #items} */
    public double[] shrink() {
        if (items.Length != size) resize(size);
        return items;
    }

    /** Increases the size of the backing array to accommodate the specified number of additional items. Useful before adding many
	 * items to avoid multiple backing array resizes.
	 * @return {@link #items} */
    public double[] ensureCapacity(int additionalCapacity) {
        int sizeNeeded = size + additionalCapacity;
        if (sizeNeeded > items.Length) resize(Mathd.Max(8, sizeNeeded));
        return items;
    }

    /** Sets the array size, leaving any values beyond the current size undefined.
	 * @return {@link #items} */
    public double[] setSize(int newSize) {
        if (newSize > items.Length) resize(Mathd.Max(8, newSize));
        size = newSize;
        return items;
    }

    protected double[] resize(int newSize) {
        double[] newItems = new double[newSize];
        double[] items = this.items;
        Array.Copy(items, 0, newItems, 0, Mathd.Min(size, newItems.Length));
        this.items = newItems;
        return newItems;
    }

    public void sort() {
        Array.Sort(items, 0, size);
    }

    public void reverse() {
        double[] items = this.items;
        for (int i = 0, lastIndex = size - 1, n = size / 2; i < n; i++) {
            int ii = lastIndex - i;
            double temp = items[i];
            items[i] = items[ii];
            items[ii] = temp;
        }
    }

    /** Reduces the size of the array to the specified size. If the array is already smaller than the specified size, no action is
	 * taken. */
    public void truncate(int newSize) {
        if (size > newSize) size = newSize;
    }


    public double[] toArray() {
        double[] array = new double[size];
        Array.Copy(items, 0, array, 0, size);
        return array;
    }


    public bool equals(object obj) {
        if (obj == this) return true;
        if (!ordered) return false;
        if (!(obj is DoubleArray)) return false;
        DoubleArray array = (DoubleArray)obj;
        if (!array.ordered) return false;
        int n = size;
        if (n != array.size) return false;
        double[] items1 = this.items;
        double[] items2 = array.items;
        for (int i = 0; i < n; i++)
            if (items1[i] != items2[i]) return false;
        return true;
    }

    public bool equals(object obj, double epsilon) {
        if (obj == this) return true;
        if (!(obj is DoubleArray)) return false;
        DoubleArray array = (DoubleArray)obj;
        int n = size;
        if (n != array.size) return false;
        if (!ordered) return false;
        if (!array.ordered) return false;
        double[] items1 = this.items;
        double[] items2 = array.items;
        for (int i = 0; i < n; i++)
            if (Mathd.Abs(items1[i] - items2[i]) > epsilon) return false;
        return true;
    }


    /** @see #DoubleArray(double[]) */
    static public DoubleArray with(params double[] array) {
        return new DoubleArray(array);
    }
}