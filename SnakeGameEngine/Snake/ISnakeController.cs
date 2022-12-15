using System.Numerics;

namespace SnakeGameEngine.Snake;

internal interface ISnakeController
{
    public Direction Direction { get; set; }
    public Vector2 FindSnakeFragmentNextPosition(ISnakeFragment fragment);
    public (int width, int height) GetSnakeFragmentSize();
    public void RegisterSnakeFragment(ISnakeFragment fragment);
}
