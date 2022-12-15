using System.Drawing;

namespace SnakeGameEngine.Entities;

internal interface ICollidableEntity : IEntity, IDisposable
{
    public Rectangle BoxCollider { get; }
    public void OnCollision(ICollidableEntity entity);
    public EventHandler<CollisionEventArgs>? Collided { get; set; }
}
