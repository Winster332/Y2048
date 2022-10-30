using Birch.Graphics;
using Birch.Models;

namespace Birch.Physics;

public class GameModelStorage
{
    private Dictionary<Guid, GameModel> _boxes;
    public GameModelStorage()
    {
        _boxes = new Dictionary<Guid, GameModel>();
    }

    public void Add(GameModel model)
    {
        model.Initialize(this);
        _boxes.Add(model.Id, model);
    }

    public IEnumerable<GameModel> GetAllModels() => _boxes.Values;

    public IEnumerable<T> GetAllModels<T>() where T : GameModel
    {
        return GetAllModels().Where(x => x.GetType() == typeof(T)).Select(x => (T)x);
    }

    public GameModel? FindModelById(Guid id)
    {
        if (!_boxes.ContainsKey(id)) return null;

        return _boxes[id];
    }

    public T? FindModelById<T>(Guid id) where T : GameModel
    {
        var model = FindModelById(id);
        if (model == null) return null;

        return (T)_boxes[id];
    }

    public void RemoveById(Guid id)
    {
        if (_boxes.ContainsKey(id))
            _boxes.Remove(id);
    }

    public void Clear()
    {
        _boxes.Clear();
    }

    public void Draw(IGraphics g)
    {
        foreach (var box in _boxes.Values)
        {
            box.InvokeDraw(g);
        }
    }
}
