namespace SnakeGameEngine.Entities;

internal sealed class CollisionEventArgs : EventArgs
{
    public ICollidableEntity First { get; }
    public ICollidableEntity Second { get; }

    public CollisionEventArgs(ICollidableEntity first, ICollidableEntity second)
    {
        First = first;
        Second = second;
    }
}
