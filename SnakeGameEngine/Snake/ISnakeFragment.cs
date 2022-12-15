using SnakeGameEngine.Entities;

namespace SnakeGameEngine.Snake;

internal interface ISnakeFragment : ICollidableEntity, IUpdateableEntity
{
    public ISnakeFragment? PreviousFragment { get; }
    public ISnakeFragment? NextFragment { get; }
    public void Grow();
}
