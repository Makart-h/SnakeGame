using System.Drawing;
using SnakeGameEngine.Entities;
using SnakeGameEngine.Services;
using SnakeGameEngine.Snake;
using SnakeGameEngine.Utility;

namespace SnakeGameEngine;

internal sealed class Game : IGame
{
    private readonly List<IEntity> _entities;
    private readonly List<IPausableEntity> _pausables;
    private readonly List<IUpdateableEntity> _updateables;
    private readonly List<ICollidableEntity> _collidables;
    private readonly List<IRemovableEntity> _removables;
    private readonly Stack<IRemovableEntity> _toRemove;
    private readonly Stack<IEntity> _toAdd;
    private readonly List<IGameService> _gameServices;

    public Game(IWorldInfo worldInfo)
    {
        _entities = new List<IEntity>();
        _pausables = new List<IPausableEntity>();
        _updateables = new List<IUpdateableEntity>();
        _collidables = new List<ICollidableEntity>();
        _removables = new List<IRemovableEntity>();
        _toRemove = new Stack<IRemovableEntity>();
        _toAdd = new Stack<IEntity>();
        _gameServices = new List<IGameService>();
        IsRunning = true;
        WorldInfo = worldInfo;
        Score = 0;
    }
    public IEnumerable<IEntity> Entities => _entities;

    public IWorldInfo WorldInfo { get; private set; }
    public int Score { get; private set; }

    public bool IsRunning { get; private set; }

    public bool IsPaused { get; private set; }

    public EventHandler<GameEndedEventArgs>? GameEnded { get; set; }

    public void AddEntity(IEntity entity) => _toAdd.Push(entity);

    public void AddGameService(IGameService gameService)
    {
        _gameServices.Add(gameService);
        gameService.Run();
    }

    public void Update()
    {
        if (IsRunning && !IsPaused)
        {
            CheckRemovables();
            HandleEntitiesToRemove();
            HandleEntitiesToAdd();
            UpdateEntities();
            LateUpdateEntities();
            CheckCollisions();
        }
    }

    public void Exit()
    {
        IsRunning = false;
        foreach (IEntity entity in _entities)
        {
            if (entity is IDisposable d)
                d.Dispose();
        }
        foreach(IGameService service in _gameServices)
        {
            if (service is IDisposable d)
                d.Dispose();
        }
    }

    public void Pause()
    {
        if (!IsPaused && IsRunning)
        {
            IsPaused = true;
            foreach (IPausableEntity pausable in _pausables)
            {
                if (!pausable.IsPaused)
                    pausable.Pause();
            }
            foreach (IGameService gameService in _gameServices)
            {
                if (!gameService.IsPaused)
                    gameService.Pause();
            }
        }
    }

    public void Resume()
    {
        if (IsPaused && IsRunning)
        {
            IsPaused = false;
            foreach (IPausableEntity pausable in _pausables)
            {
                if (pausable.IsPaused)
                    pausable.Resume();
            }
            foreach (IGameService gameService in _gameServices)
            {
                if (gameService.IsPaused)
                    gameService.Resume();
            }
        }
    }

    private void UpdateEntities()
    {
        foreach (IUpdateableEntity updateable in _updateables)
        {
            updateable.Update();
        }
    }

    private void LateUpdateEntities()
    {
        foreach (IUpdateableEntity updateable in _updateables)
        {
            updateable.LateUpdate();
        }
    }

    private void CheckRemovables()
    {
        foreach(IRemovableEntity removable in _removables)
        {
            if (removable.IsRemoved)
                _toRemove.Push(removable);
        }
    }

    private void HandleEntitiesToRemove()
    {
        while(_toRemove.TryPop(out IRemovableEntity? removable))
        {
            _entities.Remove(removable);
            _removables.Remove(removable);
            if (removable is IUpdateableEntity u)
                _updateables.Remove(u);
            if (removable is IPausableEntity p)
                _pausables.Remove(p);
            if (removable is ICollidableEntity c)
                _collidables.Remove(c);
        }
    }

    private void HandleEntitiesToAdd()
    {
        while(_toAdd.TryPop(out IEntity? entity))
        {
            _entities.Add(entity);
            if (entity is IRemovableEntity removable)
            {
                _removables.Add(removable);
            }
            if (entity is IUpdateableEntity updateable)
            {
                _updateables.Add(updateable);
            }
            if (entity is IPausableEntity pausable)
            {
                _pausables.Add(pausable);
            }
            if (entity is ICollidableEntity collidable)
            {
                collidable.Collided += OnCollision;
                _collidables.Add(collidable);
            }
        }
    }
    
    private void OnCollision(object? sender, CollisionEventArgs e)
    {
        if(e.First.Tag == EntityTag.Snake && e.Second.Tag == EntityTag.Snake)
        {
            OnGameEnded(new GameEndedEventArgs("Try again! :D", Score));
        }
        else if(e.First.Tag == EntityTag.Snake && e.Second.Tag == EntityTag.Apple)
        {
            if(e.First is ISnakeFragment { NextFragment: null})
                Score++;
        }
    }

    private void OnGameEnded(GameEndedEventArgs e)
    {
        IsRunning = false;
        Exit();
        GameEnded?.Invoke(this, e);
    }

    /// <summary>
    /// Checks all unique pairs to find collisions.
    /// </summary>
    private void CheckCollisions()
    {
        for(int i = 0; i < _collidables.Count; ++i)
        {
            for(int j = i+1; j < _collidables.Count; ++j)
            {
                if (AreColliding(_collidables[i].BoxCollider, _collidables[j].BoxCollider))
                {
                    _collidables[i].OnCollision(_collidables[j]);
                    _collidables[j].OnCollision(_collidables[i]);
                }
            }
        }
    }

    private static bool AreColliding(Rectangle first, Rectangle second)
    {
        return first.X < second.X + second.Width
            && first.X + first.Width > second.X
            && first.Y < second.Y + second.Height
            && first.Height + first.Y > second.Y;
    }
}
