using System.Timers;
using SnakeGameEngine.Entities;
using SnakeGameEngine.Utility;

namespace SnakeGameEngine.Services;

internal sealed class AppleSpawnerService : IGameService, IDisposable
{
    private readonly IGame _game;
    private readonly PausableTimer _spawnTimer;
    private readonly double _appleLifespanInSeconds;
    private readonly Random _random;
    private bool _isDisposed;
    private readonly List<(int, int)> _everyValidSpawnPoint;

    public bool IsPaused { get; private set; }

    public AppleSpawnerService(IGame game, double cooldownInSeconds = 3, double appleLifespanInSeconds = 5)
    {
        _game = game;
        _random = new Random();
        _spawnTimer = new PausableTimer(TimeSpan.FromSeconds(cooldownInSeconds).TotalMilliseconds, SpawnApple, true);
        _appleLifespanInSeconds = appleLifespanInSeconds;
        _everyValidSpawnPoint = new List<(int, int)>();
        _game.AddGameService(this);        
    }

    private void GenerateSpawnPoints()
    {
        for(int x = 0; x < _game.WorldInfo.WorldWidth; x+=_game.WorldInfo.CellWidth)
        {
            for(int y = 0; y < _game.WorldInfo.WorldHeight; y+=_game.WorldInfo.CellHeight)
            {
                _everyValidSpawnPoint.Add((x, y));
            }
        }
    }

    public void Run()
    {
        GenerateSpawnPoints();
        _spawnTimer.Start();
    }

    public void Pause()
    {
        if (!IsPaused)
        {
            IsPaused = true;
            _spawnTimer.Pause();
        }
    }

    public void Resume()
    {
        if (IsPaused)
        {
            IsPaused = false;
            _spawnTimer.Resume();
        }
    }

    private void SpawnApple(object? sender, ElapsedEventArgs e)
    {
        System.Numerics.Vector2 position;
        var emptyCells = FindEmptyCells();
        int index = _random.Next(emptyCells.Count);
        (position.X, position.Y) = emptyCells[index];
        ICollectibleEntity newApple = GameEngineFactory.CreateApple(position, _game.WorldInfo, _appleLifespanInSeconds);
        _game.AddEntity(newApple);
    }

    private List<(int x, int y)> FindEmptyCells()
    {
        List<(int x, int y)> cells = new(_everyValidSpawnPoint);
        foreach(IEntity entity in _game.Entities)
        {
            cells.Remove(((int x, int y))(entity.Position.X, entity.Position.Y));
        }
        return cells;
    }

    public void Dispose()
    {
        if(!_isDisposed)
        {
            _isDisposed = true;
            _spawnTimer.Dispose();
        }
    }
}
