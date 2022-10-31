using System.Numerics;
using Birch.FileSystem;
using Birch.Graphics;
using Birch.Physics;
using Birch.UI.Screens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Y2048.Core.Models;
using Y2048.Core.Services;
using Y2048.Core.UI;
using Timer = Birch.Common.Timer;

namespace Y2048.Core.Screens;

public enum GameState
{
    Wait,
    Running,
    Finish
}

public class GameScreen : Screen
{
    private BoxGeneratorService _boxGenerator;
    private IPhysicsService _physics;
    private DeadLineService _deadLine;
    public BoxModel SpawnModel;
    public ManipulatorService Manipulator { get; private set; }
    public BoundMapModel MapModel { get; private set; }
    public Timer GenerateBoxTimer { get; private set; }
    public GameModelStorage ModelStorage { get; }
    private StateService _state;
    public GameState State { get; private set; }
    public GameOverLayout GameOverLayout { get; private set; }
    private IFileSystem _fileSystem;

    public GameScreen(BoxGeneratorService boxGenerator, StateService state, ManipulatorService manipulator, IPhysicsService physics, IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
        State = GameState.Wait;
        ModelStorage = new GameModelStorage();
        _boxGenerator = boxGenerator;
        _physics = physics;
        _state = state;
        _boxGenerator.Initialize(_physics, ModelStorage, state);
        Manipulator = manipulator;
        GameOverLayout = new GameOverLayout(this, _fileSystem);
        GameOverLayout.Restart += (_, _) => Restart();

        var value = _fileSystem.ReadStringFromAsset("data.json");
        if (!string.IsNullOrEmpty(value))
        {
            _state.SetMaxBalls(JToken.Parse(value)["maxBalls"]?.ToObject<int>() ?? 0);
        }
    }

    private void Restart()
    {
        foreach (var box in ModelStorage.GetAllModels<BoxModel>().ToArray())
        {
            if(box.RigidBody == null) continue;
            _physics.DestroyBody(box.RigidBody);
            ModelStorage.RemoveById(box.Id);
        }

        _state.Reset();

        State = GameState.Running;
        GameOverLayout.Hide();
    }

    protected override void Start()
    {
        MapModel = new BoundMapModel(Width, Height);
        MapModel.CreateRigidBody(_physics, new RigidBodyDefinition
        {
            Type = BodyModelType.Static
        });
        _deadLine = new DeadLineService(Width, Height, MapModel.Thickness);

        AddView(GameOverLayout);
        var next = _boxGenerator.GenerateNextData();
        var boxSize = (Width - MapModel.Thickness * 2) / 6f;
        SpawnModel = new BoxModel(_boxGenerator, next.number, next.color)
        {
            Width = boxSize,
            Height = boxSize
        };
        SpawnModel.Transform.TranslateY((-Height/2+SpawnModel.Height/2) + Height/20f);

        Manipulator.SetHorizontalLimit(-Width / 2 + SpawnModel.Width / 2, Width / 2 - SpawnModel.Width / 2);
        Manipulator.Moved += ManipulatorOnMoved;
        Manipulator.Released += ManipulatorOnReleased;

        Update += OnUpdate;
        Draw += OnDraw;
        TouchDown += (_, s) => Manipulator.InvokeTouchDown(s.AbsolutePosition.X - Width/2f, s.AbsolutePosition.Y, SpawnModel.Transform.X, SpawnModel.Transform.Y);
        TouchMove += (_, s) => Manipulator.InvokeTouchMove(s.AbsolutePosition.X - Width/2f, s.AbsolutePosition.Y);
        TouchUp   += (_, s) => Manipulator.InvokeTouchUp  (s.AbsolutePosition.X - Width/2f, s.AbsolutePosition.Y);

        GenerateBoxTimer = new Timer();
        GenerateBoxTimer.SetInterval(TimeSpan.FromSeconds(1));
        GenerateBoxTimer.Elapsed += () =>
        {
            SpawnModel.Visible = true;
            var next = _boxGenerator.GenerateNextData();
            SpawnModel.SetNumber(next.number);
            SpawnModel.Paint.Color = next.color;
            SpawnModel.Transform.TranslateX(0);
        };
    }

    private void ManipulatorOnReleased(object? sender, Vector2 e)
    {
        if (State == GameState.Finish) return;
        if (!SpawnModel.Visible) return;
        var box = new BoxModel(_boxGenerator, SpawnModel.Number, SpawnModel.Paint.Color)
        {
            Width = SpawnModel.Width,
            Height = SpawnModel.Height
        };
        box.Transform.Translate(SpawnModel.Transform.X, SpawnModel.Transform.Y);
        box.CreateRigidBody(_physics, new RigidBodyDefinition
        {
        });
        ModelStorage.Add(box);
        GenerateBoxTimer.Start();

        SpawnModel.Visible = false;
    }

    private void ManipulatorOnMoved(object? sender, Vector2 e)
    {
        if (SpawnModel.Visible && State != GameState.Finish) SpawnModel.Transform.TranslateX(e.X);
    }

    private void OnUpdate(float dt)
    {
        if (State == GameState.Finish) return;

        _boxGenerator.Update();
        _physics.Step(dt / 30.0f, 20, 20);
        GenerateBoxTimer.Update();

        if (GenerateBoxTimer.IsEnabled) return;

        foreach (var box in ModelStorage.GetAllModels<BoxModel>())
        {
            if (_deadLine.PointInDeadArea(box.Transform.X, box.Transform.Y))
            {
                ShowGameOver();
            }
        }
    }

    private void ShowGameOver()
    {
        State = GameState.Finish;
        GameOverLayout.Show(_state.CurrentDisplay, _state.MaxBallsDisplay);

        var data = JsonConvert.SerializeObject(new
        {
            maxBalls = _state.MaxBalls
        });
        _fileSystem.RewriteToAsset("data.json", data);
    }

    private void OnDraw(IGraphics g)
    {
        g.Clear(new Color(69,62,69));
        g.Save();
        g.Translate(Width/2, Height/2);

        g.Save();
        g.Translate(Width/2-10, -Height/2+40);
        _state.DrawGameCurrent(g);
        g.Restore();

        _deadLine.Draw(g);
        DrawSpawnShadow(g);
        // Platform.InvokeDraw(g);
        ModelStorage.Draw(g);
        MapModel.InvokeDraw(g);

        _boxGenerator.Draw(g);

        g.Restore();
    }

    private void DrawSpawnShadow(IGraphics g)
    {
        if (SpawnModel.Visible)
        {
            var x = SpawnModel.Transform.X;
            var y = SpawnModel.Transform.Y;
            var w = SpawnModel.Width;
            var h = SpawnModel.Height;

            g.DrawRect(x - w / 2, y - h / 2, w, Height, new Paint
            {
                Style = PaintStyle.Fill,
                Shader = Shader.CreateLinearGradient(new Vec2(w / 2, y - h / 2),
                    new Vec2(w / 2, Height / 2), new[]
                    {
                        new Color(255, 255, 255, 10),
                        new Color(69, 62, 69, 10)
                    }, ShaderTileMode.Clamp)
            });
        }

        SpawnModel.InvokeDraw(g);
    }
}