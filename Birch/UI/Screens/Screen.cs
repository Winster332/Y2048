using Birch.System;
using Birch.UI.Primitives;

namespace Birch.UI.Screens;

public abstract class Screen : RectangleView
{
    private IAppContext? _context;

    protected Screen()
    {
        RegistryFieldReference(WIDTH_FIELD, () => _context?.GetScreenSize().Width ?? 0f, _ => { });
        RegistryFieldReference(HEIGHT_FIELD, () => _context?.GetScreenSize().Height ?? 0f, _ => { });
    }

    internal void Initialize(IAppContext context)
    {
        _context = context;

        Start();
    }

    protected abstract void Start();
}