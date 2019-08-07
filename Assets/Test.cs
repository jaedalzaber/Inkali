using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
    Engine engine;
    private ImmutableArray<Entity> entities;
    private GameObject o;
    private Path p1;
    private Path p2;
    private PaintSolid paint;

    public GameObject[] points;
    private PCubic c1;

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
        SystemStroke systemStroke = new SystemStroke();
        engine.addSystem(systemFill);
        engine.addSystem(systemStroke);
        engine.addEntityListener(Family.all(typeof(CompFill)).get(), systemFill);
        engine.addEntityListener(Family.all(typeof(CompStroke)).get(), systemStroke);


        p1 = engine.CreatePath("p1");
        // p1.Add(new PCubic(new Vector2d(-2,-2), new Vector2d(0,4), new Vector2d(2,4), new Vector2d(4,-2)));
        p2 = engine.CreatePath("p2");
        // p2.Add(new PCubic(new Vector2d(2, -2), new Vector2d(-4, 3), new Vector2d(6, 3), new Vector2d(0, -2)));
        // p1.Add(new PCubic(new Vector2d(-1, 2), new Vector2d(3, 3), new Vector2d(4, 1)));
        // // p1.Add(new PLine(new Vector2d(2.5f, -1f)));
        // p1.Add(new PQuadratic(new Vector2d(1, -3), new Vector2d(-1f, -1)));
        // p1.Add(new PCubic(new Vector2d(0, -3), new Vector2d(4, -4), new Vector2d(4, 0)));
        // p1.Add(new PCubic(new Vector2d(0, 0), new Vector2d(0, -3), new Vector2d(-1, 1.5f)));

        paint = new PaintSolid(Color.cyan);
        PaintSolid paint2 = new PaintSolid(Color.blue);
        p1.FillPaint = paint;
        p1.StrokePaint = paint2;
        p1.StrokeWidth = .1f;
        c1 = new PCubic(new Vector2d(2,0), new Vector2d(-2,3), new Vector2d(4,3), new Vector2d(0,0));
        // c1 = new PCubic(new Vector2d(0,0), new Vector2d(1,0), new Vector2d(1,1), new Vector2d(2,0));
        p1.Add(c1);

        p2.FillPaint = new PaintSolid(Color.yellow);
        p2.StrokePaint = new PaintSolid(Color.red);
        p2.StrokeWidth = .1f;

        c1.Ctrl1=new Vector2d(points[1].transform.position.x, points[1].transform.position.y);
        c1.Ctrl2=new Vector2d(points[2].transform.position.x, points[2].transform.position.y);
        c1.EndPoint=new Vector2d(points[3].transform.position.x, points[3].transform.position.y);

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
        if(((Touch)points[0].GetComponent(typeof(Touch))).Touched()){
            c1.StartPoint=new Vector2d(points[0].transform.position.x, points[0].transform.position.y);
            p1.UpdateStroke = true;
            p1.UpdateFill = true;
        }

        if(((Touch)points[1].GetComponent(typeof(Touch))).Touched()){
            c1.Ctrl1=new Vector2d(points[1].transform.position.x, points[1].transform.position.y);
            p1.UpdateStroke = true;
            p1.UpdateFill = true;
        }

        if(((Touch)points[2].GetComponent(typeof(Touch))).Touched()){
            c1.Ctrl2=new Vector2d(points[2].transform.position.x, points[2].transform.position.y);
            p1.UpdateStroke = true;
            p1.UpdateFill = true;
        }

        if(((Touch)points[3].GetComponent(typeof(Touch))).Touched()){
            c1.EndPoint=new Vector2d(points[3].transform.position.x, points[3].transform.position.y);
            p1.UpdateStroke = true;
            p1.UpdateFill = true;
        }
    }
}
