using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImmutableArray<T> : IEnumerable<T> {

    private List<T> array;

    public ImmutableArray(List<T> array)
    {
        this.array = array;
    }

    public int size()
    {
        return array.Count;
    }

    public T get(int index)
    {
        return array[index];
    }

    public bool contains(T value)
    {
        return array.Contains(value);
    }

    public int indexOf(T value)
    {
        return array.IndexOf(value);
    }

    public int lastIndexOf(T value)
    {
        return array.LastIndexOf(value);
    }
    

    public T[] toArray()
    {
        return array.ToArray();
    }

    public List<T> toList() {
        return array;
    }

    public override int GetHashCode()
    {
        return array.GetHashCode();
    }

    public override bool Equals(System.Object obj)
    {
        return array.Equals(obj);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return array.GetEnumerator();
    }

    public IEnumerator<T> GetEnumerator()
    {
        return array.GetEnumerator();
    }
}
