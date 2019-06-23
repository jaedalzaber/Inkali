using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ComponentType {
	private static Dictionary<Type , ComponentType> assignedComponentTypes = new Dictionary<Type, ComponentType>();
	private static int typeIndex = 0;

	private int index;

	private ComponentType () {
		index = typeIndex++;
	}

	/** @return This ComponentType's unique index */
	public int getIndex () {
		return index;
	}

	/**
	 * @param componentType The {@link Component} class
	 * @return A ComponentType matching the Component Class
	 */
	public static ComponentType getFor (Type componentType) {
        ComponentType type = null;
        if (assignedComponentTypes.ContainsKey(componentType)) { 
            type = assignedComponentTypes[componentType];
        }

		if (type == null) {
			type = new ComponentType();
			assignedComponentTypes.Add(componentType, type);
		}

		return type;
	}

	/**
	 * Quick helper method. The same could be done via {@link ComponentType.getFor(Class<? extends Component>)}.
	 * @param componentType The {@link Component} class
	 * @return The index for the specified {@link Component} Class
	 */
	public static int getIndexFor (Type componentType) {
		return getFor(componentType).getIndex();
	}

	/**
	 * @param componentTypes list of {@link Component} classes
	 * @return Bits representing the collection of components for quick comparison and matching. See
	 *         {@link Family#getFor(Bits, Bits, Bits)}.
	 */
	public static Bits getBitsFor (params Type[] componentTypes) {
		Bits bits = new Bits();

		int typesLength = componentTypes.Length;
		for (int i = 0; i < typesLength; i++) {
			bits.set(getIndexFor(componentTypes[i]));
		}

		return bits;
	}


	public override int GetHashCode () {
		return index;
	}


	public  override bool Equals (System.Object obj) {
		if (this == obj) return true;
		if (obj == null) return false;
		if (GetType() != obj.GetType()) return false;
		ComponentType other = (ComponentType)obj;
		return index == other.index;
	}
}
