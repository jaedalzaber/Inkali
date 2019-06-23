using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Listener<T>
{
    /**
	 * @param signal The Signal that triggered event
	 * @param object The object passed on dispatch
	 */
    void receive(Signal<T> signal, T obj);
}
