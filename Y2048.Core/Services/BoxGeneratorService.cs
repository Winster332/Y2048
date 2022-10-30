using System.Numerics;
using Birch.Extensions;
using Birch.Graphics;
using Birch.Physics;
using Y2048.Core.Models;

namespace Y2048.Core.Services;

public abstract class BaseDeferredOperation
{
    public abstract void Execute();
}

public class DeferredOperation : BaseDeferredOperation
{
    private Action _handler;
    public DeferredOperation(Action handler)
    {
        _handler = handler;
    }

    public override void Execute()
    {
        _handler.Invoke();
    }
}

public class DeferredOperation<T1> : BaseDeferredOperation
{
    private Action<T1> _handler;
    private T1 Arg1;
    public DeferredOperation(T1 arg1, Action<T1> handler)
    {
        _handler = handler;
        Arg1 = arg1;
    }

    public override void Execute()
    {
        _handler.Invoke(Arg1);
    }
}

public class DeferredOperation<T1, T2> : BaseDeferredOperation
{
    private Action<T1, T2> _handler;
    private T1 Arg1;
    private T2 Arg2;
    public DeferredOperation(T1 arg1, T2 arg2, Action<T1, T2> handler)
    {
        _handler = handler;
        Arg1 = arg1;
        Arg2 = arg2;
    }

    public override void Execute()
    {
        _handler.Invoke(Arg1, Arg2);
    }
}

public class DeferredQueueOperations
{
    private Queue<BaseDeferredOperation> _operations;

    public DeferredQueueOperations()
    {
        _operations = new Queue<BaseDeferredOperation>();
    }

    public void Add<T1, T2>(T1 arg1, T2 arg2, Action<T1, T2> action)
    {
        var operation = new DeferredOperation<T1, T2>(arg1, arg2, action);
        _operations.Enqueue(operation);
    }

    public void Add<T1>(T1 arg, Action<T1> action)
    {
        var operation = new DeferredOperation<T1>(arg, action);
        _operations.Enqueue(operation);
    }

    public void Add(Action action)
    {
        var operation = new DeferredOperation(action);
        _operations.Enqueue(operation);
    }

    public void Flash()
    {
        while (_operations.TryDequeue(out var operation))
        {
            operation.Execute();
        }
    }
}

public class BoxGeneratorService
{
    public Dictionary<int, Color> BoxColors = new()
    {
        { 2, new Color(255,198,62) },
        { 4, new Color(90,203,135) },
        { 8, new Color(231,107,132) },
        { 16, new Color(92,189,227) },
        { 32, new Color(252,132,43) },
        { 64, new Color(250,120,225) },
        { 128, new Color(173,123,54) },
        { 256, new Color(153,153,153) },
        { 512, new Color(204,158,50) },
        { 1024, new Color(72,162,108) },
        { 2048, new Color(185,86,106) },
        { 4096, new Color(171,90,203) },
        { 8192, new Color(90,119,203) },
        { 16384, new Color(203,138,90) },
        { 32768, new Color(90,192,203) },
        { 65536, new Color(36,42,51) },
        { 131072, new Color(151,203,122) },
    };
    private ParticlesService _particles;
    private IPhysicsService _physics;
    private GameModelStorage _modelStorage;
    private Random _random;
    private DeferredQueueOperations _deferredQueue;
    private StateService _state;

    public BoxGeneratorService(ParticlesService particles)
    {
        _particles = particles;
        _random = new Random();
        _deferredQueue = new DeferredQueueOperations();
    }

    public void Initialize(IPhysicsService physics, GameModelStorage modelStorage, StateService state)
    {
        _physics = physics;
        _modelStorage = modelStorage;
        _state = state;
    }

    public void GenerateFrom(BoxModel a, BoxModel b)
    {
        _deferredQueue.Add(a, b, (boxA, boxB) =>
        {
            var nextNumber = boxA.Number + boxB.Number;
            var color = BoxColors[nextNumber];
            CreateParticles(boxA.Transform.GetPosition(), boxB.Transform.GetPosition(), color);
            _state.Apply(nextNumber);

            var box = new BoxModel(this, nextNumber, color)
            {
                Width = boxA.Width,
                Height = boxA.Height
            };
            var p = (boxA.RigidBody?.Definition.LinearVelocity.Length() ?? 0f) >
                    (boxB.RigidBody?.Definition.LinearVelocity.Length() ?? 0f)
                ? new Vector2(boxA.Transform.X, boxA.Transform.Y)
                : new Vector2(boxB.Transform.X, boxB.Transform.Y);
            box.Transform.Translate(p.X, p.Y);
            box.CreateRigidBody(_physics, new RigidBodyDefinition
            {
                LinearVelocity = (boxB.RigidBody?.Definition.LinearVelocity ?? Vector2.Zero) + (boxA.RigidBody?.Definition.LinearVelocity ?? Vector2.Zero),
                AngularVelocity = (boxB.RigidBody?.Definition.AngularVelocity ?? 0f) + (boxA.RigidBody?.Definition.AngularVelocity ?? 0f),
            });
            _modelStorage.Add(box);
        });
    }

    public (int number, Color color) GenerateNextData()
    {
        var variants = _modelStorage.GetAllModels<BoxModel>()
            .Select(x => x.Number)
            .Distinct()
            .ToArray();
        if (variants.Length == 0) return (2, BoxColors[2]);

        var index = _random.Next(0, variants.Length - 1);
        var number = variants[index];

        var needAddNew = _random.Next(-50, 10) >= 0;
        var needAddDiv = _random.Next(-10, 50) >= 0;
        if (needAddDiv && number != 2)
        {
            number /= 2;

            var needAddDoubleDiv = _random.Next(-50, 10) >= 0;
            if (needAddDoubleDiv && number != 2)
            {
                number /= 2;
            }
        }
        else if (needAddNew) number *= 2;

        return (number, BoxColors[number]);
    }

    public void Update()
    {
        _particles.Update();

        _deferredQueue.Flash();
    }

    public void Draw(IGraphics g)
    {
        _particles.Draw(g);
    }

    private Vector2 CreateParticles(Vector2 positionA, Vector2 positionB, Color color)
    {
        var distance = positionA.Distance(positionB);
        var positionBetweenAb = positionA + (positionB - positionA).Normalize() * (distance / 2);
        _particles.Generate(positionBetweenAb.X, positionBetweenAb.Y, color, 15);

        return positionBetweenAb;
    }
}