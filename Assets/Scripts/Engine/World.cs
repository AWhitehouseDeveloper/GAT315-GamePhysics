using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public BoolData simulate;
    public BoolData collision;
    public BoolData wrap;
    public BoolData CollisionDebug;
    public FloatData gravity;
    public FloatData gravitation;
    public FloatData fixedFPS;
    public StringData fpsText;
    public StringData collisionText;
    public BroadPhaseType broadPhaseType;
    public VectorField vectorField;

    static World instance;
    static public World Instance { get { return instance; } }

    public Vector2 Gravity { get { return new Vector2(0, gravity.value); } }
    public List<Body> bodies { get; set; } = new List<Body>();
    public List<Spring> springs { get; set; } = new List<Spring>();
    public List<Force> forces { get; set; } = new List<Force>();

    public Vector2 WorldSize { get => size * 2; }
    public AABB AABB { get => aabb; }

    BroadPhase broadPhase;
    BroadPhase[] broadPhases = { new NullBroadPhase(), new Quadtree(), new BVH() };

    AABB aabb;
    Vector2 size;

    float timeAccumulator = 0;
    float fixedDeltaTime { get{ return 1.0f / fixedFPS.value; }}
    float fps = 0;
    float fpsAverage = 0;
    float smoothing = 0.975f;

    private void Awake()
    {
        instance = this;
        size = Camera.main.ViewportToWorldPoint(Vector2.one);
        aabb = new AABB(Vector2.zero, size * 2);
    }

    void Update()
    {
        float dt = Time.deltaTime;
        fps = (1.0f / dt);
        fpsAverage = (fpsAverage * smoothing) + (fps * (1.0f - smoothing));
        fpsText.value = "FPS: " + fpsAverage.ToString("F1");

        broadPhase = broadPhases[broadPhaseType.index];

        springs.ForEach(spring => spring.Draw());
        if (!simulate.value) return;

        GravitationalForce.ApplyForce(bodies, gravitation.value);
        forces.ForEach(force => bodies.ForEach(body => force.ApplyForce(body)));
        springs.ForEach(spring => spring.ApplyForce());
        bodies.ForEach(body => vectorField.ApplyForce(body));

        //bodies.ForEach(body => body.shape.color = Color.green);

        timeAccumulator += Time.deltaTime;
        while(timeAccumulator >= fixedDeltaTime)
        {
            bodies.ForEach(body => body.Step(fixedDeltaTime));
            bodies.ForEach(body => Integrator.SemiImplicitEuler(body, fixedDeltaTime));

            if (collision)
            {
                bodies.ForEach(body => body.shape.color = Color.white);
                broadPhase.Build(aabb, bodies);

                Collision.CreateBroadPhaseContacts(broadPhase, bodies, out List<Contact> contacts);    
                Collision.CreateNarrowPhaseContacts(ref contacts);

                contacts.ForEach(contact => Collision.UpdateContactInfo(ref contact));

                ContactSolver.Resolve(contacts);
                if (CollisionDebug)
                {
                    contacts.ForEach(contact => { contact.bodyA.shape.color = Color.green; contact.bodyB.shape.color = Color.green; });
                }
            }

            timeAccumulator -= fixedDeltaTime;
        }
        if (CollisionDebug)
        {
            broadPhase.Draw();
        }
        collisionText.value = "Broad Phase: " + BroadPhase.potentialCollisionCount.ToString();

        if (wrap) { bodies.ForEach(body => body.position = Utilities.Wrap(body.position, -size, size)); }

        bodies.ForEach(body => body.force = Vector2.zero);
        bodies.ForEach(body => body.acceleration = Vector2.zero);
    }
}
