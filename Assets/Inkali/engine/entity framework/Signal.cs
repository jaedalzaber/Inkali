using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signal<T> {
    private SnapshotArray<Listener<T>> listeners;

    public Signal()
    {
        listeners = new SnapshotArray<Listener<T>>();
    }

    /**
	 * Add a Listener to this Signal
	 * @param listener The Listener to be added
	 */
    public void add(Listener<T> listener)
    {
        listeners.Add(listener);
    }

    /**
	 * Remove a listener from this Signal
	 * @param listener The Listener to remove
	 */
    public void remove(Listener<T> listener)
    {
        listeners.removeValue(listener, true);
    }

    /** Removes all listeners attached to this {@link Signal}. */
    public void removeAllListeners()
    {
        listeners.clear();
    }

    /**
	 * Dispatches an event to all Listeners registered to this Signal
	 * @param object The object to send off
	 */
    public void dispatch(T obj)
    {
        Listener<T>[] items = listeners.begin();
        for (int i = 0, n = listeners.Count; i < n; i++)
        {
            Listener<T> listener = items[i];
            listener.receive(this, obj);
        }
        listeners.end();
    }
}
