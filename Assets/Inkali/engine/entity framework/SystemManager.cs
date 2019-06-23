using System;
using System.Collections.Generic;
using UnityEngine;

class SystemManager
{
    private SystemComparator systemComparator = new SystemComparator();
    private static List<EntitySystem> systems = new List<EntitySystem>(16);
    private ImmutableArray<EntitySystem> immutableSystems = new ImmutableArray<EntitySystem>(systems);
    private Dictionary<Type, EntitySystem> systemsByClass = new Dictionary<Type, EntitySystem>();
    private SystemListener listener;

    public SystemManager(SystemListener listener)
    {
        this.listener = listener;
    }

    public void addSystem(EntitySystem system)
    {
        Type systemType = system.GetType();
        EntitySystem oldSytem = getSystem<EntitySystem>(systemType);

        if (oldSytem != null)
        {
            removeSystem(oldSytem);
        }

        systems.Add(system);
        systemsByClass.Add(systemType, system);
        systems.Sort(systemComparator);
        listener.systemAdded(system);
    }

    public void removeSystem(EntitySystem system)
    {
        if (systems.Remove(system))
        {
            systemsByClass.Remove(system.GetType());
            listener.systemRemoved(system);
        }
    }

    public void removeAllSystems()
    {
        while (systems.Count > 0)
        {
            removeSystem(systems[0]);
        }
    }


    public T getSystem<T>(Type systemType) where T : EntitySystem
    {
        if(systemsByClass.ContainsKey(systemType))
            return (T)systemsByClass[systemType];
        return null;
    }

    public ImmutableArray<EntitySystem> getSystems()
    {
        return immutableSystems;
    }

    private class SystemComparator : IComparer<EntitySystem>{

        public int Compare(EntitySystem a, EntitySystem b)
    {
        return a.priority > b.priority ? 1 : (a.priority == b.priority) ? 0 : -1;
    }
}

public interface SystemListener
{
    void systemAdded(EntitySystem system);
    void systemRemoved(EntitySystem system);
}
}