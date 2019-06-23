using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Test : MonoBehaviour {
    Engine engine;
    private ImmutableArray<Entity> entities;
    private GameObject o;
    private Path p1;
    private PaintSolid paint;

    public class PositionComponent : Component
    {
        public float x = 0.0f;
        public float y = 0.0f;
    }

    public class VelocityComponent : Component
    {
        public float x = 1.0f;
        public float y = 1.0f;
    }

    public class MovementSystem : EntitySystem, EntityListener {
        private List<Entity> entities;
        bool once = true;
        private ComponentMapper<PositionComponent> pm = ComponentMapper<PositionComponent>.getFor(typeof(PositionComponent));
	    private ComponentMapper<VelocityComponent> vm = ComponentMapper<VelocityComponent>.getFor(typeof(VelocityComponent));

	    public MovementSystem() { }

        public override void addedToEngine(Engine engine)
        {
            entities = engine.getEntitiesFor(Family.all(typeof(PositionComponent), typeof(VelocityComponent)).get()).toList();
	    }

        public void entityAdded(Entity entity) {
            entities.Add(entity);
        }

        public void entityRemoved(Entity entity) {
            entities.Remove(entity);
        }

        public override void removedFromEngine(Engine engine) {
        }

        public override void update(float deltaTime) {
            for (int i = 0; i < entities.Count; ++i) {
                Entity entity = entities[i];
                PositionComponent position = pm.get(entity);
                VelocityComponent velocity = vm.get(entity);

                position.x += velocity.x * deltaTime;
                position.y += velocity.y * deltaTime;
                if (once) {
                    //Debug.Log("Entity Pos.x: " + position.x);
                }
            }
                    once = false;
        }
    }

	// Use this for initialization
	void Start () {
        engine = new Engine();
        SystemFill systemFill = new SystemFill();
        engine.addSystem(systemFill);
        engine.addEntityListener(Family.all(typeof(CompFill)).get(), systemFill);

        p1 = engine.CreatePath("p1");
        p1.Add(new PCubic(new Vector2d(0,1), new Vector2d(0,2), new Vector2d(2,2), new Vector2d(2,0)));
        p1.Add(new PCubic(new Vector2d(5, -2), new Vector2d(2, -3), new Vector2d(0, -2)));
        p1.Add(new PCubic(new Vector2d(-1, 2), new Vector2d(3, 3), new Vector2d(4, 1)));
        // p1.Add(new PLine(new Vector2d(2, -1)));
        p1.Add(new PCubic(new Vector2d(4, -4), new Vector2d(0, -4), new Vector2d(-.5f, 1)));
        p1.Add(new PCubic(new Vector2d(0, -3), new Vector2d(4, -3), new Vector2d(4, 1)));
        p1.Add(new PCubic(new Vector2d(0, 0), new Vector2d(0, -3), new Vector2d(4, -4)));

        paint = new PaintSolid(Color.cyan);
        p1.FillPaint = paint;

        // Path p2 = engine.CreatePath("p2");
        // p2.Add(new PArc(new Vector2d(0, 0), new Vector2d(1, 1), 2, 2, 0, true, PArc.SweepDirection.ANTI_CLOCKWISE));
        // p2.Add(new PArc(new Vector2d(0, 0), new Vector2d(-.5, 2), 2, 2, 0, true, PArc.SweepDirection.CLOCKWISE));
        // p2.FillPaint = new PaintSolid(Color.green);

        engine.addEntity(p1);
        // engine.addEntity(p2);

    }
    // Update is called once per frame
    void Update () {
        engine.update(Time.deltaTime);
        if (Input.anyKeyDown) {
            Color c = Color.yellow;
            // paint.Color = c;
        }
    }
}
