using System.Numerics;
using Birch.Extensions;
using Birch.Graphics;

namespace Y2048.Core.Services;

public class Particle
{
    public float X { get; set; }
    public float Y { get; set; }
    public float VelocityX { get; private set; }
    public float VelocityY { get; private set; }
    public float Opencity { get; set; }
    public float OpencityVelocity { get; set; }
    public float Radius { get; set; }
    public float ScaleVelocity { get; set; }
    public Color Color { get; set; }

    public Particle(float x, float y, float vx, float vy, float radius, float opencity, float opencityVelocity, float scaleVelocity, Color color)
    {
        X = x;
        Y = y;
        VelocityX = vx;
        VelocityY = vy;
        Opencity = opencity;
        OpencityVelocity = opencityVelocity;
        Radius = radius;
        ScaleVelocity = scaleVelocity;
        Color = color;
    }
}

public class ParticlesService
{
    private List<Particle> _particles;
    private Random _random;
    private float _particleSize;
    private Paint _paint;

    public ParticlesService()
    {
        _particles = new List<Particle>();
        _random = new Random();
        _particleSize = 20f;
        _paint = new Paint
        {
            Color = new Color(0, 0, 0),
            Style = PaintStyle.Fill
        };
    }

    public void Generate(Vector2 position, Color color)
    {
        var angle = _random.NextFloat(0, MathF.PI * 2);
        var distance = _random.NextFloat(10, 40f);

        var correctX = MathF.Sin(angle) * distance;
        var correctY = MathF.Cos(angle) * distance;

        var direction = ((position + new Vector2(correctX, correctY)) - position).Normalize() * 2f;

        var particle = new Particle(position.X + correctX, position.Y + correctY, direction.X, direction.Y, 5f, 1, -0.001f, -0.2f, color);
        _particles.Add(particle);
    }

    public void Generate(float x, float y, Color color, int count)
    {
        for (var i = 0; i < count; i++)
        {
            Generate(new Vector2(x, y), color);
        }
    }

    public void Update()
    {
        for (var i = 0; i < _particles.Count; i++)
        {
            var particle = _particles[i];
            particle.X += particle.VelocityX;
            particle.Y += particle.VelocityY;
            particle.Opencity += particle.OpencityVelocity;
            particle.Radius += particle.ScaleVelocity;

            if (particle.Opencity <= 0.01f || particle.Radius <= 0.01f)
            {
                _particles.RemoveAt(i);
            }
        }
    }

    public void Draw(IGraphics g)
    {
        for (var i = 0; i < _particles.Count; i++)
        {
            var particle = _particles[i];

            g.Save();
            g.Translate(particle.X, particle.Y);
            particle.Color = particle.Color.WithAlpha((byte)(255 * particle.Opencity));
            _paint.Color = particle.Color;
            g.DrawCircle(-_particleSize / 2, -_particleSize / 2, particle.Radius, _paint);
            g.Restore();
        }
    }
}
