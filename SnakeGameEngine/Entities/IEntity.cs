using System.Numerics;

namespace SnakeGameEngine.Entities;

internal interface IEntity
{
    public Vector2 Position { get; }
    public int Width { get; }
    public int Height { get; }
    public EntityTag Tag { get; }
}
