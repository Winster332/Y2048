using System.Numerics;
using Box2D.NetStandard.Collision;
using Box2D.NetStandard.Dynamics.Bodies;
using Box2D.NetStandard.Dynamics.Contacts;
using Box2D.NetStandard.Dynamics.Fixtures;
using Box2D.NetStandard.Dynamics.World;
using Box2D.NetStandard.Dynamics.World.Callbacks;
using B2D = Box2D;

namespace Birch.Physics.Box2D;

public class ContactResolver : ContactListener
{
    public override void BeginContact(in Contact contact)
    {
    }

    public override void EndContact(in Contact contact)
    {
        var bodyA = contact.GetFixtureA().GetBody();
        var bodyB = contact.GetFixtureB().GetBody();

        if (bodyA.UserData != null && bodyA.UserData.GetType() == typeof(RigidBody) &&
            bodyB.UserData != null && bodyB.UserData.GetType() == typeof(RigidBody))
        {
            var rbA = bodyA.GetUserData<RigidBody>();
            var rbB = bodyB.GetUserData<RigidBody>();

            rbA.InvokeContactBegin(rbB);
            // rbB.InvokeContactBegin(rbA);
        }
    }

    public override void PreSolve(in Contact contact, in Manifold oldManifold)
    {
    }

    public override void PostSolve(in Contact contact, in ContactImpulse impulse)
    {
    }
}

public class PhysicsBox2DService : IPhysicsService
{
    public const float WorldScale = 30f;

    private World _world;
    public PhysicsBox2DService()
    {
        _world = new World(new Vector2(0.000000000000000e+00f, 1.000000000000000e+01f));
        _world.SetContactListener(new ContactResolver());
    }

    public void Step(float dt, int velocityIterations, int positionIterations)
    {
        try
        {
            _world.Step(dt, velocityIterations, positionIterations);
        }
        catch (Exception)
        {
        }

        for (var body = _world.GetBodyList(); body != null; body = body.GetNext())
        {
            if (body.UserData == null)
                continue;

            var model = body.GetUserData<Birch.Physics.RigidBody>();
            if (model.IsRemoved)
            {
                _world.DestroyBody(body);
                continue;
            }

            model.SetPosition(body.Position * WorldScale, true);
            model.SetAngle(body.GetAngle(), true);
            model.Definition.LinearVelocity = body.GetLinearVelocity();
            model.Definition.AngularVelocity = body.GetAngularVelocity();
            // for (var fixture = body.GetFixtureList(); fixture != null; fixture = fixture.GetNext())
            // {
            //     Console.WriteLine("1213");
            // }
        }
    }

    public void CreateRigidBody(RigidBody body)
    {
        var bodyDefinition = BuildBodyDefinition(body);

        var b2dBody = _world.CreateBody(bodyDefinition);

        foreach (var modelShape in body.GetShapes())
        {
            var fixtureDefinition = BuildShape(modelShape);

            if (fixtureDefinition == null) continue;

            b2dBody.CreateFixture(fixtureDefinition);
        }
        // return bodyDef;
    }

    public void DestroyBody(RigidBody b)
    {
        for (var body = _world.GetBodyList(); body != null; body = body.GetNext())
        {
            if (body.UserData == null)
                continue;

            var model = body.GetUserData<Birch.Physics.RigidBody>();
            if (model.Id == b.Id)
                _world.DestroyBody(body);
        }
    }


    private BodyDef BuildBodyDefinition(RigidBody body)
    {
        var bodyDef = new BodyDef();
        bodyDef.userData = body;
        bodyDef.type = ConvertBodyModelType(body.Definition.Type);
        bodyDef.position = body.Position / WorldScale;
        bodyDef.angle = body.Angle / WorldScale;
        bodyDef.linearVelocity = body.Definition.LinearVelocity;
        bodyDef.angularVelocity = body.Definition.AngularVelocity;
        bodyDef.linearDamping = body.Definition.LinearDamping;
        bodyDef.angularDamping = body.Definition.AngularDamping;
        bodyDef.allowSleep = body.Definition.AllowSleep;
        bodyDef.awake = body.Definition.Awake;
        bodyDef.fixedRotation = body.Definition.FixedRotation;
        bodyDef.bullet = body.Definition.Bullet;
        // bd.active = true;
        bodyDef.gravityScale = body.Definition.GravityScale;

        return bodyDef;
    }

    private FixtureDef? BuildShape(Birch.Physics.Shape modelShape)
    {
        var shape = default(B2D.NetStandard.Collision.Shapes.Shape);

        if (modelShape.GetType() == typeof(Birch.Physics.Shapes.PolygonShape))
        {
            shape = BuildPolygonShape((Birch.Physics.Shapes.PolygonShape)modelShape);
        }
        else if (modelShape.GetType() == typeof(Birch.Physics.Shapes.CircleShape))
        {
            shape = BuildCircleShape((Birch.Physics.Shapes.CircleShape)modelShape);
        }

        if (shape == null) return null;

        var fixtureDef = new FixtureDef();
        fixtureDef.userData = modelShape;
        fixtureDef.friction = modelShape.Definition.Friction;
        fixtureDef.restitution = modelShape.Definition.Restitution;
        fixtureDef.density = modelShape.Definition.Density;
        fixtureDef.isSensor = modelShape.Definition.IsSensor;
        fixtureDef.filter.categoryBits = modelShape.Definition.Filter.CategoryBits;
        fixtureDef.filter.maskBits = modelShape.Definition.Filter.MaskBits;
        fixtureDef.filter.groupIndex = modelShape.Definition.Filter.GroupIndex;
        fixtureDef.shape = shape;

        return fixtureDef;
    }

    private B2D.NetStandard.Collision.Shapes.Shape BuildPolygonShape(Birch.Physics.Shapes.PolygonShape modelPolygon)
    {
        var polygon = new B2D.NetStandard.Collision.Shapes.PolygonShape();
        polygon.Set(modelPolygon.GetVertices().Select(x => x.Position / WorldScale).ToArray());
        return polygon;
    }

    private B2D.NetStandard.Collision.Shapes.Shape BuildCircleShape(Birch.Physics.Shapes.CircleShape modelCircle)
    {
        var circle = new B2D.NetStandard.Collision.Shapes.CircleShape();
        circle.Center = modelCircle.Center / WorldScale;
        circle.Radius = modelCircle.Radius / WorldScale;
        return circle;
    }

    private BodyType ConvertBodyModelType(BodyModelType type)
    {
        if (type == BodyModelType.Dynamic) return BodyType.Dynamic;
        if (type == BodyModelType.Kinetic) return BodyType.Kinematic;
        if (type == BodyModelType.Static) return BodyType.Static;

        return BodyType.Static;
    }
}