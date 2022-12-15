using SnakeGameEngine.Entities;
using SnakeGameEngine.Snake;
using SnakeGameEngine.Utility;
using System.Numerics;

namespace SnakeGameEngine;

internal static class GameEngineFactory
{
    public static IWorldInfo CreateWorldInfo(int width, int height, int cellWidth = 32, int cellHeight = 32)
    {
        return new WorldInfo(width, height, cellWidth, cellHeight);
    }
    public static IGame CreateGame(IWorldInfo worldInfo)
    {
        return new Game(worldInfo);
    }
    public static ISnakeController CreateSnakeController(IGame owner, Direction initialDirection = Direction.Right)
    {
        return new SnakeController(owner, initialDirection);
    }

    public static ISnakeFragment CreateSnakeFragment(ISnakeController snakeController, ISnakeFragment? nextFragment, Vector2 position)
    {
        return new SnakeFragment(snakeController, nextFragment, position);
    }

    public static ICollectibleEntity CreateApple(Vector2 position, IWorldInfo worldInfo, double lifespanInSeconds)
    {
        return new Apple(position, worldInfo.CellWidth, worldInfo.CellHeight, lifespanInSeconds);
    }
}
