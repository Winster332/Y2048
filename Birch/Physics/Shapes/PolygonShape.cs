using System.Numerics;
using Birch.Extensions;

namespace Birch.Physics.Shapes;

public class Vertex
{
    public Guid Id { get; private set; }
    public Vector2 Position { get; private set; }
    public bool Selected { get; private set; }

    public Vertex(Guid id, Vector2 position, bool selected)
    {
        Id = id;
        Position = position;
        Selected = selected;
    }

    public void SetPosition(Vector2 position)
    {
        Position = position;
    }

    public void SetSelect(bool value)
    {
        Selected = value;
    }
}

public class PolygonShape : Shape
{
    private Dictionary<Guid, Vertex> _vertices;
    private Dictionary<int, Guid> _indexes;

    public IEnumerable<Vector2> GetPolygon() => _vertices.Values.Select(x => x.Position);

    public PolygonShape(Vector2[] vertices)
    {
        _vertices = vertices
            .Select(v => new Vertex(Guid.NewGuid(), v, false))
            .ToDictionary(x => x.Id);
        _indexes = new Dictionary<int, Guid>();

        RebuildIndexes();
    }

    public void SetVertices(Vector2[] vertices)
    {
        _vertices.Clear();

        _vertices = vertices
            .Select(v => new Vertex(Guid.NewGuid(), v, false))
            .ToDictionary(x => x.Id);

        RebuildIndexes();
    }

    private void RebuildIndexes()
    {
        _indexes.Clear();
        var index = 0;
        foreach (var g in _vertices)
        {
            _indexes.Add(index, g.Key);
            index++;
        }
    }

    public override void SetPosition(Vector2 position)
    {
        foreach (var vertex in _vertices.Values)
        {
            var normalize = (vertex.Position - position).Normalize();
            var length = vertex.Position.Distance(position);
            var diff = normalize * length;
            // var diff = position + vertex.Position;

            vertex.SetPosition(diff);
        }
    }

    public override void SetAngle(float angle, Vector2 position)
    {
        var transformedPolygon = _vertices.Values.Select(x => position + x.Position).ToArray().RotatePoints(position, angle);

        for (var i = 0; i < transformedPolygon.Length; i++)
        {
            var vertex = GetVertex(i);

            vertex?.SetPosition(transformedPolygon[i] - position);
        }
    }

    public IEnumerable<Vertex> GetVertices() => _vertices.Values;

    public void SetVertexSelect(Guid vertexId, bool value)
    {
        if (_vertices.ContainsKey(vertexId)) _vertices[vertexId].SetSelect(value);
    }

    public Vertex? GetVertex(Guid id)
    {
        if (_vertices.ContainsKey(id))
        {
            return _vertices[id];
        }

        return null;
    }

    public Vertex? GetVertex(int index)
    {
        return GetVertex(_indexes[index]);
    }

    public void SetVertexPosition(int index, Vector2 position)
    {
        _vertices[_indexes[index]].SetPosition(position);
    }

    public override object Clone()
    {
        var newVertices = new Vector2[_vertices.Count];
        var kl = GetPolygon().ToArray();
        kl.CopyTo(newVertices, 0);
        return new PolygonShape(newVertices);
    }

    public void ScaleX(float scale, float angle)
    {
        // var transformedPolygon = _vertices.Values.Select(x => position + x.Position).ToArray().RotatePoints(position, angle);
        for (var i = 0; i < _vertices.Count; i++)
        {
            SetVertexPosition(i, ScaledPoint(GetVertex(i).Position, scale, scale));
        }
    }

    public void ScaleY(float scale)
    {
        for (var i = 0; i < _vertices.Count; i++)
        {
            SetVertexPosition(i, ScaledPoint(GetVertex(i).Position, scale, scale));
        }
    }

    public void Scale(float scaleX, float scaleY)
    {
        for (var i = 0; i < _vertices.Count; i++)
        {
            SetVertexPosition(i, ScaledPoint(GetVertex(i).Position, scaleX, scaleY));
        }
    }

    Vector2 ScaledPoint(Vector2 pt, float scaleX, float scaleY)
    {
        return new Vector2(pt.X * scaleX, (pt.Y * scaleY));
    }

    public Guid? DivideEdge(Guid vertexAId, Guid vertexBId, Vector2 point)
    {
        var vertexA = GetVertex(vertexAId);
        var vertexB = GetVertex(vertexBId);

        if (vertexA == null || vertexB == null) return null;

        var vertices = _vertices.Values.ToArray();

        var newVertexes = new List<Vertex>();
        for (int i = 0, j = 1; j < vertices.Length + 1; i++, j++)
        {
            var a = vertices[i];
            newVertexes.Add(a);
            var b = j < vertices.Length ? vertices[j] : vertices[0];

            if (a.Id == vertexAId && b.Id == vertexBId)
            {
                var vertex = new Vertex(Guid.NewGuid(), point, false);
                newVertexes.Add(vertex);
            }
        }

        if (newVertexes.Count != vertices.Length)
        {
            _vertices.Clear();
            foreach (var newVertex in newVertexes)
            {
                _vertices.Add(newVertex.Id, newVertex);
            }

            RebuildIndexes();
        }

        return null;
    }

    public void RemoveVertex(Guid vertexId)
    {
        if (!_vertices.ContainsKey(vertexId)) return;

        _vertices.Remove(vertexId);
        RebuildIndexes();
    }

    public static PolygonShape CreateBox(float x, float y, float w, float h)
    {
        return new PolygonShape(new[]
        {
            new Vector2(x-w / 2, y-h / 2),
            new Vector2(x+w / 2, y-h / 2),
            new Vector2(x+w / 2, y+h / 2),
            new Vector2(x-w / 2, y+h / 2),
        });
    }
}
