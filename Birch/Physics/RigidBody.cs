using System.Numerics;
using Birch.Physics.Attributes;

namespace Birch.Physics;

public enum BodyModelType
{
    [DefinitionName("Статика")]
    Static,
    [DefinitionName("Динамика")]
    Dynamic,
    [DefinitionName("Кинематика")]
    Kinetic
}

public class RigidBodyDefinition
{
    [DefinitionName("Линейная скорость")]
    public Vector2 LinearVelocity { get; set; }

    [DefinitionName("Угловая скорость")]
    public float AngularVelocity { get; set; }

    [DefinitionName("Линейный демпфер")]
    public float LinearDamping {get; set; }

    [DefinitionName("Угловой демпфер")]
    public float AngularDamping { get; set; }

    [DefinitionName("Засыпающий")]
    public bool AllowSleep { get; set; }

    [DefinitionName("Пробуждаемый")]
    public bool Awake { get; set; }

    [DefinitionName("Фикс.угол")]
    public bool FixedRotation { get; set; }

    [DefinitionName("Доп.пров.коллизий")]
    public bool Bullet { get; set; }

    [DefinitionName("Гравитакция")]
    public float GravityScale { get; set; }

    [DefinitionName("Тип")]
    public BodyModelType Type { get; set; }

    public RigidBodyDefinition()
    {
        Type = BodyModelType.Dynamic;
        LinearVelocity = Vector2.Zero;
        AngularVelocity = 0;
        LinearDamping = 0;
        AngularDamping = 0;
        AllowSleep = false;
        Awake = true;
        FixedRotation = false;
        Bullet = true;
        // bd.active = true;
        GravityScale = 1f;
    }
}

public delegate void RigidBodyFieldUpdated<TField>(RigidBody body, TField field);
public delegate void RigidBodyContactEventHandler(RigidBody bodyA, RigidBody bodyB);

public class RigidBody : ICloneable
{
    public RigidBodyDefinition Definition { get; private set; }
    public Guid Id { get; private set; }
    public float Angle { get; private set; }
    public Vector2 Position { get; private set; }
    public float X => Position.X;
    public float Y => Position.Y;
    private IDictionary<Guid, Shape> _shapes;

    public event RigidBodyFieldUpdated<Vector2>? PositionUpdated;
    public event RigidBodyFieldUpdated<float>? AngleUpdated;
    public event RigidBodyContactEventHandler? ContactBegin;
    public bool IsRemoved { get; private set; }

    public RigidBody(Guid id, RigidBodyDefinition definition)
    {
        Definition = definition;
        Id = id;
        Position = new Vector2();
        Angle = 0;
        _shapes = new Dictionary<Guid, Shape>();
        IsRemoved = false;
    }

    public RigidBody(RigidBodyDefinition definition)
    {
        Definition = definition;
        Id = Guid.NewGuid();
        Position = new Vector2();
        Angle = 0;
        _shapes = new Dictionary<Guid, Shape>();
    }

    public int CountShapes => _shapes.Count;
    public IEnumerable<Shape> GetShapes() => _shapes.Values;

    public Shape? GetShapeById(Guid id) => !_shapes.ContainsKey(id) ? null : _shapes[id];

    public TShape? GetShapeById<TShape>(Guid id) where TShape : Shape
    {
        var shape = GetShapeById(id);
        if (shape?.GetType() != typeof(TShape)) return null;

        return (TShape)shape;
    }

    public void InvokeContactBegin(RigidBody body)
    {
        ContactBegin?.Invoke(this, body);
    }

    public void AddShapes(IEnumerable<Shape> shapes)
    {
        foreach (var shape in shapes)
        {
            AddShape(shape);
        }
    }

    public void AddShape(Shape shape)
    {
        _shapes.Add(shape.Id, shape);

        shape.Initialize(this);
    }

    public void SetPosition(Vector2 position, bool notify = false)
    {
        Position = position;

        if (notify) PositionUpdated?.Invoke(this, position);
    }

    public void SetAngle(float angle, bool notify = false)
    {
        Angle = angle;

        if (notify) AngleUpdated?.Invoke(this, angle);
    }

    public void RemoveShape(Guid shapeId)
    {
        if (_shapes.ContainsKey(shapeId))
        {
            _shapes.Remove(shapeId);
        }
    }

    public object Clone()
    {
        var model = new RigidBody(Definition);
        model.SetPosition(new Vector2(X, Y));
        // model.SetAngle(Angle);

        foreach (var shape in _shapes.Values)
        {
            model.AddShape((Shape)shape.Clone());
        }

        return model;
    }

    public void Remove()
    {
        IsRemoved = true;
    }
}