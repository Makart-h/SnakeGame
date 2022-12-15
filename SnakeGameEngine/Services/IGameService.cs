using SnakeGameEngine.Interfaces;

namespace SnakeGameEngine.Services;

internal interface IGameService : IPausable
{
    public void Run();
}
