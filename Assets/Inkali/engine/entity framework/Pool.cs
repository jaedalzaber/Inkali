using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Pool<T>
{
    /** The maximum number of objects that will be pooled. */
    public int max;
    /** The highest number of free objects. Can be reset any time. */
    public int peak;

    private List<T> freeObjects;

    /** Creates a pool with an initial capacity of 16 and no maximum. */
    public Pool(): this(16, int.MaxValue)
    {
    }

    /** Creates a pool with the specified initial capacity and no maximum. */
    public Pool(int initialCapacity) : this(initialCapacity, int.MaxValue)
    {
    }

    /** @param max The maximum number of free objects to store in this pool. */
    public Pool(int initialCapacity, int max)
    {
        freeObjects = new List<T>(initialCapacity);
        this.max = max;
    }

    abstract protected T newObject();

    /** Returns an object from this pool. The object may be new (from {@link #newObject()}) or reused (previously
	 * {@link #free(Object) freed}). */
    public T obtain()
    {
        T last = freeObjects.Count == 0 ? newObject() : freeObjects[freeObjects.Count - 1];
        if (freeObjects.Count != 0)
            freeObjects.RemoveAt(freeObjects.Count - 1);
        return last;
    }

    /** Puts the specified object in the pool, making it eligible to be returned by {@link #obtain()}. If the pool already contains
	 * {@link #max} free objects, the specified object is reset but not added to the pool.
	 * <p>
	 * The pool does not check if an object is already freed, so the same object must not be freed multiple times. */
    public void free(T obj)
    {
        if (obj == null) throw new ArgumentNullException("object cannot be null.");
        if (freeObjects.Count < max)
        {
            freeObjects.Add(obj);
            peak = Mathf.Max(peak, freeObjects.Count);
        }
        reset(obj);
    }

    /** Called when an object is freed to clear the state of the object for possible later reuse. The default implementation calls
	 * {@link Poolable#reset()} if the object is {@link Poolable}. */
    protected void reset(T obj)
    {
        if (obj is Poolable) ((Poolable)obj).reset();
    }

    /** Puts the specified objects in the pool. Null objects within the array are silently ignored.
	 * <p>
	 * The pool does not check if an object is already freed, so the same object must not be freed multiple times.
	 * @see #free(Object) */
    public void freeAll(List<T> objects)
    {
        if (objects == null) throw new ArgumentNullException("objects cannot be null.");
        List<T> freeObjects = this.freeObjects;
        int max = this.max;
        for (int i = 0; i < objects.Count; i++)
        {
            T obj = objects[i];
            if (obj == null) continue;
            if (freeObjects.Count < max) freeObjects.Add(obj);
            reset(obj);
        }
        peak = Mathf.Max(peak, freeObjects.Count);
    }

    /** Removes all free objects from this pool. */
    public void clear()
    {
        freeObjects.Clear();
    }

    /** The number of objects available to be obtained. */
    public int getFree()
    {
        return freeObjects.Count;
    }

    /** Objects implementing this interface will have {@link #reset()} called when passed to {@link Pool#free(Object)}. */
   
}