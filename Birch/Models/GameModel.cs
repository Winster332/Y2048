using System.Numerics;
using Birch.Graphics;
using Birch.Physics;

namespace Birch.Models;

public class Transform
{
    public float X { get; private set; }
    public float Y { get; private set; }
    public float Angle { get; private set; }
    public Vector2 ScalePosition { get; private set; }
    public float ScaleX { get; private set; }
    public float ScaleY { get; private set; }

    public Vector2 GetPosition() => new Vector2(X, Y);

    public Transform()
    {
        X = 0;
        Y = 0;
        Angle = 0;
        ScalePosition = Vector2.Zero;
        ScaleX = 1;
        ScaleY = 1;
    }

    public void Translate(float x, float y)
    {
        X = x;
        Y = y;
    }

    public void TranslateX(float x)
    {
        X = x;
    }

    public void TranslateY(float y)
    {
        Y = y;
    }

    public void Rotate(float radians)
    {
        Angle = radians;
    }

    public void Scale(float s)
    {
        ScaleX = s;
        ScaleY = s;
    }

    public void Scale(float sx, float sy)
    {
        ScaleX = sx;
        ScaleY = sy;
    }

    public void Scale(float sx, float sy, float px, float py)
    {
        ScaleX = sx;
        ScaleY = sy;
        ScalePosition = new Vector2(px, py);
    }
}

public delegate void GameModelContactEventHandler<TA, TB>(TA a, TB b) where TA : GameModel where TB : GameModel;

public abstract class GameModel
{
    public Guid Id { get; }
    public Transform Transform { get; set; }
    public RigidBody? RigidBody { get; set; }
    public bool Visible { get; set; }
    public bool Removed { get; private set; }
    internal GameModelStorage? _storage;
    public event GameModelContactEventHandler<GameModel, GameModel>? Contacted;

    protected GameModel()
    {
        Id = Guid.NewGuid();
        Visible = true;
        Transform = new Transform();
        RigidBody = null;
        Removed = false;
    }

    internal void Initialize(GameModelStorage storage)
    {
        _storage = storage;
    }

    protected GameModel? FindModelById(Guid id) => _storage?.FindModelById(id);
    protected T? FindModelById<T>(Guid id) where T : GameModel => _storage?.FindModelById<T>(id);

    public void Remove()
    {
        RigidBody?.Remove();
        Removed = true;
        _storage?.RemoveById(Id);
    }

    public void CreateRigidBody(IPhysicsService physics, RigidBodyDefinition definition)
    {
        var shapes = GetShapes();
        if (shapes.Length == 0)
            return;

        RigidBody = new RigidBody(Id, definition);
        RigidBody.SetPosition(new Vector2(Transform.X, Transform.Y));
        RigidBody.SetAngle(Transform.Angle);
        RigidBody.AddShapes(shapes);

        physics.CreateRigidBody(RigidBody);

        RigidBody.PositionUpdated += (_, position) =>
        {
            Transform.Translate(position.X, position.Y);
        };
        RigidBody.AngleUpdated += (_, angle) =>
        {
            Transform.Rotate(angle);
        };
        RigidBody.ContactBegin += RigidBodyOnContactBegin;
    }

    private void RigidBodyOnContactBegin(RigidBody bodya, RigidBody bodyb)
    {
        var modelB = FindModelById(bodyb.Id);
        var modelA = FindModelById(bodya.Id);

        if (modelB == null || modelA == null) return;

        if (bodya.Id == Id)
        {
            Contacted?.Invoke(this, modelB);
        }
        else if (bodyb.Id == Id)
        {
            Contacted?.Invoke(modelB, this);
        }
    }

    protected abstract Shape[] GetShapes();

    public void InvokeDraw(IGraphics g)
    {
        if (!Visible) return;

        g.Save();
        g.Translate(Transform.X, Transform.Y);
        g.RotateRadians(Transform.Angle);
        g.Scale(Transform.ScaleX, Transform.ScaleY, Transform.ScalePosition.X, Transform.ScalePosition.Y);

        Draw(g);

        g.Restore();
    }

    protected virtual void Draw(IGraphics graphics) { }
}
