using System;
using System.Collections.Generic;
using UnityEngine;

public class Engine
{
    //private static Family empty = Family.all().get();

    private Listener<Entity> componentAdded;
    private Listener<Entity> componentRemoved;

    private SystemManager systemManager;
    private EntityManager entityManager;
    private ComponentOperationHandler componentOperationHandler;
    private FamilyManager familyManager;
    private bool updating;

    /**
	 * Creates a new Entity object.
	 * @return @{@link Entity}
	 */

    public Engine()
    {

      systemManager = new SystemManager(new EngineSystemListener(this));
      entityManager = new EntityManager(new EngineEntityListener(this));
      componentOperationHandler = new ComponentOperationHandler(new EngineDelayedInformer(this));
      familyManager = new FamilyManager(entityManager.getEntities());
      componentAdded = new ComponentListener(familyManager);
      componentRemoved = new ComponentListener(familyManager);
}

    public Entity createEntity()
    {
        return new Entity();
    }

    public Path CreatePath(string name) {
        GameObject obj = new GameObject(name);
        obj.AddComponent<Path>();

        return obj.GetComponent<Path>();
    }

    /**
	 * Creates a new {@link Component}. To use that method your components must have a visible no-arg constructor
	 */
    public T createComponent<T>(Type componentType)
    {
            return (T)Activator.CreateInstance(componentType);
    }

    /**
	 * Adds an entity to this Engine.
	 * This will throw an IllegalArgumentException if the given entity
	 * was already registered with an engine.
	 */
    public void addEntity(Entity entity)
    {
        bool delayed = updating || familyManager.notifying();
        entityManager.addEntity(entity, delayed);
    }

    /**
	 * Removes an entity from this Engine.
	 */
    public void removeEntity(Entity entity)
    {
        bool delayed = updating || familyManager.notifying();
        entityManager.removeEntity(entity, delayed);
    }

    /**
	 * Removes all entities of the given {@link Family}.
	 */
    public void removeAllEntities(Family family)
    {
        bool delayed = updating || familyManager.notifying();
        entityManager.removeAllEntities(getEntitiesFor(family), delayed);
    }

    /**
	 * Removes all entities registered with this Engine.
	 */
    public void removeAllEntities()
    {
        bool delayed = updating || familyManager.notifying();
        entityManager.removeAllEntities(delayed);
    }

    /**
	 * Returns an {@link ImmutableArray} of {@link Entity} that is managed by the the Engine
	 *  but cannot be used to modify the state of the Engine. This Array is not Immutable in
	 *  the sense that its contents will not be modified, but in the sense that it only reflects
	 *  the state of the engine.
	 *
	 * The Array is Immutable in the sense that you cannot modify its contents through the API of
	 *  the {@link ImmutableArray} class, but is instead "Managed" by the Engine itself. The engine
	 *  may add or remove items from the array and this will be reflected in the returned array.
	 *
	 * This is an important note if you are looping through the returned entities and calling operations
	 *  that may add/remove entities from the engine, as the underlying iterator of the returned array
	 *  will reflect these modifications.
	 *
	 * The returned array will have entities removed from it if they are removed from the engine,
	 *   but there is no way to introduce new Entities through the array's interface, or remove
	 *   entities from the engine through the array interface.
	 *
	 *  Discussion of this can be found at https://github.com/libgdx/ashley/issues/224
	 *
	 * @return An unmodifiable array of entities that will match the state of the entities in the
	 *  engine.
	 */
    public ImmutableArray<Entity> getEntities()
    {
        return entityManager.getEntities();
    }

    /**
	 * Adds the {@link EntitySystem} to this Engine.
	 * If the Engine already had a system of the same class,
	 * the new one will replace the old one.
	 */
    public void addSystem(EntitySystem system)
    {
        systemManager.addSystem(system);
    }

    /**
	 * Removes the {@link EntitySystem} from this Engine.
	 */
    public void removeSystem(EntitySystem system)
    {
        systemManager.removeSystem(system);
    }

    /**
	 * Removes all systems from this Engine.
	 */
    public void removeAllSystems()
    {
        systemManager.removeAllSystems();
    }

    /**
	 * Quick {@link EntitySystem} retrieval.
	 */

    public T getSystem<T>(Type systemType) where T : EntitySystem
    {
        return  (T)systemManager.getSystem<EntitySystem>(systemType);
    }

    /**
	 * @return immutable array of all entity systems managed by the {@link Engine}.
	 */
    public ImmutableArray<EntitySystem> getSystems()
    {
        return systemManager.getSystems();
    }

    /**
	 * Returns immutable collection of entities for the specified {@link Family}. Will return the same instance every time.
	 */
    public ImmutableArray<Entity> getEntitiesFor(Family family)
    {
        return familyManager.getEntitiesFor(family);
    }

    /**
	 * Adds an {@link EntityListener}.
	 *
	 * The listener will be notified every time an entity is added/removed to/from the engine.
	 */
    public void addEntityListener(EntityListener listener)
    {
        //addEntityListener(empty, 0, listener);
    }

    /**
	 * Adds an {@link EntityListener}. The listener will be notified every time an entity is added/removed
	 * to/from the engine. The priority determines in which order the entity listeners will be called. Lower
	 * value means it will get executed first.
	 */
    public void addEntityListener(int priority, EntityListener listener)
    {
        //addEntityListener(empty, priority, listener);
    }

    /**
	 * Adds an {@link EntityListener} for a specific {@link Family}.
	 *
	 * The listener will be notified every time an entity is added/removed to/from the given family.
	 */
    public void addEntityListener(Family family, EntityListener listener)
    {
        addEntityListener(family, 0, listener);
    }

    /**
	 * Adds an {@link EntityListener} for a specific {@link Family}. The listener will be notified every time an entity is
	 * added/removed to/from the given family. The priority determines in which order the entity listeners will be called. Lower
	 * value means it will get executed first.
	 */
    public void addEntityListener(Family family, int priority, EntityListener listener)
    {
        familyManager.addEntityListener(family, priority, listener);
    }

    /**
	 * Removes an {@link EntityListener}
	 */
    public void removeEntityListener(EntityListener listener)
    {
        familyManager.removeEntityListener(listener);
    }

    /**
	 * Updates all the systems in this Engine.
	 * @param deltaTime The time passed since the last frame.
	 */
    public void update(float deltaTime)
    {
        if (updating)
        {
            throw new ArgumentException("Cannot call update() on an Engine that is already updating.");
        }

        updating = true;
        ImmutableArray<EntitySystem> systems = systemManager.getSystems();
        try
        {
            for (int i = 0; i < systems.size(); ++i)
            {
                EntitySystem system = systems.get(i);

                if (system.checkProcessing())
                {
                    system.update(deltaTime);
                }

                while (componentOperationHandler.hasOperationsToProcess() || entityManager.hasPendingOperations())
                {
                    componentOperationHandler.processOperations();
                    entityManager.processPendingOperations();
                }
            }
        }
        finally
        {
            updating = false;
        }
    }

    protected void addEntityInternal(Entity entity)
    {
        entity.componentAdded.add(componentAdded);
        entity.componentRemoved.add(componentRemoved);
        entity.componentOperationHandler = componentOperationHandler;

        familyManager.updateFamilyMembership(entity);
    }

    protected void removeEntityInternal(Entity entity)
    {
        familyManager.updateFamilyMembership(entity);

        entity.componentAdded.remove(componentAdded);
        entity.componentRemoved.remove(componentRemoved);
        entity.componentOperationHandler = null;
    }

    private class ComponentListener : Listener<Entity> {
        private FamilyManager familyManager;

        public ComponentListener(FamilyManager familyManager)
        {
            this.familyManager = familyManager;
        }
        public void receive(Signal<Entity> signal, Entity obj)
    {
        familyManager.updateFamilyMembership(obj);
    }
}

    private class EngineSystemListener : SystemManager.SystemListener
    {
        private Engine engine;
        public EngineSystemListener(Engine engine)
        {
            this.engine = engine;
        }
                public void systemAdded(EntitySystem system)
                {
                    system.addedToEngineInternal(engine);
                }

                public void systemRemoved(EntitySystem system)
                {
                    system.removedFromEngineInternal(engine);
                }
	}
	
	private class EngineEntityListener : EntityListener
    {
        private Engine engine;
        public EngineEntityListener(Engine engine)
        {
            this.engine = engine;
        }
        public void entityAdded(Entity entity)
        {
            engine.addEntityInternal(entity);
        }

        public void entityRemoved(Entity entity)
        {
            engine.removeEntityInternal(entity);
        }
	}
	
	private class EngineDelayedInformer : ComponentOperationHandler.BooleanInformer
    {
        private Engine engine;
        public EngineDelayedInformer(Engine engine)
        {
            this.engine = engine;
        }
        public bool value()
        {
              return engine.updating;
        }
	}
}