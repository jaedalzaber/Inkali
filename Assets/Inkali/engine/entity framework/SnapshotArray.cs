using System;
using System.Collections.Generic;
using UnityEngine;

public class SnapshotArray<T> : InkList<T> {
	private T[] snapshot, recycled;
	private int snapshots;

	public SnapshotArray () : base(){
	}

	public SnapshotArray (InkList<T> array) : base(array){
	}

	public SnapshotArray (int capacity) : base(capacity) {
		
	}
		
	public SnapshotArray (T[] array) : base(array) {
	}

	/** Returns the backing array, which is guaranteed to not be modified before {@link #end()}. */
	public T[] begin () {
		modified();
		snapshot = _items;
		snapshots++;
		return _items;
	}

	/** Releases the guarantee that the array returned by {@link #begin()} won't be modified. */
	public void end () {
		snapshots = Mathf.Max(0, snapshots - 1);
		if (snapshot == null) return;
		if (snapshot != _items && snapshots == 0) {
			// The backing array was copied, keep around the old array.
			recycled = snapshot;
			for (int i = 0, n = recycled.Length; i < n; i++)
                ReferenceEquals(recycled[i], null);
        }
		snapshot = null;
	}

	private void modified () {
		if (snapshot == null || snapshot != _items) return;
		// Snapshot is in use, copy backing array to recycled array or create new backing array.
		if (recycled != null && recycled.Length >= Count) {
			Array.Copy(_items, 0, recycled, 0, Count);
			_items = recycled;
			recycled = null;
		} else
            EnsureCapacity(_items.Length);
	}

	public void set (int index, T value) {
		modified();
		if (index >= Count)
			throw new IndexOutOfRangeException ();
		_items[index] = value;
	}

	public void insert (int index, T value) {
		modified();
		Insert(index, value);
	}

	public void swap (int first, int second) {
		modified();
		if (first >= Count)
			throw new IndexOutOfRangeException ();
		if (second >= Count)
			throw new IndexOutOfRangeException ();
		T firstValue = _items [first];
		_items [first] = _items [second];
		_items [second] = firstValue;
	}

	public bool removeValue (T value, bool identity) {
		modified();
		return Remove(value);
	}

	public void removeIndex (int index) {
		modified();
		RemoveAt(index);
	}

	public void removeRange (int start, int end) {
		modified();
		RemoveRange(start, end);
	}

	public void clear () {
		modified();
		Clear();
	}


	/** @see #SnapshotArray(Object[]) */
	static public SnapshotArray<T> with (params T[] array) {
		return new SnapshotArray<T>(array);
	}
}