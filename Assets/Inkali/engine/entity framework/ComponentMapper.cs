using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ComponentMapper<T> where T : Component {
	private ComponentType componentType;

	/**
	 * @param componentClass Component class to be retrieved by the mapper.
	 * @return New instance that provides fast access to the {@link Component} of the specified class.
	 */
	public static ComponentMapper<T> getFor (Type componentClass) {
		return new ComponentMapper<T>(componentClass);
	}

	/** @return The {@link Component} of the specified class belonging to entity. */
	public T get (Entity entity) {
		return entity.getComponent<T>(componentType);
	}

	/** @return Whether or not entity has the component of the specified class. */
	public bool has (Entity entity) {
		return entity.hasComponent(componentType);
	}

	private ComponentMapper (Type componentClass) {
		componentType = ComponentType.getFor(componentClass);
	}
}
