using SnakeGameEngine.Utility;

namespace SnakeGameGUI;

internal static class WPFSnakeGameFactory
{
    public static IWPFSnakeGame Create(IWorldInfo worldInfo)
    {
        return new WPFSnakeGame(worldInfo);
    }
}
