﻿using System;

public class Bag<E>
{
    private E[] data;
    public int _size = 0;

    /**
	 * Empty Bag with an initial capacity of 64.
	 */
    public Bag() : this(64)
    {
    }

    /**
	 * Empty Bag with the specified initial capacity.
	 * @param capacity the initial capacity of Bag.
	 */

    public Bag(int capacity)
    {
        data = new E[capacity];
    }

    /**
	 * Removes the element at the specified position in this Bag. Order of elements is not preserved.
	 * @param index
	 * @return element that was removed from the Bag.
	 */
    public E remove(int index)
    {
        E e = data[index]; // make copy of element to remove so it can be returned
        data[index] = data[--_size]; // overwrite item to remove with last element
        data[_size] = default(E); // null last element, so gc can do its work
        return e;
    }

    /**
	 * Removes and return the last object in the bag.
	 * @return the last object in the bag, null if empty.
	 */
    public E removeLast()
    {
        if (_size > 0)
        {
            E e = data[--_size];
            data[_size] = default(E);
            return e;
        }

        return default(E);
    }

    /**
	 * Removes the first occurrence of the specified element from this Bag, if it is present. If the Bag does not contain the
	 * element, it is unchanged. It does not preserve order of elements.
	 * @param e
	 * @return true if the element was removed.
	 */
    public bool remove(E e)
    {
        for (int i = 0; i < _size; i++)
        {
            E e2 = data[i];

            if (ReferenceEquals(e, e2))
            {
                data[i] = data[--_size]; // overwrite item to remove with last element
                data[_size] = default(E); // null last element, so gc can do its work
                return true;
            }
        }

        return false;
    }

    /**
	 * Check if bag contains this element. The operator == is used to check for equality.
	 */
    public bool contains(E e)
    {
        for (int i = 0; _size > i; i++)
        {
            if (ReferenceEquals(e, data[i]))
            {
                return true;
            }
        }
        return false;
    }

    /**
	 * @return the element at the specified position in Bag.
	 */
    public E get(int index)
    {
        return data[index];
    }

    /**
	 * @return the number of elements in this bag.
	 */
    public int size()
    {
        return _size;
    }

    /**
	 * @return the number of elements the bag can hold without growing.
	 */
    public int getCapacity()
    {
        return data.Length;
    }

    /**
	 * @param index
	 * @return whether or not the index is within the bounds of the collection
	 */
    public bool isIndexWithinBounds(int index)
    {
        return index < getCapacity();
    }

    /**
	 * @return true if this list contains no elements
	 */
    public bool isEmpty()
    {
        return _size == 0;
    }

    /**
	 * Adds the specified element to the end of this bag. if needed also increases the capacity of the bag.
	 */
    public void add(E e)
    {
        // is size greater than capacity increase capacity
        if (_size == data.Length)
        {
            grow();
        }

        data[_size++] = e;
    }

    /**
	 * Set element at specified index in the bag.
	 */
    public void set(int index, E e)
    {
        if (index >= data.Length)
        {
            grow(index * 2);
        }
        _size = index + 1;
        data[index] = e;
    }

    /**
	 * Removes all of the elements from this bag. The bag will be empty after this call returns.
	 */
    public void clear()
    {
        // null all elements so gc can clean up
        for (int i = 0; i < _size; i++)
        {
            data[i] = default(E);
        }

        _size = 0;
    }

    private void grow()
    {
        int newCapacity = (data.Length * 3) / 2 + 1;
        grow(newCapacity);
    }


    private void grow(int newCapacity)
    {
        E[] oldData = data;
        data = new E[newCapacity];
        Array.Copy(oldData, 0, data, 0, oldData.Length);
    }
}