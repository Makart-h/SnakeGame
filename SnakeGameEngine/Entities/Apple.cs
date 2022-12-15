using System.Drawing;
using System.Numerics;
using System.Timers;
using SnakeGameEngine.Utility;

namespace SnakeGameEngine.Entities;

internal sealed class Apple : ICollectibleEntity, IPausableEntity, IDisposable
{
    private readonly PausableTimer _lifespanTimer;
    private bool _isDisposed;

    public Apple(Vector2 position, int width, int height, double lifespanInSeconds)
    {
        Position = position;
        Width = width;
        Height = height;
        Tag = EntityTag.Apple;
        _lifespanTimer = new PausableTimer(TimeSpan.FromSeconds(lifespanInSeconds).TotalMilliseconds, RemoveOnLifespanElapsed, false);
        _lifespanTimer.Start();
    }
    public Rectangle BoxCollider => new((int)Position.X, (int)Position.Y, Width, Height);

    public Vector2 Position { get; private set; }

    public int Width { get; private set; }

    public int Height { get; private set; }

    public EntityTag Tag { get; private set; }

    public bool IsRemoved { get; private set; }

    public bool IsPaused { get; private set; }

    public EventHandler<CollisionEventArgs>? Collided { get; set; }

    public void OnCollision(ICollidableEntity entity)
    {
        if (!IsRemoved)
        {
            Collided?.Invoke(this, new CollisionEventArgs(this, entity));
            Remove();
        }
    }

    private void RemoveOnLifespanElapsed(object? sender, ElapsedEventArgs e) => Remove();

    public void Remove()
    {
        if (!IsRemoved)
        {
            _lifespanTimer.Dispose();
            Collided = null;
            IsRemoved = true;
        }
    }

    public void Pause()
    {
        if (!IsRemoved && !IsPaused)
        {
            IsPaused = true;
            _lifespanTimer.Pause();
        }
    }

    public void Resume()
    {
        if (!IsRemoved && IsPaused)
        {
            IsPaused = false;
            _lifespanTimer.Resume();
        }
    }

    public void Dispose()
    {
        if(!_isDisposed)
        {
            _isDisposed = true;
            Remove();
        }
    }
}
