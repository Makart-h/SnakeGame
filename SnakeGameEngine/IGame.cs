using SnakeGameEngine.Entities;
using SnakeGameEngine.Interfaces;
using SnakeGameEngine.Services;
using SnakeGameEngine.Utility;

namespace SnakeGameEngine;

internal interface IGame : IPausable
{
    public EventHandler<GameEndedEventArgs>? GameEnded { get; set; }
    public IEnumerable<IEntity> Entities { get; }
    public int Score { get; }
    public IWorldInfo WorldInfo { get; }
    public bool IsRunning { get; }
    public void AddEntity(IEntity entity);
    public void AddGameService(IGameService gameService);
    public void Update();
    public void Exit();
}
