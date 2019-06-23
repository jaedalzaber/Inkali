using System;
using System.Collections.Generic;

public class EntityManager
{
    private EntityListener listener;
    private List<Entity> entities = new List<Entity>(16);
    private List<Entity> entitySet = new List<Entity>(); // Implement ObjectSet libgdx
    private ImmutableArray<Entity> immutableEntities;
    private List<EntityOperation> pendingOperations = new List<EntityOperation>(16);
    private EntityOperationPool entityOperationPool = new EntityOperationPool();

    public EntityManager(EntityListener listener)
    {
        immutableEntities = new ImmutableArray<Entity>(entities);
        this.listener = listener;
    }

    public void addEntity(Entity entity)
    {
        addEntity(entity, false);
    }

    public void addEntity(Entity entity, bool delayed)
    {
        if (delayed)
        {
            EntityOperation operation = entityOperationPool.obtain();
            operation.entity = entity;
            operation.type = EntityOperation.Type.Add;
            pendingOperations.Add(operation);
        }
        else
        {
            addEntityInternal(entity);
        }
    }

    public void removeEntity(Entity entity)
    {
        removeEntity(entity, false);
    }

    public void removeEntity(Entity entity, bool delayed)
    {
        if (delayed)
        {
            if (entity.scheduledForRemoval)
            {
                return;
            }
            entity.scheduledForRemoval = true;
            EntityOperation operation = entityOperationPool.obtain();
            operation.entity = entity;
            operation.type = EntityOperation.Type.Remove;
            pendingOperations.Add(operation);
        }
        else
        {
            removeEntityInternal(entity);
        }
    }

    public void removeAllEntities()
    {
        removeAllEntities(immutableEntities);
    }

    public void removeAllEntities(bool delayed)
    {
        removeAllEntities(immutableEntities, delayed);
    }

    public void removeAllEntities(ImmutableArray<Entity> entities)
    {
        removeAllEntities(entities, false);
    }

    public void removeAllEntities(ImmutableArray<Entity> entities, bool delayed)
    {
        if (delayed)
        {
            foreach (Entity entity in entities)
            {
                entity.scheduledForRemoval = true;
            }
            EntityOperation operation = entityOperationPool.obtain();
            operation.type = EntityOperation.Type.RemoveAll;
            operation.entities = entities;
            pendingOperations.Add(operation);
        }
        else
        {
            while (entities.size() > 0)
            {
                removeEntity(entities.get(0), false);
            }
        }
    }

    public ImmutableArray<Entity> getEntities()
    {
        return immutableEntities;
    }

    public bool hasPendingOperations()
    {
        return pendingOperations.Count > 0;
    }

    public void processPendingOperations()
    {
        for (int i = 0; i < pendingOperations.Count; ++i)
        {
            EntityOperation operation = pendingOperations[i];

            switch (operation.type)
            {
                case EntityOperation.Type.Add: addEntityInternal(operation.entity); break;
                case EntityOperation.Type.Remove: removeEntityInternal(operation.entity); break;
                case EntityOperation.Type.RemoveAll:
                    while (operation.entities.size() > 0)
                    {
                        removeEntityInternal(operation.entities.get(0));
                    }
                    break;
                default:
                    throw new ArgumentException("Unexpected EntityOperation type");
            }

            entityOperationPool.free(operation);
        }

        pendingOperations.Clear();
    }

    protected void removeEntityInternal(Entity entity)
    {
        bool removed = entitySet.Remove(entity);

        if (removed)
        {
            entity.scheduledForRemoval = false;
            entity.removing = true;
            entities.Remove(entity);
            listener.entityRemoved(entity);
            entity.removing = false;
        }
    }

    protected void addEntityInternal(Entity entity)
    {
        if (entitySet.Contains(entity))
        {
            throw new ArgumentException("Entity is already registered " + entity);
        }

        entities.Add(entity);
        entitySet.Add(entity);

        listener.entityAdded(entity);
    }

    private class EntityOperation : Poolable
    {

        public enum Type
    {
        Add,
        Remove,
        RemoveAll
    }

    public Type type;
    public Entity entity;
    public ImmutableArray<Entity> entities;

        public void reset()
    {
        entity = null;
    }
}

private class EntityOperationPool : Pool<EntityOperation> {

        protected override EntityOperation newObject()
        {
            return new EntityOperation();
        }
    }
}
