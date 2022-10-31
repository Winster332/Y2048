using System.Numerics;
using Birch.Graphics;
using Y2048.Core.Screens;
using Y2048.Core.Services;

namespace Y2048.Core.Models;

public class BoxModel : RectangleModel
{
    public int Number { get; private set; }
    public string DisplayValue { get; private set; }
    private BoxGeneratorService _generatorService;

    public BoxModel(BoxGeneratorService generatorService, int number, Color color)
    {
        _generatorService = generatorService;
        Number = number;
        DisplayValue = number.ToString();
        Paint.Color = color;
        Contacted += (_, model) =>
        {
            if (model.GetType() == typeof(BoxModel))
            {
                ResolveContact((BoxModel)model);
            }
        };
    }

    private void ResolveContact(BoxModel box)
    {
        if (box.Number == Number)
        {
            _generatorService.GenerateFrom(this, box);

            Remove();
            box.Remove();
        }
    }

    protected override void Draw(IGraphics g)
    {
        base.Draw(g);

        g.DrawText(DisplayValue, 0, -5, new Paint
        {
            TextSize = 25f,
            TextAlign = TextAlign.Center,
            Style = PaintStyle.Fill,
            BoldText = true,
            Color = new Color(255, 255, 255),
        });
    }

    public void SetNumber(int newNumber)
    {
        Number = newNumber;
        DisplayValue = Number.ToString();
    }
}