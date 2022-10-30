using System.Drawing;
using Birch.UI.Screens;

namespace Birch.System;

public interface IAppContext
{
    public ScreenService ScreenService { get; }
    public SizeF GetScreenSize();
    public void SetScreenSize(float width, float height);
}