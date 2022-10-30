using System.Numerics;
using Birch.Physics.Attributes;

namespace Birch.Physics;

public class ShapeDefinitionFilter
{
    public ushort CategoryBits { get; set; }
    public ushort MaskBits { get; set; }
    public short GroupIndex { get; set; }

    public ShapeDefinitionFilter()
    {
        CategoryBits = 1;
        MaskBits = 65535;
        GroupIndex = 0;
    }
}

public class ShapeDefinition
{
    [DefinitionName("Трение")]
    public float Friction { get; set; }

    [DefinitionName("Восстановление")]
    public float Restitution { get; set; }

    [DefinitionName("Плотность")]
    public float Density { get; set; }

    [DefinitionName("Сенсор")]
    public bool IsSensor { get; set; }
    public ShapeDefinitionFilter Filter { get; }

    public ShapeDefinition()
    {
        Friction = 3.000000119209290e-01f;
        Restitution = 0.4f;
        Density = 1f;
        IsSensor = false;
        Filter = new ShapeDefinitionFilter();
    }
}

public abstract class Shape : ICloneable
{
    public ShapeDefinition Definition { get; }
    public Guid Id { get; private set; }
    protected RigidBody Body;

    protected Shape()
    {
        Id = Guid.NewGuid();
        Definition = new ShapeDefinition();
    }

    public Guid GetBodyId() => Body.Id;

    public void Initialize(RigidBody body)
    {
        Body = body;
        SetAngle(body.Angle, body.Position);
    }

    public abstract object Clone();


    public abstract void SetPosition(Vector2 position);
    public abstract void SetAngle(float angle, Vector2 position);
}
