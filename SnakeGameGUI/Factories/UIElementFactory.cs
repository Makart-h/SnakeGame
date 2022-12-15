using SnakeGameEngine.Entities;
using SnakeGameEngine.Snake;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SnakeGameGUI;

internal static class UIElementFactory
{
    public static UIElement CreateUIElement(IEntity entity)
    {
        return entity.Tag switch
        {
            EntityTag.Snake when entity is ISnakeFragment { NextFragment: null } =>
            new Rectangle()
            {
                Width = entity.Width,
                Height = entity.Height,
                Fill = Brushes.Purple,
                Stroke = Brushes.Black,
                StrokeThickness = 2,
            },
            EntityTag.Snake => new Rectangle()
            {
                Width = entity.Width,
                Height = entity.Height,
                Fill = Brushes.BlueViolet,
                Stroke = Brushes.Black,
                StrokeThickness = 2,
            },
            EntityTag.Apple => new Ellipse()
            {
                Width = entity.Width,
                Height = entity.Height,
                Fill = Brushes.Red,
                Stroke = Brushes.Black,
                StrokeThickness = 2,
            },
            _ => throw new NotImplementedException(),
        };
    }
}
