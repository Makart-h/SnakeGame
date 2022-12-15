using System.Drawing;
using System.Numerics;
using SnakeGameEngine.Entities;

namespace SnakeGameEngine.Snake;

internal sealed class SnakeFragment : ISnakeFragment
{
    private Vector2 _nextPosition;
    private bool _isDisposed;
    private readonly ISnakeController _snakeController;
    public SnakeFragment(ISnakeController snakeController, ISnakeFragment? nextFragment, Vector2 position)
    {
        _snakeController = snakeController;
        NextFragment = nextFragment;
        PreviousFragment = null;
        Tag = EntityTag.Snake;
        (Width, Height) = _snakeController.GetSnakeFragmentSize();
        Position = position;
    }

    public ISnakeFragment? PreviousFragment { get; private set; }

    public ISnakeFragment? NextFragment { get; private set; }

    public Rectangle BoxCollider => new((int)Position.X, (int)Position.Y, Width, Height);

    public Vector2 Position { get; private set; }

    public int Width { get; private set; }

    public int Height { get; private set; }

    public EntityTag Tag { get; private set; }

    public EventHandler<CollisionEventArgs>? Collided { get; set; }

    public void OnCollision(ICollidableEntity entity)
    {
        Collided?.Invoke(this, new CollisionEventArgs(this, entity));
        if (entity.Tag == EntityTag.Apple && NextFragment == null)
            Grow();
    }

    public void Update()
    {
        _nextPosition = _snakeController.FindSnakeFragmentNextPosition(this);
    }

    public void LateUpdate()
    {
        Position = _nextPosition;
    }

    /// <summary>
    /// Snake grows from it's furthest element from the head
    /// so the method is run on the previous fragment
    /// unitl the last one is reached. Then it creates new fragment.
    /// </summary>
    public void Grow()
    {
        if (PreviousFragment == null)
        {
            ISnakeFragment newFragment = GameEngineFactory.CreateSnakeFragment(_snakeController, this, Position);
            PreviousFragment = newFragment;
            _snakeController.RegisterSnakeFragment(newFragment);
        }
        else
        {
            PreviousFragment.Grow();
        }
    }

    public void Dispose()
    {
        if(!_isDisposed)
        {
            _isDisposed = true;
            Collided = null;
        }
    }
}
