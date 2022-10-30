using Birch.Graphics;
using Birch.UI.Events.Keyboard;
using Birch.UI.Events.Touch;

namespace Birch.UI;

public abstract class View
{
    public string Id { get; private set; }
    public const string X_FIELD = "x";
    public const string Y_FIELD = "y";
    private Dictionary<string, object?> _fields;
    private Dictionary<string, (Func<object> Getter, Action<object?> Setter)> _fieldsReferences;
    public bool CanFocusable { get; set; }
    public bool IsFocus { get; protected set; }

    public float X
    {
        get => GetField<float>(X_FIELD) + (Parent?.X ?? 0f);
        set => SetField(X_FIELD, value);
    }
    public float Y
    {
        get => GetField<float>(Y_FIELD) + (Parent?.Y ?? 0f);
        set => SetField(Y_FIELD, value);
    }

    protected List<View> Children;
    protected View? Parent;

    public event ViewEventHandler? Loaded;

    public event ViewEventHandler<KeyboardState>? KeyboardDown;
    public event ViewEventHandler<KeyboardState>? KeyboardUp;
    public event ViewEventHandler<KeyboardPress>? KeyboardPress;

    public event ViewEventHandler<TouchState>? TouchDown;
    public event ViewEventHandler<TouchState>? TouchMove;
    public event ViewEventHandler<TouchState>? TouchUp;

    public event ViewEventHandler<TouchState>? TouchEnter;
    public event ViewEventHandler<TouchState>? TouchLeave;
    public event ViewEventHandler<TouchState>? Click;
    public event ViewEventHandler<TouchState>? DoubleClick;

    public event ViewUpdateEventHandler<IGraphics>? Draw;
    public event ViewUpdateEventHandler<float>? Update;
    public bool Visible { get; set; }

    protected View()
    {
        Id = Guid.NewGuid().ToString();
        Children = new List<View>();
        Visible = true;
        CanFocusable = false;
        IsFocus = false;

        _fields = new Dictionary<string, object?>();
        _fieldsReferences = new Dictionary<string, (Func<object> Getter, Action<object?> Setter)>();

        RegistryField(X_FIELD, 0f);
        RegistryField(Y_FIELD, 0f);
    }

    public T GetChildrenById<T>(string id) where T : View => (T) Children.First(x => x.Id == id);

    public IEnumerable<T> GetChildrenByType<T>() where T : View
    {
        var targetType = typeof(T);
        return Children.Where(x => x.GetType() == targetType).Select(x => (T)x);
    }

    public void SetId(string id)
    {
        Id = id;
    }

    internal virtual void Initialize()
    {
        Loaded?.Invoke(this);
    }

    public void SetVisible(bool visible)
    {
        Visible = visible;
    }

    public abstract bool IsIntersectWith(float x, float y);

    protected void RegistryFieldReference(string key, Func<object> getter, Action<object>? setter = null)
    {
        if (_fields.ContainsKey(key)) _fields.Remove(key);

        setter ??= _ => { };

        if (!_fieldsReferences.ContainsKey(key)) _fieldsReferences.Add(key, (getter, setter));
        else _fields[key] = getter;
    }

    protected void RegistryField<T>(string key, T defaultValue)
    {
        if (_fieldsReferences.ContainsKey(key)) _fieldsReferences.Remove(key);

        if (!_fields.ContainsKey(key)) _fields.Add(key, defaultValue);
        else _fields[key] = defaultValue;
    }

    public T? GetField<T>(string key)
    {
        if (_fields.ContainsKey(key))
        {
            return (T?)_fields[key];
        }
        else if (_fieldsReferences.ContainsKey(key))
        {
            return (T?)_fieldsReferences[key].Getter();
        }

        return default;
    }

    protected void SetField<T>(string key, T value)
    {
        if (_fields.ContainsKey(key))
        {
            _fields[key] = value;
        }
        else if (_fieldsReferences.ContainsKey(key))
        {
            _fieldsReferences[key].Setter(value);
        }
    }

    public void AddView(View view)
    {
        view.Parent = this;
        Children.Add(view);

        view.Initialize();
    }

    public void ClearViews()
    {
        Children.Clear();
    }

    public void SetPosition(float x, float y)
    {
        X = x;
        Y = y;
    }

    public void InvokeUpdate(float dt)
    {
        if (!Visible) return;

        Update?.Invoke(dt);

        foreach (var view in Children)
        {
            view.InvokeUpdate(dt);
        }
    }

    // private Camera? _camera = null;
    // public void SetComputedCamera(Camera? camera)
    // {
    //     IsComputedCamera = camera == null;
    //     _camera = camera;
    // }
    // public bool IsComputedCamera { get; private set; } = true;

    public virtual void InvokeDraw(IGraphics g)
    {
        if (!Visible) return;

        Draw?.Invoke(g);

        foreach (var view in Children)
        {
            view.InvokeDraw(g);
        }
    }

    // protected abstract void Update();
    // protected abstract void Draw(SKCanvas canvas);

    protected bool _cursorOverView = false;
    private DateTime? _lastClickDateTime = DateTime.Now;

    private TouchState NextTouchState(TouchState state)
    {
        var next = state.NewTarget();

        // if (!IsComputedCamera && _camera != null)
        // {
        //     next.SetPosition(next.AbsolutePosition - _camera.Position);
        // }

        return next;
    }

    public virtual bool InvokeTouchDown(TouchState state)
    {
        if (!Visible) return false;

        var currentState = NextTouchState(state);

        if (!IsIntersectWith(currentState.AbsolutePosition.X, currentState.AbsolutePosition.Y)) return false;

        var childrenComputed = false;
        foreach (var child in Children.Where(x => x.Visible))
        {
            var isComputed = child.InvokeTouchDown(currentState);

            if (isComputed)
            {
                childrenComputed = true;
                break;
            }
        }

        if (!childrenComputed)
        {
            TouchDown?.Invoke(this, currentState);
        }

        return childrenComputed || Children.Count == 0 || currentState.IsStopPropagation;
    }

    public virtual bool InvokeTouchMove(TouchState state)
    {
        if (!Visible) return false;

        var currentState = NextTouchState(state);

        if (!IsIntersectWith(currentState.AbsolutePosition.X, currentState.AbsolutePosition.Y))
        {
            // InvokeTouchLeave(state);
            if (_cursorOverView)
            {
                TouchLeave?.Invoke(this, currentState);
                _cursorOverView = false;
            }
            return false;
        }

        // InvokeTouchEnter(state);
        if (!_cursorOverView)
        {
            TouchEnter?.Invoke(this, currentState);
            _cursorOverView = true;
        }

        var childrenComputed = false;
        foreach (var child in Children.Where(x => x.Visible))
        {
            var isComputed = child.InvokeTouchMove(currentState);

            if (isComputed)
            {
                childrenComputed = true;
                break;
            }
        }

        // Children.ForEach(x => x.InvokeTouchMove(state));

        if (!childrenComputed)
        {
            TouchMove?.Invoke(this, currentState);
        }

        return childrenComputed || Children.Count == 0;
    }

    private void InvokeTouchEnter(TouchState state)
    {
    }

    private void InvokeTouchLeave(TouchState state)
    {
    }

    public virtual bool InvokeTouchUp(TouchState state)
    {
        if (!Visible) return false;

        var currentState = NextTouchState(state);

        if (!IsIntersectWith(currentState.AbsolutePosition.X, currentState.AbsolutePosition.Y)) return false;

        var childrenComputed = false;
        foreach (var child in Children.Where(x => x.Visible))
        {
            var isComputed = child.InvokeTouchUp(currentState);

            if (isComputed)
            {
                childrenComputed = true;
                break;
            }
        }
        // var childrenComputed = Children.Select(x => x.InvokeTouchUp(state)).FirstOrDefault();

        if (!childrenComputed)
        {
            TouchUp?.Invoke(this, currentState);
            Click?.Invoke(this, currentState);
            var diff = DateTime.Now - _lastClickDateTime;
            _lastClickDateTime = DateTime.Now;

            if (_lastClickDateTime != null && diff <= TimeSpan.FromSeconds(0.5))
            {
                DoubleClick?.Invoke(this, currentState);
                _lastClickDateTime = null;
            }
        }

        return childrenComputed || Children.Count == 0;
        // Children.ForEach(x => x.InvokeTouchUp(state));
    }

    public void InvokeKeyDown(KeyboardState state)
    {
        KeyboardDown?.Invoke(this, state);
    }

    public void InvokeKeyUp(KeyboardState state)
    {
        KeyboardUp?.Invoke(this, state);
    }

    public void InvokeKeyPress(KeyboardPress state)
    {
        KeyboardPress?.Invoke(this, state);

        foreach (var child in Children)
        {
            child.InvokeKeyPress(state);
        }
    }
}

public delegate void ViewEventHandler<in T>(View view, T args);
public delegate void ViewEventHandler(View view);
public delegate void ViewUpdateEventHandler<in T>(T args);
