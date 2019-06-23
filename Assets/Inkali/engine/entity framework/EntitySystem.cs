using System;
using System.Collections.Generic;

public abstract class EntitySystem
{
    /** Use this to set the priority of the system. Lower means it'll get executed first. */
    public int priority;

    private bool processing;
    private Engine engine;

    /** Default constructor that will initialise an EntitySystem with priority 0. */
    public EntitySystem() : this(0)
    {
    }

    /**
	 * Initialises the EntitySystem with the priority specified.
	 * @param priority The priority to execute this system with (lower means higher priority).
	 */
    public EntitySystem(int priority)
    {
        this.priority = priority;
        this.processing = true;
    }

    /**
	 * Called when this EntitySystem is added to an {@link Engine}.
	 * @param engine The {@link Engine} this system was added to.
	 */
    public abstract void addedToEngine(Engine engine);

    /**
	 * Called when this EntitySystem is removed from an {@link Engine}.
	 * @param engine The {@link Engine} the system was removed from.
	 */
    public abstract void removedFromEngine(Engine engine);

    /**
	 * The update method called every tick.
	 * @param deltaTime The time passed since last frame in seconds.
	 */
    public abstract void update(float deltaTime);

    /** @return Whether or not the system should be processed. */
    public bool checkProcessing()
    {
        return processing;
    }

    /** Sets whether or not the system should be processed by the {@link Engine}. */
    public void setProcessing(bool processing)
    {
        this.processing = processing;
    }

    /** @return engine instance the system is registered to.
	 * It will be null if the system is not associated to any engine instance. */
    public Engine getEngine()
    {
        return engine;
    }

     public void addedToEngineInternal(Engine engine)
    {
        this.engine = engine;
        addedToEngine(engine);
    }

    public void removedFromEngineInternal(Engine engine)
    {
        this.engine = null;
        removedFromEngine(engine);
    }
}